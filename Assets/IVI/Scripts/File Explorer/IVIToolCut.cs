public class IVIToolCut : IVITool {

	public IVIToolCut(){
		name = IVILocalization.GetText_Cut ();
	}
	public override void Use(){
		IVIUser user = IVISession.User;

		if(user.Focus != null && user.Focus.GetComponent<IVIStar>() != null) {
			IVIStar target = user.Focus.GetComponent<IVIStar>();
			if (target.Entry is IVIDirectory) {
				IVISession.Clipboard.MoveSelectionInto(target);
				// TODO Repasser en mode sélection. Il faut un lien vers le IVIToolBehaviour.
			}
		}
	}
}
