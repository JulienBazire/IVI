using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IVIStar : MonoBehaviour {

	[HideInInspector] public IVIEntry Entry = null;
	public IVIStar Parent = null;
	public List<IVIStar> Children = new List<IVIStar>();
	public float Scale = 1;

	public Vector3 RevolutionUpVector = Vector3.up;
	public float RevolutionAngle = 0;
	public float RevolutionSpeed = 0;
	public float OrbitUpOffsetHack = 0;
	public float OrbitAngleZ = 0;
	public float OrbitAngleY = 0;
	public float OrbitSpeed = 0;
	[Tooltip("0 means 'Surface-to-surface with parent'. World-scale.")]
	public float OrbitDistance = 0;

	// XXX TODO document this !
	public float SweepValue = 0;

	public bool IsRoot { get { return Parent == null; } }
	public IVIStar Root { get { return IsRoot ? this : Parent.Root; } }
	public int ComputedDepth { get { return IsRoot ? 0 : 1+Parent.ComputedDepth; } }


	/// <summary> Performs an action on this star's children, then on itself, recursively. </summary>
	public void DoRecursively_ChildrenFirst(System.Action<IVIStar> d) {
		foreach (IVIStar child in Children)
			child.DoRecursively (d);
		d (this);
	}
	/// <summary> Performs an action on this star, then on its children, recursively. </summary>
	public void DoRecursively(System.Action<IVIStar> d) {
		d (this);
		foreach (IVIStar child in Children)
			child.DoRecursively (d);
	}
	/// <summary> Performs an action on this star, then on its children, recursively. </summary>
	public void DoRecursively(int depth, System.Action<IVIStar, int> d) {
		d (this, depth);
		if (depth > 0)
			foreach (IVIStar child in Children)
				child.DoRecursively(depth-1, d);
	}

	/// <summary> Performs a search by checking this star, then its children, recursively. Returns a single element. </summary>
	public IVIStar SearchRecursively(System.Predicate<IVIStar> pred) {
		if (pred (this))
			return this;
		foreach (IVIStar child in Children) {
			IVIStar found = child.SearchRecursively (pred);
			if (found != null)
				return found;
		}
		return null;
	}

	public void DoIf(System.Predicate<IVIStar> pred, System.Action<IVIStar> d) {
		if (pred (this))
			d (this);
	}


	// Seulement applicable aux stars qui ne sont pas celles affichées dans le clipboard
	private Color initial_color = Color.magenta;

	public void NotifySelected() {
		GetComponent<Renderer>().material.color = Color.green;
	}
	public void NotifyDeselected() {
		GetComponent<Renderer>().material.color = initial_color;
	}
	void Start() {
		initial_color = GetComponent<Renderer> ().material.color;
	}



	public void SetOpacity(float opacity) {
		Material mat = GetComponent<Renderer> ().material;
		Color col = mat.color;
		col.a = opacity;
		mat.color = col;
	}
	public void SetMaterial(){
		if (Entry is IVIDirectory)
			return;
		IVIFile file = Entry as IVIFile;
		string NameMat = file.Type.ToString ();
		Material mat = Resources.Load<Material>("Materials/" + NameMat);
		Renderer rend = GetComponent<Renderer> (); 
		rend.material = mat;
	}
	public void SetLight(){
		if (Entry is IVIFile)
			Destroy (GetComponent<Light> ());
	}

	public void SetName() {
		gameObject.name = "[" + ComputedDepth + "] " + Entry.FullPath;
	}

	public void RegenerateChildren(int depth, Transform sysTransform) {
		// XXX depth is unused !
		SetName ();
		if (Entry is IVIFile)
			return;
		IVIDirectory dir = Entry as IVIDirectory;
		foreach(IVIEntry childEntry in dir.Entries) {
			GameObject go = Instantiate(gameObject, sysTransform);
			IVIStar child = go.GetComponent<IVIStar>();
			child.Children.Clear ();
			child.Parent = this;
			child.Entry = childEntry;
			Children.Add (child);
		}
	}
	public void GenerateNextLevel(int depth, IVIStarSystem sys, List<IVIStar> addTo) {
		if (depth != 1)
			return;
		if (Entry is IVIFile)
			return;
		Entry = new IVIDirectory (Entry.FullPath, 1);
		// XXX FIXME !! It's ugly ! This is done in two places.
		DoRecursively (s => s.RegenerateChildren (depth, sys.transform));
		DoRecursively (s => s.PlaceChildrenAround());
		DoRecursively (ComputedDepth, (s, d) => s.ResetDistanceAndScale(level:2, sys:sys)); // XXX Hack !!
		DoRecursively (s => s.SetMaterial());
		DoRecursively (s => s.SetLight ());
		DoRecursively (s => addTo.Add (s));
		addTo.Remove (this);
	}


	public void PlaceChildrenAround() {
		float i = 0;
		foreach (IVIStar child in Children) {
			child.OrbitAngleY = 360f * i / (float)Children.Count;
			++i;
		}
	}

	public void ResetDistanceAndScale(int level, IVIStarSystem sys) {
		if (level==0)
			return;
		Scale = (level==1 ? sys.L1_Scale : sys.L2R_Scale);
		OrbitDistance = (level==1 ? sys.L1_OrbitDistance : sys.L2R_OrbitDistance);
		OrbitSpeed = Random.Range (.5f, .8f);
		OrbitAngleZ = Random.Range (-20f, 20f);
		OrbitUpOffsetHack = sys.L0_Scale * Random.Range (-0.1f, 0.1f);
		RevolutionSpeed = Random.Range (1f, 20f);
		RevolutionUpVector = Random.rotation * Vector3.up;
	}

	public float ConvertedSweepValue { get { return SweepValue * 720f; } }
	public float ActualOrbitAngleY { get { return Mathf.Repeat(OrbitAngleY + ConvertedSweepValue, 360); } }
	public void CommitSweepValue(float v) {
		OrbitAngleY = ActualOrbitAngleY;
	}

	public void RecomputeTransform() {
		float actualOrbitDistance = (IsRoot ? 0 : OrbitDistance + Scale/2 + Parent.Scale/2);
		Vector3 orbitRight      = Quaternion.AngleAxis(OrbitAngleZ   , Vector3.forward) * Vector3.right;
		Vector3 orbitUpVector   = Quaternion.AngleAxis(OrbitAngleZ+90, Vector3.forward) * Vector3.right;
		transform.localRotation = Quaternion.AngleAxis(RevolutionAngle, RevolutionUpVector);
		transform.position = Quaternion.AngleAxis(ActualOrbitAngleY, orbitUpVector) * orbitRight * actualOrbitDistance;
		transform.position += orbitUpVector.normalized * OrbitUpOffsetHack;
		if (!IsRoot) {
			transform.position += Parent.gameObject.transform.position;
			transform.localScale = Vector3.one * Scale;
		}
	}

	void Update () {
		OrbitAngleY = Mathf.Repeat(OrbitAngleY + OrbitSpeed*Time.smoothDeltaTime, 360);
		RevolutionAngle = Mathf.Repeat(RevolutionAngle + RevolutionSpeed*Time.smoothDeltaTime, 360);
		RecomputeTransform ();
	}

}
