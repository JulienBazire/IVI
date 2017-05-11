using System.Collections.Generic;
using UnityEngine;

public class IVIPrimaryHand : IVIHand
{
	public IVIGestureExtendedIndexRelaxed gestureExtendedIndexRelaxed = new IVIGestureExtendedIndexRelaxed();
	public IVIGestureBothHandsSweeping gestureBothHandsSweeping = new IVIGestureBothHandsSweeping();
	public IVIGestureClosedFist gestureClosedFist = new IVIGestureClosedFist();
	public IVIGestureReleasedPinch gestureReleasedPinch = new IVIGestureReleasedPinch();

	readonly Color COLOR_WHEN_USING_TOOL = Color.green;
	private Stack<Color> colorStack = new Stack<Color>();

	public override void UpdateSingleHandGestures() {
		gestureExtendedIndexRelaxed.Update (this);
		gestureClosedFist.Update(this);
		gestureReleasedPinch.Update (this);
	}
	public override void UpdateHandPairGestures() {
		gestureBothHandsSweeping.Update (this, IVISession.SecondaryHand);
	}
	public void OnSuccessfulToolGestureBegin() {
		PushColor (COLOR_WHEN_USING_TOOL);
	}
	public void OnFailedToolGestureBegin() {
		PushColor (Renderer.material.color);
	}
	public void OnToolGestureEnd() {
		PopColor ();
	}
	public void PushColor(Color c) {
		colorStack.Push (Renderer.material.color);
		Renderer.material.color = c;
	}
	public void PopColor() {
		Renderer.material.color = colorStack.Pop ();
	}
}
