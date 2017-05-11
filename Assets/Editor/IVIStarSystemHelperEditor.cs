using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IVIStarSystem))]
class IVIStarSystemHelperEditor : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		IVIStarSystem sys = target as IVIStarSystem;
		if (GUILayout.Button ("Enter Parent now!"))
			sys.EnterParent ();
		if (GUILayout.Button("Enter Child now!"))
			sys.EnterChild (sys.NextCurrentStar);
	}
}