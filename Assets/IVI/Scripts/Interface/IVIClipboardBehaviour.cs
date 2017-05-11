//#define TEST_WITH_FAKE_ENTRIES

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Leap.Unity;
using UnityEngine.VR;

public class IVIClipboardBehaviour : MonoBehaviour {

	public float SizeOfPrimaryElement = 0.1f;
	public float DistanceOfSystemToPalm = 0.1f;

	public float MaxBeginDistanceFromUser = 0.4f;
	public float MinVelocityToThrow = 0.2f;

	public IVIStarSystem SourceStarSystem;

	[HideInInspector]
	public IVIDirectory TrashDirectory;

	private GameObject selection;
	private IVIStarSystem selection_star_system;

	// Etablit une correspondance pour chaque référence d'étoile possédée par le clipboard
	// vers la référence d'étoile correspondante sur le StarSystem source.
	private Dictionary<IVIStar, IVIStar> selection_sys_to_src_sys = new Dictionary<IVIStar, IVIStar>();

	void Start () {
		IVISession.SecondaryHand.gestureThrowingForward.MaxBeginDistanceFromUser = MaxBeginDistanceFromUser;
		IVISession.SecondaryHand.gestureThrowingForward.MinVelocityToThrow = MinVelocityToThrow;

		TrashDirectory = new IVIDirectory (GetTrashPath(), 0);
		Debug.Log("Trash path : " + TrashDirectory.FullPath);

		// Créer un GameObject "Selection" qui est un StarSystem.
		// On ne fait ça qu'une fois.
		selection = new GameObject("Selection");
		selection.transform.SetParent(transform);
		selection_star_system = selection.AddComponent<IVIStarSystem>();
		#if TEST_WITH_FAKE_ENTRIES
		selection_star_system.InitialPath = @"C:\Users\Documents\test_clipboard";
		#else
		selection_star_system.InitialPath = null;
		selection_star_system.InitialDir = new IVIDirectory(new List<IVIEntry>());
		#endif
		// On fait Instantiate pour ne pas prendre le Prefab par référence
		// (on disable son trailrenderer)
		selection_star_system.StarPrefab = Instantiate(SourceStarSystem.StarPrefab, selection_star_system.transform);
		selection_star_system.StarPrefab.name = "ClipboardStarPrefab";
		selection_star_system.StarPrefab.GetComponent<TrailRenderer> ().enabled = false;
		selection_star_system.StarPrefab.SetActive (false);
		selection_star_system.AllowEnteringStars = false;
		selection_star_system.AllowSweepingStars = false;
		selection_star_system.L0_Scale = 0.2f/*/8f*/;
		selection_star_system.L1_Scale = 0.06f/*/8f*/;
		selection_star_system.L2R_Scale = selection_star_system.L1_Scale / 6f;
		selection_star_system.L1_OrbitDistance = 0;
		selection_star_system.L2R_OrbitDistance = 0;
		// Le clipboard est une hiérarchie "plate", grâce à la façon
		// dont l'acte de sélection est géré.
		selection_star_system.Depth = 1;
		selection_star_system.VisualDepth = 1;
	}

	void OnSelectionStarSystemChanged() {
		selection_star_system.Root.DoRecursively (s => s.PlaceChildrenAround());
		selection_star_system.Root.DoRecursively (1, (s, depth) => s.ResetDistanceAndScale(level:1-depth, sys:selection_star_system));
		selection_star_system.Root.DoRecursively (s => s.SetMaterial());
		selection_star_system.Root.DoRecursively (s => s.SetLight ());
		selection_star_system.Root.DoRecursively (s => s.SetName());
		selection_star_system.Root.gameObject.SetActive (false);	
	}

