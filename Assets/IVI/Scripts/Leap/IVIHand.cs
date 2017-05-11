using UnityEngine;

public abstract class IVIHand
{
	//<summary>Remember to check for IsDetected's value.</summary>
	[HideInInspector] public Leap.Hand LeapHand = new Leap.Hand();
	[HideInInspector] public bool IsDetected;
	[HideInInspector] public IVIGesturesDetectors Detectors; // Clone of the IVIGesturesDetectorsSettings
	protected Leap.Unity.IHandModel leapHandModel;
	public Leap.Unity.IHandModel LeapHandModel { 
		get { return leapHandModel; } 
		set { leapHandModel = value; Detectors.UseLeapHandModel(leapHandModel); } 
	}
	public Leap.Unity.IHandModel LeapHandGraphicsModel;

	[HideInInspector] public Renderer Renderer {
		get { return LeapHandGraphicsModel.GetComponentInChildren<Renderer> (); }
	}

	public void Detect(Leap.Hand hand) {
		LeapHand = hand;
		IsDetected = true;
		LeapHandGraphicsModel.gameObject.SetActive(true);
	}

	public void Undetect() {
		LeapHand = new Leap.Hand();
		IsDetected = false;
		LeapHandGraphicsModel.gameObject.SetActive(false);
	}

	public abstract void UpdateSingleHandGestures ();
	public abstract void UpdateHandPairGestures ();
}
