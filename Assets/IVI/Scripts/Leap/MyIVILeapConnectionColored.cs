// A script which colors an object depending on the Leap Motion's connection status.
// To have better functionality, we should use Leap's DisconnectionNotice class.

using UnityEngine;

public class MyIVILeapConnectionColored : MonoBehaviour
{
	public Color ColorWhenConnected, ColorWhenDisconnected;

	void Update () {
		Material material = GetComponent<Renderer>().material;
		material.color = IVISession.LeapProvider.IsConnected () ? ColorWhenConnected : ColorWhenDisconnected;
	}
}

