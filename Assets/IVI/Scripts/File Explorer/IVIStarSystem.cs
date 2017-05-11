using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

public class IVIStarSystem : MonoBehaviour
{
	public GameObject StarPrefab = null;
	public string InitialPath = null;
	public IVIDirectory InitialDir = null;

	[HideInInspector] public IVIStar CurrentStar = null;
	public IVIStar NextCurrentStar = null;
	public IVIStar Root { get { return CurrentStar.Root; } }

	[Range(0,4)] public int Depth = 2;
	[Range(0,4)] public int VisualDepth = 2;

	public float L0_Scale = 1f;
	public float L1_OrbitDistance = 8f;
	public float L1_Scale = 4f;
	public float L2R_OrbitDistance = 0f;
	public float L2R_Scale = 1f;
	// The R after L2 means "Recursive".

	[HideInInspector] public bool AllowSweepingStars = true;
	[HideInInspector] public bool AllowEnteringStars = true;
	public float EnterTransitionTime = 1;
	float EnterStartTime = 0;
	float EnterProgress = 1;
	List<IVIStar> EnterDestroyList = new List<IVIStar>();
	List<IVIStar> EnterFadeInList  = new List<IVIStar>();
	List<IVIStar> EnterFadeOutList = new List<IVIStar>();
	bool EnteringChild = true; // Otherwise, it's EnteringParent.

	public GameObject FatalMessageBoxPrefab;

	void Start() {
		GameObject go = Instantiate (StarPrefab, transform);
		CurrentStar = go.GetComponent<IVIStar> ();
		CurrentStar.Scale = L0_Scale;
		if (InitialPath == null || InitialPath.Length <= 0)
			InitialPath = Application.dataPath;
		try {
			CurrentStar.Entry = InitialDir != null ? InitialDir : new IVIDirectory (InitialPath, Depth);
		} catch (System.Exception e) {
			Debug.Log ("Failed to initialize StarSystem: " + e);
			if (FatalMessageBoxPrefab == null) {
				//UnityEditor.EditorApplication.isPlaying = false;
				Application.Quit ();
			} else
				Instantiate (FatalMessageBoxPrefab);
		}
		CurrentStar.DoRecursively (Depth, (s, depth) => s.RegenerateChildren(depth, transform));
		CurrentStar.DoRecursively (s => s.PlaceChildrenAround());
		CurrentStar.DoRecursively (Depth, (s, depth) => s.ResetDistanceAndScale(level:Depth-depth, sys:this));
		CurrentStar.DoRecursively (s => s.SetMaterial());
		CurrentStar.DoRecursively (s => s.SetLight ());
		CurrentStar.DoRecursively (s => s.SetName());
		ApplyVisualDepth ();
	}
	void ApplyVisualDepth() {
		CurrentStar.DoRecursively (Depth, (s, depth) => s.gameObject.SetActive (false));
		CurrentStar.DoRecursively (VisualDepth, (s, depth) => s.gameObject.SetActive (true));
		CurrentStar.gameObject.SetActive (false);
	}

	void Update() {
		if (EnterProgress >= 1) {
			ApplyPinch ();
			ApplySweep ();
			return;
		}
		EnterProgress = Mathf.Clamp01 ((Time.time - EnterStartTime) / EnterTransitionTime);
		OnEnterProgress ();
		if (EnterProgress < 1)
			return;
		OnEnterFinished ();
	}

