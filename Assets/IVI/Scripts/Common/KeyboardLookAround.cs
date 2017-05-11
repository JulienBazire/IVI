using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardLookAround : MonoBehaviour {

	public GameObject RotateMe;
	public Vector2 AngularSpeed;
	private Vector2 angular_offset = Vector2.zero;
	public const KeyCode ResetKeyCode = KeyCode.Keypad0;

	void Update () {
		if (Input.GetKey (ResetKeyCode)) {
			RotateMe.transform.Rotate (-new Vector3(angular_offset.y, angular_offset.x));
			angular_offset = Vector2.zero;
		}

		Vector2 new_angular_offset = Vector2.zero;

		if (Input.GetKey (KeyCode.DownArrow)) {
			new_angular_offset.y -= AngularSpeed.y;
		}
		if (Input.GetKey (KeyCode.UpArrow)) {
			new_angular_offset.y += AngularSpeed.y;
		}
		if (Input.GetKey (KeyCode.LeftArrow)) {
			new_angular_offset.x -= AngularSpeed.x;
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			new_angular_offset.x += AngularSpeed.x;
		}

		angular_offset += new_angular_offset;
		RotateMe.transform.Rotate (new Vector3(new_angular_offset.y, new_angular_offset.x));
	}
}
