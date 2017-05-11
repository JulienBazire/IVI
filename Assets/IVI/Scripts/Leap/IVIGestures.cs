// Definition for the IVISingleHandGesture/IVIHandPairGesture interfaces 
// and most gestures used by IVI.
// A "gesture" here is a class that manages its own data, strictly related
// to what its reader would be interested in.
// Leap's Detectors are not used "as-is" by IVI - They must be handled only
// by specialized IVIGestures which take care of "translating" them into what's
// actually meant. 
// It just so happens that some IVIGestures don't require Leap's Detectors, and some
// are simple pass-throughs.
// 
// To add support for a new gesture, create an IVIGestureSomething class
// which implements either IVISingleHandGesture of IVIHandPairGesture. 
// Then, add it as a member
// of the relevant class (probably IVIPrimaryHand/IVISecondaryHand), 
// and take care to call its Update() method where relevant.
//
// If your new gesture would benefit from some of Leap's Detectors, then
// add those as public members into the IVIGesturesDetectors class, and add
// the required lines into its UseLeapHandModel() method.
// Then fill what's missing in the inspector - you'll be able to configure the Detectors
// and display their gizmos in the editor.
//
// Also, you might want to add a new GameObject member to IVIPerHandDetectorHub, which
// would contain one or several of Leap's Detector components. You'd choose the appropriate
// detectors.
//
// --- ANNEX : gesture wishlist---
// Primary and secondary :
// - secondary Close fist + primary palm on secondary;
// Primary only :
// - Index raised (others closed, ignoring thumb and major).
//   + Looking at IVI, select mode
// - Sweep to turn around directories
// Secondary only :
// - Palm facing camera (clipboard)
// - Close fist + throw

using UnityEngine;
using UnityEngine.Assertions;
using Leap.Unity;
using System.Linq;


public interface IVISingleHandGesture {
	bool IsBeingPerformed { get; }
	void Update(IVIHand hand);
}
public interface IVIHandPairGesture {
	bool IsBeingPerformed { get; }
	// The rationale for these two methods is to enforce correctness.
	// That is, there is a hand and an "other" hand, and we must make sure
	// that one is primary and the other is secondary.
	// In pratice, you'll want to implement some kind of...
	//     void UpdateTwo(IVIHand hand, IVIHand other) {}
	// ... that both of these Update() methods would call, so you write 
	// the actual update code only once.
	void Update(IVIPrimaryHand   hand, IVISecondaryHand other);
	void Update(IVISecondaryHand hand, IVIPrimaryHand   other);
}


// This Gesture is performed when :
// - the index finger is extended;
// - the ring finger and pinky are closed;
// The thumb and middle finger are ignored, hence the "Relaxed".
public class IVIGestureExtendedIndexRelaxed : IVISingleHandGesture {
	public bool IsBeingPerformed { get; protected set; }
	public void Update(IVIHand hand) {
		IsBeingPerformed = hand.Detectors.ExtendedIndexRelaxed.IsActive;
		//Debug.Log (GetType().FullName + " : " + IsBeingPerformed);
	}
}


public class IVIGestureClosedFist : IVISingleHandGesture {
	public bool IsBeingPerformed { get; protected set; }
	public void Update(IVIHand hand) {
		IsBeingPerformed = hand.Detectors.ClosedFist.IsActive;
		//Debug.Log (GetType().FullName + " : " + IsBeingPerformed);
	}
}


public class IVIGestureContactWithOtherPalm : IVIHandPairGesture {
	public bool IsBeingPerformed { get; protected set; }
	public void Update(IVIPrimaryHand   hand, IVISecondaryHand other) {
		UpdateTwo(hand, other);
	}
	public void Update(IVISecondaryHand hand, IVIPrimaryHand   other) {
		UpdateTwo(hand, other);
	}
	protected static readonly float MAXDIST = .1f;
	protected void UpdateTwo(IVIHand hand, IVIHand other) {
		// XXX Disable it for now since it doesn't work as expected.
		IsBeingPerformed = false;
		#if false
		Vector3 ppos = hand .LeapHand.PalmPosition.ToVector3();
		Vector3 opos = other.LeapHand.PalmPosition.ToVector3();
		float dist = Vector3.Distance (ppos, opos);
		Debug.Log (dist);
		IsBeingPerformed = hand.Detectors.ClosedFist.IsActive 
			            && other.Detectors.AllFingersExtended.IsActive
						&& dist <= MAXDIST;
		#endif
	}
}

