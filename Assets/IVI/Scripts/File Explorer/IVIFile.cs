using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IVIFile : IVIEntry {

	public string FullPath { get { return Systemfile.FullName; } }
	public IVIFileType Type;
	public FileInfo Systemfile;

	public IVIFile(string path) : this(new FileInfo(path)) {}

	public IVIFile(FileInfo file){
		Systemfile = file;
		SetType ();
		try{
			if(!Systemfile.Exists){
				Systemfile.Create();
			}
		}catch(Exception e){
			Debug.Log ("The process failed : " + e.ToString ());
		}
	}
	public void SetType(){
		string extension = Systemfile.Extension;

		switch (extension) {
		case ".txt": case ".doc": case ".odt": case ".docx":
			Type = IVIFileType.TEXT;
			break;

		case ".jpg": case ".jpeg": case ".png": case ".pdf": case ".raw": case ".svg": case ".bmp": case ".ppm": case ".bpm":
			Type = IVIFileType.IMAGE;
			break;

		case ".mov": case ".mp4": case ".avi": case ".mkv":
			Type = IVIFileType.VIDEO;
			break;

		case ".mp3": case ".wav": 
			Type = IVIFileType.MUSIC;
			break;

		case ".exe":
			Type = IVIFileType.EXECUTABLE;
			break;

		default:
			Type = IVIFileType.UNKNOWN;
			break;
			
		}
	}

	public void CutTo(IVIDirectory target){
		try {
			string tmppath = Path.Combine (target.FullPath, Systemfile.Name);
			Debug.Log (FullPath + " ==filecut==> " + tmppath);
			Systemfile.MoveTo (tmppath);
			target.Entries.Add(this);
		} catch (Exception e) {
			Debug.Log("Failed to move file \"" + Systemfile.FullName +"\" : " + e.ToString());
		}
	}

	public IVIEntry CopyTo(IVIDirectory target){
		try {
			string tmppath = Path.Combine (target.FullPath, Systemfile.Name);
			Debug.Log (FullPath + " ==filecpy==> " + tmppath);
			IVIFile cpy = new IVIFile(Systemfile.CopyTo (tmppath, true));
			target.Entries.Add(cpy);
			return cpy;
		} catch (Exception e) {
			Debug.Log("Failed to copy file \"" + Systemfile.FullName +"\" : " + e.ToString());
		}
		return null;
	}
}
