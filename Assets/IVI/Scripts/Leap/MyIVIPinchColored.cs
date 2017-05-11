// A script which colors an object depending on whether a pinch gesture is detected or not.

using UnityEngine;

public class MyIVIPinchColored : MonoBehaviour
{
	public Color ColorWhenPinched, ColorWhenNotPinched;
	protected Leap.Unity.PinchDetector PinchDetector;

	void Start() {
		PinchDetector = FindObjectOfType<Leap.Unity.PinchDetector> ();
	}

	void Update () {
		Material material = GetComponent<Renderer>().material;
		material.color = PinchDetector.IsPinching ? ColorWhenPinched : ColorWhenNotPinched;
	}
}