public class IVIGesturePalmFacingCamera : IVISingleHandGesture {
	public bool IsBeingPerformed { get; protected set; }
	public void Update(IVIHand hand) {
		IsBeingPerformed = hand.Detectors.AllFingersExtended.IsActive 
						&& hand.Detectors.PalmFacingCamera.IsActive;
		//Debug.Log (GetType().FullName + " : " + IsBeingPerformed);
	}
}

// Both hands must face the same direction, which in turn is either left or right.
public class IVIGestureBothHandsSweeping : IVIHandPairGesture {
	public bool IsBeingPerformed { get; protected set; }
	protected Vector3 initialPos, currentPos, initialCameraRight;
	public float SweepValue { 
		get { 
			if (!IsBeingPerformed)
				return 0;
			Vector3 move = currentPos - initialPos;
			initialCameraRight = Vector3.ProjectOnPlane(initialCameraRight, Vector3.up);
			Quaternion rot = Quaternion.FromToRotation (initialCameraRight, Vector3.right);
			return (rot * move).x;
		} 
	}

	public void Update(IVIPrimaryHand   hand, IVISecondaryHand other) { UpdateTwo(hand, other); }
	public void Update(IVISecondaryHand hand, IVIPrimaryHand   other) { UpdateTwo(hand, other); }
	protected void UpdateTwo(IVIHand hand, IVIHand other) {
		bool handFacesLeft   = hand .Detectors.PalmFacingLeft .IsActive;
		bool otherFacesLeft  = other.Detectors.PalmFacingLeft .IsActive;
		bool handFacesRight  = hand .Detectors.PalmFacingRight.IsActive;
		bool otherFacesRight = other.Detectors.PalmFacingRight.IsActive;
		bool both_face_same_direction = (handFacesLeft && otherFacesLeft) || (handFacesRight && otherFacesRight);
		bool are_both_detected = hand.IsDetected && other.IsDetected;
		if (!(are_both_detected && both_face_same_direction)) {
			IsBeingPerformed = false;
			//Debug.Log (GetType().FullName + " : " + IsBeingPerformed);
			return;
		}
		Vector3 pos = Vector3.Lerp(hand.LeapHand.PalmPosition.ToVector3(), other.LeapHand.PalmPosition.ToVector3(), .5f);
		if (!IsBeingPerformed) {
			initialCameraRight = Vector3.ProjectOnPlane(IVISession.User.transform.right, Vector3.up);
			initialPos = pos;
		}
		currentPos = pos;
		IsBeingPerformed = true;
		Debug.Log(GetType().FullName + " : " + IsBeingPerformed);
		Debug.Log(GetType().FullName + " : " + SweepValue);
	}
}

public enum ThrowingState {
	Undefined,
	FirstStep,
	ThrowStep
}
	
public class IVIGestureThrowingForward : IVISingleHandGesture {

	public bool IsBeingPerformed { get; protected set; }
	public System.Action<IVIHand> State;
	public Vector3 FistClosedPos = Vector3.zero;
	public float FistClosedTime = 0;
	public Vector3 Force = Vector3.zero;
	// !!! These two are overriden BY the IVIClipboardBehaviour.
	public float MaxBeginDistanceFromUser;
	public float MinVelocityToThrow;
	public float MinThrowDistanceFromClosedFist = 0.2f;
	public float MinThrowDuration = 0f;

