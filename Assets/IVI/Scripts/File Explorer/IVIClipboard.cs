using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IVIClipboard {
/*

	public List<IVIEntry> Entries = new List<IVIEntry>();
	public IVIDirectory BinDir = null;
	public IVIStarSystem StarSystemToNotify = null;

	public void Select(IVIEntry element) {
		Entries.Add (element);
		// TODO Prevent adding duplicates
		Debug.Log ("Added " + element.FullPath + " to clipboard");
	}

	public void MoveToBin(){
		if (BinDir == null) {
			string path = Path.Combine (Application.dataPath, "IVIBin");
			BinDir = new IVIDirectory (path, 0);
			Debug.Log("Bin path : " + BinDir.FullPath);
		}
		foreach (IVIEntry element in Entries)
			element.CutTo(BinDir);
		DeselectAll ();
	}

	public void DeselectAll(){
		Entries.Clear ();
	}

	public void CutPasteSelection(IVIDirectory target){
		try {
			foreach (IVIEntry element in Entries){
				element.CutTo(target);
				StarSystemToNotify.NotifyEntryCut(element, target);
			}
		} catch (Exception e){
			Console.WriteLine ("Failed to cut selection : " + e.ToString ());
		}
	}
	public void CopyTo(IVIDirectory target){
		try{
			foreach (IVIEntry element in Entries){
				element.CopyTo(target);
				StarSystemToNotify.NotifyEntryCopied(element, target);
			}
		}catch(Exception e){
			Console.WriteLine ("Failed to copy selection : " + e.ToString ());
		}
	}
	*/
}
