using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class IVIDirectory : IVIEntry {

	public string FullPath { get { return Systemdirectory!=null ? Systemdirectory.FullName : "<fake_dir>"; } }
	public List<IVIEntry> Entries = new List<IVIEntry>();
	public DirectoryInfo Systemdirectory;

	private int initial_depth = 0;

	public IVIDirectory(DirectoryInfo di, int depth) {
		initial_depth = depth;
		Systemdirectory = di;
		Assert.IsNotNull (Systemdirectory);
		try{
			if(!Systemdirectory.Exists)
				Systemdirectory.Create();
			else {
				InitChildren(depth);
			}
		}catch(Exception e){
			Debug.Log ("Couldn't create directory `" + Systemdirectory.FullName + "` : " + e.ToString ());
			throw e;
		}
	}
	public IVIDirectory(string path, int depth) : this(new DirectoryInfo(path), depth) {}

	public IVIDirectory(List<IVIEntry> entries){
		Systemdirectory = null;
		Entries = entries;
	}

	public void InitChildren(int depth){
		depth--;
		if (depth >= 0) {
			string[] subdirectories = Directory.GetDirectories (Systemdirectory.FullName);
			string[] files = Directory.GetFiles (Systemdirectory.FullName);

			UnityEngine.Assertions.Assert.IsNotNull(this.Entries);
			this.Entries.Clear ();

			if (subdirectories.Length >= 1) {
				foreach (string element in subdirectories) {
					IVIDirectory subdir = new IVIDirectory (element, depth);
					bool isHidden = (subdir.Systemdirectory.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
					if (!isHidden || IVISession.DisplayHiddenFiles)
						this.Entries.Add (subdir);
				}
			}
			if(files.Length >= 1) {
				foreach(string element in files){
					IVIFile file = new IVIFile (element);
					bool isHidden = (file.Systemfile.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
					if (!isHidden || IVISession.DisplayHiddenFiles)
						this.Entries.Add (file);
				}
			}
		}
	}

	public long GetLength(){
		long size = 0;
		foreach (var element in Entries) {
			if (element is IVIDirectory) {
				IVIDirectory elt = element as IVIDirectory;
				size += elt.GetLength ();
			} else {
				IVIFile elt = element as IVIFile;
				size += elt.Systemfile.Length;
			}
		}
		return size;
	}

	public void CutTo(IVIDirectory target){
		if(!Systemdirectory.Exists){
			throw new FileLoadException ("Unknown directory at : " + Systemdirectory.FullName);
		}
		if(!target.Systemdirectory.Exists){
			throw new FileLoadException ("Unknown directory at : " + target.Systemdirectory.FullName);
		}
		string destinationPath = Path.Combine (target.FullPath, Systemdirectory.Name);
		Debug.Log (FullPath + " ==dircut==> " + destinationPath);
		DirectoryInfo new_dir = DirectoryCopy (FullPath, destinationPath, true);
		Systemdirectory.Delete (true);
		IVIDirectory replace = new IVIDirectory (new_dir, initial_depth);
		Systemdirectory = replace.Systemdirectory;
		Entries = replace.Entries;
		target.Entries.Add (this);
	}

	public IVIEntry CopyTo(IVIDirectory target){
		string destinationPath = Path.Combine (target.FullPath, Systemdirectory.Name);
		Debug.Log (FullPath + " ==dircpy==> " + destinationPath);
		DirectoryInfo new_dir = DirectoryCopy (FullPath, destinationPath, true);
		IVIDirectory cpy = new IVIDirectory (new_dir, initial_depth);
		target.Entries.Add (cpy);
		return cpy;
	}

	private static DirectoryInfo DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs){
		// Get the subdirectories for the specified directory.
		DirectoryInfo dir = new DirectoryInfo(sourceDirName);

		if (!dir.Exists){
			throw new DirectoryNotFoundException(
				"Source directory does not exist or could not be found: "
				+ sourceDirName);
		}

		DirectoryInfo[] dirs = dir.GetDirectories();
		// If the destination directory doesn't exist, create it.
		if (!Directory.Exists(destDirName)){
			Directory.CreateDirectory(destDirName);
		}

		// Get the files in the directory and copy them to the new location.
		FileInfo[] files = dir.GetFiles();
		foreach (FileInfo file in files){
			string temppath = Path.Combine(destDirName, file.Name);
			file.CopyTo(temppath, false);
		}

		// If copying subdirectories, copy them and their contents to new location.
		if (copySubDirs){
			foreach (DirectoryInfo subdir in dirs){
				string temppath = Path.Combine(destDirName, subdir.Name);
				DirectoryCopy(subdir.FullName, temppath, copySubDirs);
			}
		}
		return new DirectoryInfo (destDirName);
	}
}

