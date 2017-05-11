using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Leap.Unity;

public class IVIToolBehaviour : MonoBehaviour
{
	private Transform FingerTip;

	private List<GameObject> toolsButtons = new List<GameObject>();
	private List<GameObject> toolsCapsules = new List<GameObject>();
	private int activatedTool = 0;
	private GameObject toolsWheel;
	private bool isUsingTool = false;

	public float SizeOfWheel = 0.2f;
	public float SizeOfObject = 0.05f;
	public float SizeOfObjectCollider = 0.05f;
	public Color ActivatedToolColor = new Color(0,1,0);

	public GameObject[] ToolsPrefab;
	public GameObject CapsulePrefab;
	public GameObject SubTitlesCanvas;
	public GameObject EmptyText;

	public float SubtitleDistance = 0.5f; 

	// Use this for initialization
	void Start (){
		
		toolsWheel = new GameObject("ToolsWheel");
		toolsWheel.transform.SetParent(transform);

		int toolIndex = 0;
		float SpaceBetweenTools = Mathf.PI / 2;
		if(toolsButtons.Count > 0) SpaceBetweenTools = Mathf.PI / toolsButtons.Count;

		foreach (IVITool tool in IVISession.TaskManager.CurrentMode.Tools) {
			GameObject ToolObject = Instantiate (ToolsPrefab[toolIndex], toolsWheel.transform);
			GameObject ToolCaps = Instantiate (CapsulePrefab, ToolObject.transform);

			ToolCaps.name = "";
			ToolObject.transform.localScale = new Vector3 (SizeOfObject, SizeOfObject, SizeOfObject);
			ToolObject.transform.Translate (new Vector3 (Mathf.Cos (toolIndex * SpaceBetweenTools) * SizeOfWheel, Mathf.Sin (toolIndex * SpaceBetweenTools) * SizeOfWheel, 0));
			ToolCaps.transform.localScale = new Vector3 (5f, 5f, 5f);
			toolsButtons.Add(ToolObject);
			toolsCapsules.Add(ToolCaps);

			GameObject subtitle = Instantiate( EmptyText, SubTitlesCanvas.transform);
			subtitle.transform.localScale = new Vector3 (1.0f,1.0f,1.0f);
			subtitle.transform.localPosition = 1000.0f * new Vector3 (Mathf.Cos (toolIndex * SpaceBetweenTools) * SizeOfWheel, Mathf.Sin (toolIndex * SpaceBetweenTools) * SizeOfWheel + SubtitleDistance, 0);

			subtitle.GetComponent<Text>().text = tool.name;

			toolIndex++;
		}

		Highlights (0);
	}

	public void InitPointer(Transform finger){
		FingerTip = finger;
		UnityEngine.Assertions.Assert.IsNotNull (FingerTip);
	}

	// Update is called once per frame
	void Update (){
		bool WheelActivation = IVISession.PrimaryHand.gestureExtendedIndexRelaxed.IsBeingPerformed;
		bool CloseFist = IVISession.PrimaryHand.gestureClosedFist.IsBeingPerformed;

		toolsWheel.SetActive (WheelActivation);
		SubTitlesCanvas.SetActive (WheelActivation);
		if (!WheelActivation) FollowHand();
		else ToolSelection ();

		if (CloseFist && !isUsingTool) {
			ToolActivation ();
			IVIUser user = IVISession.User;
			if (user.Focus != null && user.Focus.GetComponent<IVIStar> () != null)
				IVISession.PrimaryHand.OnSuccessfulToolGestureBegin ();
			else
				IVISession.PrimaryHand.OnFailedToolGestureBegin ();
			isUsingTool = true;
		}
		if (!CloseFist && isUsingTool) {
			isUsingTool = false;
			IVISession.PrimaryHand.OnToolGestureEnd();
		}
	}

	public void ToolSelection(){
		int index = 0;
		foreach (GameObject tool in toolsButtons) {
			if (DetectFingerCollision (tool)) {
				SwitchTool_Internal (index);
			}
			index++;
		}
	}
	public enum ToolIndex
	{
		SELECT = 0,
		MOVE_SELECTION = 1,
		COPY_SELECTION = 2
	};
	public void SwitchTool(ToolIndex i) {
		SwitchTool_Internal ((int)i);
	}
	void SwitchTool_Internal(int index) {
		IVISession.TaskManager.CurrentMode.CurrentTool = IVISession.TaskManager.CurrentMode.Tools [index];
		Desactivate (activatedTool);
		Highlights (index);
		activatedTool = index;
	}
	public void ToolActivation(){
		Debug.Log ("Utilisation de " + IVISession.TaskManager.CurrentMode.CurrentTool.GetType().FullName);

		IVISession.TaskManager.CurrentMode.CurrentTool.Use ();
	}

	private bool DetectFingerCollision(GameObject other){
		float dist = Vector3.Distance (FingerTip.position, other.transform.position);
		return dist <= SizeOfObjectCollider;
	}

	private void Highlights(int index){
		toolsCapsules [index].GetComponent<Renderer> ().material.SetColor ("_EmissionColor",  ActivatedToolColor);
	}
	private void Desactivate(int index){
		toolsCapsules [index].GetComponent<Renderer> ().material.SetColor ("_EmissionColor", new Color(0.1f, 0.1f, 0.1f));
	}

	public void FollowHand(){
		Leap.Hand hand = IVISession.PrimaryHand.LeapHand;
		transform.position = hand.PalmPosition.ToVector3();
		transform.LookAt (IVISession.User.transform);
	}
}

