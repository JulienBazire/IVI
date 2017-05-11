using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleOpacityFadeIn : MonoBehaviour {

	public float FadeTime = 1f; // In seconds
	[HideInInspector]
	public float Progress {
		get { return Mathf.Clamp01((Time.time - start_time) / FadeTime); }
	}

	private float start_time;

	public Material Material {
		get { return GetComponent<Renderer>().material; }
	}

	void Start() {
		start_time = Time.time;
	}

	void Update () {
		Color c = Material.color;
		c.a = 1f - Progress;
		Material.color = c;
	}
}
