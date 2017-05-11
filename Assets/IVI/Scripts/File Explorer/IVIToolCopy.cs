using System;
using UnityEngine;

public class IVIToolCopy : IVITool {
	public IVIToolCopy(){
		name = IVILocalization.GetText_Copy ();
	}
	public override void Use(){
		IVIUser user = IVISession.User;

		if(user.Focus != null && user.Focus.GetComponent<IVIStar>() != null) {
			IVIStar target = user.Focus.GetComponent<IVIStar>();
			if (target.Entry is IVIDirectory) {
				IVISession.Clipboard.CopySelectionInto(target);
			}
		}
			
	}
}