	void Update () {
		if (IVISession.SecondaryHand.gestureClosedFist.IsBeingPerformed) {
			DeselectAll ();
			IVISession.SecondaryHand.PushColorOnce (Color.yellow);
		} else {
			IVISession.SecondaryHand.PopColorOnce ();
		}

		bool show = IVISession.SecondaryHand.gesturePalmFacingCamera.IsBeingPerformed;
		selection.SetActive (show);
		if (!show)
			return;

		// Follow hand
		Leap.Hand hand = IVISession.SecondaryHand.LeapHand;
		transform.up = hand.PalmNormal.ToVector3 ();
		Vector3 pos = hand.PalmPosition.ToVector3();
		if(VRDevice.isPresent) // Since Unity 5.2
			;//pos.y += 1; // XXX Hack
		transform.position = pos;
		//transform.Translate (DistanceOfSystemToPalm * Vector3.up);
		selection.transform.localPosition = Vector3.zero;
		selection.transform.localRotation = Quaternion.identity;
		selection.transform.localScale = Vector3.one;


		// Are we throwing the selection ?

		IVIGestureThrowingForward throwing = IVISession.SecondaryHand.gestureThrowingForward;
		Debug.Log ("ThrowingState : " + throwing.State.Method.Name);
		if (!throwing.IsBeingPerformed)
			return;
		Debug.Log ("Throwing with Force = " + throwing.Force);
		
		#if true
		GameObject thrownSystem = new GameObject ("ThrownSystem");
		thrownSystem.AddComponent<Rigidbody> ();
		thrownSystem.transform.SetParent (null);
		thrownSystem.transform.position = selection.transform.position;
		foreach (Transform selectionSystem in selection.transform) {
			Debug.Log("adding " + selectionSystem.gameObject.name);
			selectionSystem.gameObject.SetActive (true);
			selectionSystem.SetParent (thrownSystem.transform);
		}
		thrownSystem.GetComponent<Rigidbody> ().useGravity = false;
		thrownSystem.GetComponent<Rigidbody>().AddForce(throwing.Force, ForceMode.Impulse);
		Destroy (thrownSystem, 5);
		//thrownSystem.AddComponent<IVIThrownSystem> ();
		//thrownSystem.GetComponent<IVIThrownSystem> ().BlackHole = BlackHole.transform;
		//thrownSystem.GetComponent<IVIThrownSystem> ().BlackHoleGravity = BlackHoleGravity;
		#else
		selection.AddComponent<Rigidbody> ();
		selection.transform.SetParent (null);
		selection.GetComponent<Rigidbody> ().useGravity = false;
		selection.GetComponent<Rigidbody> ().AddForce (throwing.Force, ForceMode.Impulse);
		Destroy (selection, 5);
		#endif

		// MoveSelectionIntoTrash();
	}

	public static string GetTrashPath() {
		return Path.Combine (Application.dataPath, "IVIBin");
	}

	public void DeselectAll() {
		foreach (IVIStar star in selection_star_system.Root.Children) {
			IVIStar star_in_src_sys = null;
			selection_sys_to_src_sys.TryGetValue (star, out star_in_src_sys);
			if (star_in_src_sys != null) {
				star_in_src_sys.NotifyDeselected ();
			}
			Destroy (star.gameObject);
		}
		selection_star_system.Root.Children.Clear ();
		selection_sys_to_src_sys.Clear ();
	}