	void OnEnterProgress() {
		Vector3 mov = CurrentStar.transform.position - NextCurrentStar.transform.position;
		CurrentStar.transform.position = transform.position + Vector3.Lerp(Vector3.zero, mov, EnterProgress);
		if (EnteringChild) {
			foreach (IVIStar child in NextCurrentStar.Children) {
				child.Scale = Mathf.Lerp (L2R_Scale, L1_Scale, EnterProgress);
				child.OrbitDistance = Mathf.Lerp (L2R_OrbitDistance, L1_OrbitDistance, EnterProgress);
			}
			NextCurrentStar.Scale = Mathf.Lerp (L1_Scale, L0_Scale, EnterProgress);
		} else {
			CurrentStar.Scale = Mathf.Lerp (L0_Scale, L1_Scale, EnterProgress);
			CurrentStar.OrbitDistance = Mathf.Lerp (0, L1_OrbitDistance, EnterProgress);
			foreach (IVIStar child in CurrentStar.Children) {
				child.Scale = Mathf.Lerp (L1_Scale, L2R_Scale, EnterProgress);
				child.OrbitDistance = Mathf.Lerp (L1_OrbitDistance, L2R_OrbitDistance, EnterProgress);
			}
			NextCurrentStar.Scale = L0_Scale;
		}

		foreach (IVIStar star in EnterFadeInList)
			star.SetOpacity (EnterProgress);
		foreach (IVIStar star in EnterFadeOutList)
			star.SetOpacity (1-EnterProgress);
	}

	void OnEnterFinished() {
		CurrentStar = NextCurrentStar;
		foreach (IVIStar star in EnterDestroyList) {
			star.Children.ForEach (child => child.Parent = null);
			if(star.Parent != null)
				star.Parent.Children.Remove (star);
			Destroy (star.gameObject);
		}
		CurrentStar.Parent = null;
		CurrentStar.DoRecursively (s => s.SetName ());
		ApplyVisualDepth ();
		EnterDestroyList.Clear ();
		EnterFadeInList.Clear ();
		EnterFadeOutList.Clear ();
	}

	void OnEnterStart() {
		EnterStartTime = Time.time;
		EnterProgress = 0;
	}

	public void EnterChild(IVIStar child) {
		Assert.IsTrue (CurrentStar.Children.Contains(child), 
			"`" + CurrentStar.Entry.FullPath + "' isn't an immediate child of `"
			+ child.Entry.FullPath + "' as expected!"
		);
		Assert.IsTrue (child.Entry is IVIDirectory, "Entering a file!");

		NextCurrentStar = child;
		OnEnterStart ();
		EnteringChild = true;

		NextCurrentStar.DoRecursively (Depth, (s, depth) => s.GenerateNextLevel(depth, this, EnterFadeInList));
		NextCurrentStar.DoRecursively (Depth, (s, depth) => s.DoIf (s_ => depth < 1, s_ => EnterFadeInList.Add (s_)));

		EnterFadeOutList.Add (NextCurrentStar);
		EnterFadeOutList.Add (CurrentStar);
		EnterDestroyList.Add (CurrentStar);
		foreach (IVIStar star in CurrentStar.Children) {
			if (star != NextCurrentStar) {
				star.DoRecursively (s => EnterFadeOutList.Add (s));
				star.DoRecursively (s => EnterDestroyList.Add (s));
			}
		}
	}

	public void EnterParent() {
		IVIDirectory curDir = CurrentStar.Entry as IVIDirectory;
		GameObject parentGo = Instantiate (StarPrefab, transform);		
		NextCurrentStar = parentGo.GetComponent<IVIStar> ();
		NextCurrentStar.Entry = new IVIDirectory (curDir.Systemdirectory.Parent.FullName, Depth);
		// XXX This is done in too many places, should be compressed into a function.
		NextCurrentStar.DoRecursively (Depth, (s, depth) => s.RegenerateChildren(depth, transform));
		NextCurrentStar.DoRecursively (s => s.PlaceChildrenAround());
		NextCurrentStar.DoRecursively (Depth, (s, depth) => s.ResetDistanceAndScale(level:Depth-depth, sys:this));
		NextCurrentStar.DoRecursively (s => s.SetMaterial());
		NextCurrentStar.DoRecursively (s => s.SetLight ());

		foreach (IVIStar child in NextCurrentStar.Children) {
			if (CurrentStar.Entry.FullPath == child.Entry.FullPath) {
				child.Children.ForEach (s_ => s_.DoRecursively_ChildrenFirst(s => Destroy (s.gameObject)));
				child.Children = CurrentStar.Children;
				child.Children.ForEach (s => s.Parent = child);
				Destroy (CurrentStar.gameObject);
				CurrentStar = child;
				EnterFadeInList.Add (CurrentStar);
			} else
				child.DoRecursively (s => EnterFadeInList.Add(s));

			child.DoRecursively (Depth, (s_, depth) => s_.DoIf(s => depth<1, s => EnterFadeOutList.Add(s)));
			child.DoRecursively (Depth, (s_, depth) => s_.DoIf(s => depth<1, s => EnterDestroyList.Add(s)));
		}
		NextCurrentStar.DoRecursively (s => s.SetName ());
		NextCurrentStar.gameObject.SetActive (false);

		OnEnterStart ();
		EnteringChild = false;
	}

