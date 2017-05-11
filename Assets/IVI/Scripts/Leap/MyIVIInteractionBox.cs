// A script which binds a GameObjects's transform to Leap's InteractionBox's.
// https://di4564baj7skl.cloudfront.net/documentation/images/Leap_InteractionBox.png

using UnityEngine;
using Leap.Unity;

public class MyIVIInteractionBox : MonoBehaviour
{
	void Update() {
		Leap.Frame leapFrame = IVISession.LeapProvider.CurrentFrame;
		Leap.InteractionBox interactionBox = leapFrame.InteractionBox;
		Transform transform = GetComponent<Transform>();
		transform.rotation = new Quaternion();
		transform.position = interactionBox.Center.ToVector3()/1000;
		transform.localScale = interactionBox.Size.ToVector3()/1000;
	}
}

