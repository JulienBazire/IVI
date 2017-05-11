using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IVIMascotBehaviour : MonoBehaviour {

	public GameObject[] ModesPrefab;
	public GameObject Indicator;

	public float SizeOfObject = 0.1f;
	public float SizeOfWheel = 0.5f;
	public float HeightOfWheel = 1f;

	private GameObject modesWheel;
	private List<GameObject> modesIcons = new List<GameObject>();
	private GameObject indic;

	public float DelayToDesactivateWheel = 3.0f;
	private float Delay;

	// Use this for initialization
	void Start () {
		modesWheel = new GameObject("ModesWheel");
		modesWheel.transform.SetParent(transform);
		modesWheel.transform.localPosition = Vector3.zero;

		int modeIndex = 0;
		float SpaceBetweenTools = Mathf.PI / 2;
		if(IVISession.TaskManager.Modes.Count > 2) SpaceBetweenTools = 2*Mathf.PI / IVISession.TaskManager.Modes.Count;

		indic = Instantiate (Indicator, transform);
		indic.transform.localPosition = new Vector3(0, HeightOfWheel + SizeOfWheel*0.8f ,1f);
		indic.transform.localScale = new Vector3 (0.75f, 0.75f, 0.75f);

		foreach (IVIMode mode in IVISession.TaskManager.Modes) {
			GameObject ModeObject = Instantiate (ModesPrefab[(int)mode.modelIndex], modesWheel.transform);

			ModeObject.name = mode.name;
			ModeObject.transform.localScale = new Vector3 (SizeOfObject, SizeOfObject, SizeOfObject);
			ModeObject.transform.localPosition = new Vector3 (Mathf.Cos (modeIndex * SpaceBetweenTools) * SizeOfWheel, Mathf.Sin (modeIndex * SpaceBetweenTools) * SizeOfWheel, 0);
			modesIcons.Add(ModeObject);

			modeIndex++;
		}

		int CurrentModeIndex = IVISession.TaskManager.Modes.IndexOf (IVISession.TaskManager.CurrentMode);

		modesWheel.transform.Rotate (new Vector3 (0, 0, 1),	90f + (CurrentModeIndex * SpaceBetweenTools * 180)/Mathf.PI);

		modesWheel.SetActive (false);
		indic.SetActive (false);

	}

	public void ActivateModesWheel(){
		modesWheel.SetActive (true);
		indic.SetActive (true);
		Delay = Time.time;
	}
	public void DesactivateModesWheel(){
		
	}	

	// Update is called once per frame
	void Update () {
		transform.LookAt (IVISession.User.transform);
		if (modesWheel.activeSelf &&
		   (Time.time - Delay) >= DelayToDesactivateWheel) {
			modesWheel.SetActive (false);
			indic.SetActive (false);
		}
	}
}
