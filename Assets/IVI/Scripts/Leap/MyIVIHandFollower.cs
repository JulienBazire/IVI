// A script which binds an object's transform to a hand's palm.

using UnityEngine;
using Leap.Unity;

public class MyIVIHandFollower : MonoBehaviour {

	[Tooltip("Should it follow the primary hand ? Otherwise, it's the secondary hand.")]
	public bool FollowsPrimaryHand;
	[HideInInspector]
	public bool FollowsSecondaryHand { get { return !FollowsPrimaryHand; } set { FollowsPrimaryHand = !value; } }

	protected Vector3 initialPos;

	void Start() {
		initialPos = transform.position;
	}

	void Update ()
	{
		if(FollowsPrimaryHand)
			FollowHand(IVISession.PrimaryHand);
		else
			FollowHand(IVISession.SecondaryHand);
	}

	void FollowHand(IVIHand ivihand)
	{
		Leap.Hand hand = ivihand.LeapHand;
		Vector3 palmPos  = hand.PalmPosition.ToVector3();
		Vector3 palmNorm = hand.PalmNormal.ToVector3();
		float   palmRot  = .02f + transform.localScale.y / 2;
		transform.position = initialPos + palmNorm * palmRot + palmPos;
		transform.rotation = hand.Basis.rotation.ToQuaternion();
	}

}
