using UnityEngine;
using UnityEngine.UI;

public class IVIUser : MonoBehaviour {

	[HideInInspector]
	public GameObject Focus = null, PrevFocus = null;

	public GameObject VrGui;
	public Text FocusDisplayName;
	public Text FocusDisplayPath;

	public float defaultGuiDistance = 5;

	private bool isRightHanded = true;
	public bool IsRightHanded { 
		get { return isRightHanded; } 
		set { 
			if (isRightHanded == value)
				return;
			isRightHanded = value;
			IVISession.OnHandednessChanged (); 
		}
	}
	[HideInInspector]
	public bool IsLeftHanded { get { return !IsRightHanded; } set { IsRightHanded = !value; } }


	void Start(){
		FocusDisplayName.fontSize = 4 * (int)defaultGuiDistance;
		FocusDisplayPath.fontSize = 2 * (int)defaultGuiDistance;
	}

	void Update() {
		PrevFocus = Focus;
		RaycastHit hit;
		if (Physics.Raycast (transform.position, transform.forward, out hit, Mathf.Infinity, Physics.DefaultRaycastLayers & ~(1<<LayerMask.NameToLayer("Lightless"))))
			Focus = hit.transform.gameObject;
		else
			Focus = null;
		HighightFocus ();
		SoundManager ();
	}

	private bool SameFocus(){
		return Focus == PrevFocus;
	}

	private void HighightFocus(){
		if (Focus == null) {
			FocusDisplayName.text = "";
			FocusDisplayPath.text = "";
			return;
		}

		string DisplayName = "";
		string DisplayPath = "";

		if (Focus.GetComponent<IVIStar> () != null) {
			if (Focus.GetComponent<IVIStar> ().Entry is IVIFile) {
				IVIFile file = Focus.GetComponent<IVIStar> ().Entry as IVIFile;
				DisplayName = file.Systemfile.Name;
				DisplayPath = file.Systemfile.FullName;
			} else {
				IVIDirectory file = Focus.GetComponent<IVIStar> ().Entry as IVIDirectory;
				DisplayName = file.Systemdirectory.Name;
				DisplayPath = file.Systemdirectory.FullName;
			}
		} else if (Focus.GetComponent<IVIMascotBehaviour> () != null) {
			IVISession.Mascot.ActivateModesWheel ();
		} else {
			DisplayName = Focus.name;
			DisplayPath = "";
		}
		FocusDisplayName.text = DisplayName;
		FocusDisplayPath.text = DisplayPath;
	}

	public void SoundManager(){
		if (Focus == null)
			return;
		if (Focus.GetComponent<IVIStar> () != null && !SameFocus ()) {
			IVISession.AudioController.PlayAudioEvent ("star_focus");
		}
	}

}

 