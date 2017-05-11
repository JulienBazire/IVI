using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IVIAudioController : MonoBehaviour {

	public Dictionary<string, AudioSource> Playlist = new Dictionary<string, AudioSource>();


	// Use this for initialization
	void Start () {
		foreach (AudioSource audio in gameObject.GetComponents<AudioSource>())
			Playlist.Add(audio.clip.name, audio);
	}

	public void PlayAudioEvent(string key){
		AudioSource src = Playlist[key];
		src.Play ();
	}
}