	// ToggleSelect(star) remplace Select(star).
	// "Select(star)" ne communique pas l'intention de déselectionner une Star
	// si elle est déjà sélectionnée.
	public void ToggleSelect(IVIStar candidate) {
		// Pour chaque star déjà dans la sélection
		foreach (IVIStar present in selection_star_system.Root.Children) {
			// Test si la star ne provient pas du star_system du clipboard même
			// (égalité de références)
			if (candidate == present) {
				Debug.Log ("[Clipboard]: Ignored " + candidate.Entry.FullPath);
				return;
			}

			// Si la candidate est déjà présente, on la désélectionne.
			if(candidate.Entry.FullPath.Equals(present.Entry.FullPath)) {
				selection_star_system.Root.Children.Remove(present);
				selection_sys_to_src_sys.Remove (present);
				Destroy (present);
				candidate.NotifyDeselected();
				Debug.Log ("[Clipboard]: Deselected " + candidate.Entry.FullPath);
				OnSelectionStarSystemChanged ();
				// C'est mauvais d'appeler Remove() pendant qu'on itère sur la collection,
				// mais on est sauvés par le return.
				return;
			}

			// Test si un supérieur/inférieur de star est déjà dans la sélection
			if (candidate.Entry.FullPath.StartsWith (present.Entry.FullPath)
			   || present.Entry.FullPath.StartsWith (candidate.Entry.FullPath)) {
				Debug.Log ("[Clipboard]: Replaced `" + present.Entry.FullPath
						 + "` by `" + candidate.Entry.FullPath + "` ");
				IVIStar present_in_src_sys;
				selection_sys_to_src_sys.TryGetValue (present, out present_in_src_sys);
				present_in_src_sys.NotifyDeselected();
				candidate.NotifySelected();
				var candidate_copy = Instantiate (candidate, selection_star_system.transform).GetComponent<IVIStar> ();
				candidate_copy.Parent = selection_star_system.Root;
				candidate_copy.Entry = candidate.Entry; // Nécessaire
				selection_star_system.Root.Children.Add(candidate_copy);
				selection_star_system.Root.Children.Remove(present);
				Destroy (present);
				selection_sys_to_src_sys.Add (candidate_copy, candidate);
				selection_sys_to_src_sys.Remove (present);
				OnSelectionStarSystemChanged ();
				// C'est mauvais d'appeler Remove() pendant qu'on itère sur la collection,
				// mais on est sauvés par le return.
				return;
			}
		}

		candidate.NotifySelected();
		var candidate_cpy = Instantiate (candidate, selection_star_system.transform).GetComponent<IVIStar> ();
		candidate_cpy.Parent = selection_star_system.Root;
		candidate_cpy.Entry = candidate.Entry; // OK, c'est nécessaire !
		selection_star_system.Root.Children.Add(candidate_cpy);
		selection_sys_to_src_sys.Add (candidate_cpy, candidate);
		Debug.Log ("[Clipboard]: Selected " + candidate.Entry.FullPath);
		OnSelectionStarSystemChanged ();
	}

	public void MoveSelectionIntoTrash() {
		IVIStar trash = new IVIStar ();
		trash.Entry = TrashDirectory;
		MoveSelectionInto(trash);
	}

	public void MoveSelectionInto(IVIStar target){
		if (target.Entry is IVIFile) {
			Debug.Log ("Won't move selection: target is not a directory");
			return;
		}
		foreach (IVIStar star in selection_star_system.Root.Children) {
			try {
				if(target.Entry.FullPath.StartsWith(star.Entry.FullPath))
					continue;
				star.Entry.CutTo (target.Entry as IVIDirectory);
				IVIStar star_in_src_sys = null;
				selection_sys_to_src_sys.TryGetValue(star, out star_in_src_sys);
				SourceStarSystem.NotifyMove(dst:target, src:star_in_src_sys);
			} catch(Exception e) {
				Console.WriteLine ("Failed to move `"+star.Entry.FullPath+"` : " + e);
			}
		}
		DeselectAll ();
	}
	public void CopySelectionInto(IVIStar target){
		if (target.Entry is IVIFile) {
			Debug.Log ("Won't copy selection: target is not a directory");
			return;
		}
		foreach (IVIStar star in selection_star_system.Root.Children) {
			try {
				if(target.Entry.FullPath.StartsWith(star.Entry.FullPath))
					continue;
				IVIEntry new_entry = star.Entry.CopyTo (target.Entry as IVIDirectory);
				IVIStar star_in_src_sys = null;
				selection_sys_to_src_sys.TryGetValue(star, out star_in_src_sys);
				star_in_src_sys.Entry = new_entry;
				SourceStarSystem.NotifyCopy(dst:target, src:star_in_src_sys);
			} catch(Exception e) {
				Console.WriteLine ("Failed to copy `"+star.Entry.FullPath+"` : " + e);
			}
		}
		DeselectAll ();
	}
}