	public void ApplyPinch() {
		if (!AllowEnteringStars)
			return;
		Assert.IsTrue(EnterProgress >= 1, "The transition needs to complete first!");
		bool isPerformed = IVISession.PrimaryHand.gestureReleasedPinch.IsBeingPerformed
			&& !IVISession.PrimaryHand.gestureBothHandsSweeping.IsBeingPerformed
			&& !IVISession.PrimaryHand.gestureExtendedIndexRelaxed.IsBeingPerformed;
		if (!isPerformed)
			return;

		if (IVISession.User.Focus == null)
			EnterParent ();
		else {
			IVIStar fstar = IVISession.User.Focus.GetComponent<IVIStar> ();
			if (fstar != null && CurrentStar.Children.Contains (fstar) && fstar.Entry is IVIDirectory)
				EnterChild (fstar);
		}
	}

	protected float prevSweepValue = 0;
	protected bool wasSweeping = false;

	public void ApplySweep() {
		if (!AllowSweepingStars)
			return;

		var g = IVISession.PrimaryHand.gestureBothHandsSweeping;
		bool isPerformed = g.IsBeingPerformed 
			&& !IVISession.PrimaryHand.gestureReleasedPinch.IsBeingPerformed
			&& !IVISession.PrimaryHand.gestureExtendedIndexRelaxed.IsBeingPerformed;

		if (wasSweeping && !isPerformed)
			CurrentStar.Children.ForEach (s => s.CommitSweepValue(prevSweepValue));
		
		CurrentStar.Children.ForEach (s => s.SweepValue = (g.IsBeingPerformed ? g.SweepValue : 0));

		prevSweepValue = g.SweepValue;
		wasSweeping = isPerformed;
	}


	// L'Entry a été déplacée : Déplacer la star dans le système.
	public void NotifyMove(IVIStar dst, IVIStar src) {
		src.Parent.Children.Remove (src);
		src.Parent = dst;
		if (src.Entry.FullPath.StartsWith(dst.Entry.FullPath)) { // HACK déplacement vers un parent
			Destroy (src);
		} else {
			dst.Children.Add (src);
			dst.PlaceChildrenAround ();
		}
		/*
		foreach(IVIStar s in dst.Children)
			Destroy(s);
		dst.Children.Clear ();
		dst.Entry = new IVIDirectory (dst.Entry.FullPath, Depth);
		dst.DoRecursively (s => s.RegenerateChildren (Depth, transform));
		dst.DoRecursively (s => s.PlaceChildrenAround());
		dst.DoRecursively (Depth, (s, d) => s.ResetDistanceAndScale(level:2, sys:this)); // XXX Hack !!
		dst.DoRecursively (s => s.SetMaterial());
		dst.DoRecursively (s => s.SetLight ());
		*/
	}
	// L'Entry a été copiée : Placer la star dans le système.
	public void NotifyCopy(IVIStar dst, IVIStar src) {
		/*
		IVIStar src_cpy = Instantiate (src).GetComponent<IVIStar> ();
		src_cpy.Parent = dst;
		dst.Children.Add (src_cpy);
		dst.DoRecursively (s => s.RegenerateChildren (Depth, transform));
		dst.DoRecursively (s => s.PlaceChildrenAround());
		dst.DoRecursively (Depth, (s, d) => s.ResetDistanceAndScale(level:2, sys:this)); // XXX Hack !!
		dst.DoRecursively (s => s.SetMaterial());
		dst.DoRecursively (s => s.SetLight ());
		*/
	}
}