	public IVIGestureThrowingForward() {
		IsBeingPerformed = false;
		State = UpdateResting;
	}
	public void Update(IVIHand hand) {
		State (hand);
	}
	void UpdateResting(IVIHand hand) {
		IsBeingPerformed = false;
		float dist = Vector3.Distance (hand.LeapHand.PalmPosition.ToVector3 (), IVISession.User.transform.position);
		//Debug.Log ("Rest: ClosedFist = "+hand.Detectors.ClosedFist.IsActive+", Dist = " + dist + ", MaxDist = " + MaxBeginDistanceFromUser);
		if (hand.Detectors.ClosedFist.IsActive && dist <= MaxBeginDistanceFromUser) 
		{
			FistClosedPos = hand.LeapHand.PalmPosition.ToVector3 ();
			FistClosedTime = Time.time;
			State = UpdateFistClosed;
		}
	}

	void UpdateFistClosed(IVIHand hand) {
		Force = Vector3.zero;
		float dist = Vector3.Distance (FistClosedPos, hand.LeapHand.PalmPosition.ToVector3 ());
		float duration = Time.time - FistClosedTime;
		float speed = dist / duration;

		if (dist < MinThrowDistanceFromClosedFist)
			return;
		State = UpdateResting;
		Debug.Log ("Throw: ClosedFist = "+hand.Detectors.ClosedFist.IsActive+", speed = " + speed + " (min required : "+MinVelocityToThrow+")");
		if (/*!hand.Detectors.ClosedFist.IsActive && */speed >= MinVelocityToThrow) 
		{
			//Debug.Log ("Throw: dist = "+dist+", speed = "+speed+", duration = "+duration+"");
			Vector3 pos = hand.LeapHand.PalmPosition.ToVector3 ();
			Vector3 throwDirection = (pos - FistClosedPos).normalized;
			Force = throwDirection * speed;
			IsBeingPerformed = true;
		}
	}

	#if UTILISER_LE_JOLI_CODE_DU_CHEF
	public float MaxBeginDistanceFromUser = 0.1f;
	public float MinVelocityToThrow = 1.0f;
	public ThrowingState state;
	public Vector3 Force;

	private Vector3 Direction;
	private float Velocity;

	private Vector3 FirstStepPosition;
	private float FirstStepTime;

	public void Update(IVIHand hand) {
		if (!hand.IsDetected || IsBeingPerformed) 
		{
			Force = new Vector3 (0, 0, 0);
			FirstStepPosition = new Vector3 (0, 0, 0);
			FirstStepTime = 0f;
			IsBeingPerformed = false;
			state = ThrowingState.Undefined;
			return;
		}


		if (hand.Detectors.ClosedFist.IsActive && 
			Vector3.Distance(hand.LeapHand.PalmPosition.ToVector3(), IVISession.User.transform.position) <= MaxBeginDistanceFromUser) 
		{
			FirstStepPosition = hand.LeapHand.PalmPosition.ToVector3 ();
			FirstStepTime = Time.time;
			state = ThrowingState.FirstStep;
			return;
		}
		if (state == ThrowingState.FirstStep && 
			!hand.Detectors.ClosedFist.IsActive &&
			(Vector3.Distance (FirstStepPosition, hand.LeapHand.PalmPosition.ToVector3 ()) / (Time.time - FirstStepTime)) >= MinVelocityToThrow) 
		{
			Direction = (hand.LeapHand.PalmPosition.ToVector3 () - FirstStepPosition).normalized;
			Velocity = Vector3.Distance (FirstStepPosition, hand.LeapHand.PalmPosition.ToVector3 ()) / (Time.time - FirstStepTime);
			Force = Velocity * Direction;
			state = ThrowingState.ThrowStep;
			IsBeingPerformed = true;
		}
	}
	#endif
}

public class IVIGestureReleasedPinch : IVISingleHandGesture {

	public bool IsBeingPerformed { get; protected set; }
	private bool isPinching = false;
	private bool wasFistClosed = false;
	public void Update(IVIHand hand) {
		if (hand.Detectors.ClosedFist.IsActive)
			wasFistClosed = true;

		if (isPinching && !hand.Detectors.Pinch.IsPinching) {
			isPinching = false;
			IsBeingPerformed = !wasFistClosed;
			wasFistClosed = false;
			return;
		}
		if (!isPinching && hand.Detectors.Pinch.IsPinching)
			isPinching = true;
		IsBeingPerformed = false;
	}
}