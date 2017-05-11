using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IVITaskManager {

	public List<IVIMode> Modes = new List<IVIMode>();
	public IVIMode CurrentMode;

	public IVITaskManager(){

		Modes.Add(new IVIModeFileExplorer());

		CurrentMode = Modes[0];
	}
}
