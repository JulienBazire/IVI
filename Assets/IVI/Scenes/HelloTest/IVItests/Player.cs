using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour {

	private Animator anim;
	private CharacterController controller;
	private Light IVILight;
	public GameObject IVICorps;
	public Material InterieurIVI;
	public float speed = 10.0f;
	private Vector3 moveDirection = Vector3.zero;
	public float turnSpeed = 60.0f;
	public float gravity = 1.0f;

	// Use this for initialization
	void Start () {
		anim = gameObject.GetComponentInChildren<Animator> ();
		controller = GetComponent<CharacterController>();
		IVILight = transform.GetComponentInChildren<Light> ();
		var mats = IVICorps.GetComponent<Renderer> ().materials;
		Debug.Log (mats.Length);
		//InterieurIVI = (from m in mats where m.ToString()=="interieur" select m).First();
		InterieurIVI = mats[0];
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKey("i")){
			anim.SetInteger ("rollParam", 1);
		}else{
			anim.SetInteger ("rollParam", 0);
		}

		if (Input.GetKey ("o")) {
			anim.SetInteger ("chocParam", 1);
			IVILight.color = new Color(0.1f, 0.0f, 0.7f, 1);
			InterieurIVI.color = new Color(0.1f, 0.0f, 0.7f, 1);
			InterieurIVI.SetColor ("_EmissionColor", new Color(0.1f, 0.0f, 0.7f, 1));
		} else {
			anim.SetInteger ("chocParam", 0);
			IVILight.color = new Color(1.0f, 0.7f, 0.0f, 1);
			InterieurIVI.color = new Color(1.0f, 0.7f, 0.0f, 1);
			InterieurIVI.SetColor ("_EmissionColor", new Color(1.0f, 0.7f, 0.0f, 1));

		}

		if(Input.GetKey("p")){
			if (anim.GetInteger ("searchParam") == 0) {
				anim.SetInteger ("searchParam", 1);
			}
		}else{
			if (anim.GetInteger ("searchParam") == 1) {
				anim.SetInteger ("searchParam", 0);
			}
		}

		if(Input.GetKey("right")){
			anim.SetInteger ("moveRightParam", 1);
			IVILight.intensity+= 0.02f;

		}else{
			anim.SetInteger ("moveRightParam", 0);
		}

		if(Input.GetKey("left")){
			anim.SetInteger ("moveLeftParam", 1);
			IVILight.intensity-= 0.02f;
		}else{
			anim.SetInteger ("moveLeftParam", 0);
		}

		moveDirection = transform.forward * Input.GetAxis ("Vertical") * speed;

		float turn = Input.GetAxis ("Horizontal");
		transform.Rotate (0, turn * turnSpeed * Time.deltaTime, 0);
		controller.Move(moveDirection * Time.deltaTime);
		moveDirection.y -= gravity * Time.deltaTime; 
	}
}
