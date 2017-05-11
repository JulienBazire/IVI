// http://stackoverflow.com/a/25442987

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IVIFatalMessageBox : MonoBehaviour {
	private bool show = false;
	public string Title = "Fatal Error";
	public string Message = "IVI has encountered a fatal error and must quit.";
	public uint Width = 300;
	public uint Height = 200;
	public float LifetimeInSeconds = 5f;
	private float starting_time = 0f;

	private Rect windowRect;

	Rect computeRect() {
		return new Rect ((Screen.width - Width)/2, (Screen.height - Height)/2, Width, Height);
	}

	void Start() {
		starting_time = Time.time;
	}
	void Update() {
		if(starting_time + LifetimeInSeconds < Time.time) {
			//UnityEditor.EditorApplication.isPlaying = false;
			Application.Quit();
		}
	}

	void OnGUI () 
	{
		if(show)
			windowRect = GUI.Window (0, computeRect(), DialogWindow, Title);
	}

	void DialogWindow (int windowID)
	{
		float y = 20;
		GUI.Label(new Rect(5,y, windowRect.width, 20), Message);
		/*
		if(GUI.Button(new Rect(5,y, windowRect.width - 10, 20), "Restart"))
		{
			Application.LoadLevel (0);
			Show = false;
		}

		if(GUI.Button(new Rect(5,y, windowRect.width - 10, 20), "Exit"))
		{
			Application.Quit();
			show = false;
		}
		*/
	}
}
