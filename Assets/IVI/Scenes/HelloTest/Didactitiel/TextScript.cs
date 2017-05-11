using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;

public class TextScript : MonoBehaviour {

	public Text txt;
	public float rate;

	public string phrase;
	private List<string> phrases = new List<string>();

	// Use this for initialization
	void Start () {

		GameObject go = txt.gameObject;
		LastTime = Time.time;

		try
		{   // Open the text file using a stream reader.
			using (StreamReader sr = new StreamReader("Assets/IVI/Scenes/HelloTest/Didactitiel/TextDidactitiel.txt"))
			{
				// Read the stream to a string, and write the string to the console.
				String line= sr.ReadLine();
				while(line != null){
					phrases.Add(line);
					line= sr.ReadLine();
				}
			}
		}
		catch (Exception e)
		{
			Debug.Log("The file could not be read:");
			Debug.Log(e.Message);
		}
	}


	IEnumerator ChargeIvi(){
		float fadeTime = GameObject.Find("_GO").GetComponent<Fading>().BeginFade(1);
		GameObject.Find("_GO").GetComponent<Fading>().BeginFade(1);
		SceneManager.LoadScene ("main", LoadSceneMode.Single);
		//SceneManager.LoadScene(2);
		yield return new WaitForSeconds (fadeTime);
	}
		
	private float LastTime;
	private int i = 0;
	private int id = 0;
	private bool wasGestureBeingPerformed = false, iviSpeak=true;

	// Update is called once per frame
	void Update () {

		float CurTime = Time.time;
		float TimeSpent = CurTime - LastTime;
		float usedRate = rate + UnityEngine.Random.Range (-.08f, .03f);

		if (id >= phrases.Count) {
			StartCoroutine(ChargeIvi ());
		}

		if (TimeSpent > usedRate && id < phrases.Count && i < phrases [id].Length ) {
			if (phrases [id] [0] == '#') {
				iviSpeak = false;

				switch (phrases [id] [1]) {
				case '1':
					Debug.Log ("ClosedFist");
					if (wasGestureBeingPerformed && !IVISession.PrimaryHand.gestureClosedFist.IsBeingPerformed) {
						wasGestureBeingPerformed = false;
						iviSpeak = false;
					}
					wasGestureBeingPerformed = IVISession.PrimaryHand.gestureClosedFist.IsBeingPerformed;
					break;
				case '2':
					Debug.Log ("HandSweeping");
					if (wasGestureBeingPerformed && !IVISession.PrimaryHand.gestureBothHandsSweeping.IsBeingPerformed) {
						wasGestureBeingPerformed = false;
						iviSpeak = false;
					}
					wasGestureBeingPerformed = IVISession.PrimaryHand.gestureBothHandsSweeping.IsBeingPerformed;
					break;
				case '3':
					Debug.Log ("ExtendedIndexRelaxed");
					if (wasGestureBeingPerformed && !IVISession.PrimaryHand.gestureExtendedIndexRelaxed.IsBeingPerformed)
						iviSpeak = false;
					wasGestureBeingPerformed = IVISession.PrimaryHand.gestureExtendedIndexRelaxed.IsBeingPerformed;
					break;
				case '4':
					Debug.Log ("Pinch");
					if (wasGestureBeingPerformed && !IVISession.PrimaryHand.gestureReleasedPinch.IsBeingPerformed)
						iviSpeak = false;
					wasGestureBeingPerformed = IVISession.PrimaryHand.gestureReleasedPinch.IsBeingPerformed;
					break;
				case '5':
					Debug.Log ("LookAt");
					if (wasGestureBeingPerformed && !IVISession.PrimaryHand.gestureReleasedPinch.IsBeingPerformed)
						iviSpeak = false;
					wasGestureBeingPerformed = IVISession.PrimaryHand.gestureReleasedPinch.IsBeingPerformed;
					break;
				case '6':
					Debug.Log ("Throw");
					if (wasGestureBeingPerformed && !IVISession.SecondaryHand.gestureThrowingForward.IsBeingPerformed)
						iviSpeak = false;
					wasGestureBeingPerformed = IVISession.SecondaryHand.gestureThrowingForward.IsBeingPerformed;
					break;
				default:
					Debug.Log ("Bonjour, je crois qu'il y a un probleme les gars");
					if (wasGestureBeingPerformed && !IVISession.PrimaryHand.gestureClosedFist.IsBeingPerformed)
						iviSpeak = false;
					wasGestureBeingPerformed = IVISession.PrimaryHand.gestureClosedFist.IsBeingPerformed;
					break; 
				}
				if (id < phrases.Count) {
					id++;
				}
			} else if (iviSpeak){
				txt.text += phrases [id] [i];
				LastTime = CurTime;
				i++;
			}
		} else if (TimeSpent > 1.3f && iviSpeak){
			if (id < phrases.Count) {
				id++;
			}
			txt.text = "";
			i = 0;
		}
	}
}
