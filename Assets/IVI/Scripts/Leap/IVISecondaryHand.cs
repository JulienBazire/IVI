using UnityEngine;

public class IVISecondaryHand : IVIHand
{
	public IVIGestureClosedFist gestureClosedFist = new IVIGestureClosedFist();
	public IVIGestureContactWithOtherPalm gestureContactWithOtherPalm = new IVIGestureContactWithOtherPalm();
	public IVIGesturePalmFacingCamera gesturePalmFacingCamera = new IVIGesturePalmFacingCamera();
	public IVIGestureThrowingForward gestureThrowingForward = new IVIGestureThrowingForward();

	public override void UpdateSingleHandGestures() {
		gestureClosedFist.Update(this);
		gesturePalmFacingCamera.Update(this);
		gestureThrowingForward.Update(this);
	}
	public override void UpdateHandPairGestures() {
		gestureContactWithOtherPalm.Update(this, IVISession.PrimaryHand);
	}

	private Color initial_color = Color.magenta;
	private bool has_pushed_color = false;
	public void PushColorOnce(Color c) {
		if (has_pushed_color)
			return;
		has_pushed_color = true;
		initial_color = Renderer.material.color;
		Renderer.material.color = c;
	}
	public void PopColorOnce() {
		if (!has_pushed_color)
			return;
		has_pushed_color = false;
		Renderer.material.color = initial_color;
	}
}
