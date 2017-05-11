using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

abstract public class IVIMode {
	public string name;
	public IVIModelMode modelIndex;
	public List<IVITool> Tools = new List<IVITool>();
	public IVITool CurrentTool;
}
