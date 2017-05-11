public class IVIGesturesDetectors : UnityEngine.MonoBehaviour
{
	public Leap.Unity.ExtendedFingerDetector ExtendedIndexRelaxed;
	public Leap.Unity.PinchDetector Pinch;
	public Leap.Unity.ExtendedFingerDetector ClosedFist;
	public Leap.Unity.ExtendedFingerDetector AllFingersExtended;
	public Leap.Unity.PalmDirectionDetector  PalmFacingCamera;
	public Leap.Unity.PalmDirectionDetector  PalmFacingLeft;
	public Leap.Unity.PalmDirectionDetector  PalmFacingRight;

	public void UseLeapHandModel(Leap.Unity.IHandModel leapHandModel) {
		ExtendedIndexRelaxed.HandModel = leapHandModel;
		Pinch.HandModel = leapHandModel;
		ClosedFist.HandModel = leapHandModel;
		AllFingersExtended.HandModel = leapHandModel;
		PalmFacingCamera.HandModel = leapHandModel;
		PalmFacingLeft.HandModel = leapHandModel;
		PalmFacingRight.HandModel = leapHandModel;
	}
}

