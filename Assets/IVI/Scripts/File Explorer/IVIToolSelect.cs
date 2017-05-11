public class IVIToolSelect : IVITool {
	public IVIToolSelect(){
		name = IVILocalization.GetText_Select ();
	}

	public override void Use(){
		IVIUser user = IVISession.User;
		IVIStar star = user.Focus==null ? null : user.Focus.GetComponent<IVIStar> ();

		if (user.Focus == null || star == null)
			return;

		IVISession.Clipboard.ToggleSelect (star);
	}
}
