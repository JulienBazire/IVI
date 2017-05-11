using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IVIModeFileExplorer : IVIMode
{
	public IVIModeFileExplorer(){
		
		name = IVILocalization.GetText_FileExplorerMode();
		modelIndex = IVIModelMode.FILE_EXPLORER;

		Tools.Add(new IVIToolSelect());
		Tools.Add(new IVIToolCopy());
		Tools.Add(new IVIToolCut());

		CurrentTool = Tools [0];
	}
}

