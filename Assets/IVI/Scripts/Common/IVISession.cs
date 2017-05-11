using UnityEngine;
using UnityEngine.Assertions;
using Leap.Unity;

public class IVISession : MonoBehaviour
{
	public static IVISession Instance = null;

	public static IVITaskManager TaskManager = new IVITaskManager();
	public static IVIClipboardBehaviour Clipboard = null;
	public IVIUser User_;
	public static IVIUser User { get { return Instance.User_; } }
	public IVIMascotBehaviour Mascot_;
	public static IVIMascotBehaviour Mascot { get { return Instance.Mascot_; } }
	public static IVIPrimaryHand   PrimaryHand   = new IVIPrimaryHand();
	public static IVISecondaryHand SecondaryHand = new IVISecondaryHand();

	// These are not static, because they must be set from the editor.
	public LeapServiceProvider LeapProvider_;
	public Leap.Unity.IHandModel LeapHandModelLeft, LeapHandModelRight, LeapHandGraphicsModelLeft, LeapHandGraphicsModelRight;
	public IVIGesturesDetectors GesturesDetectorsSettings;
	public bool DisplayHiddenFiles_ = false;

	public static LeapServiceProvider LeapProvider { get { return Instance.LeapProvider_; } }
	public static bool DisplayHiddenFiles { get { return Instance.DisplayHiddenFiles_; } }

	public IVIAudioController AudioController_;
	public static IVIAudioController AudioController { get { return Instance.AudioController_; } } 

	// https://unity3d.com/learn/tutorials/projects/2d-roguelike-tutorial/writing-game-manager
	void Awake() {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);    
        DontDestroyOnLoad(gameObject);
	}

	void Start() {
		PrimaryHand  .Detectors = Instantiate (GesturesDetectorsSettings, transform);
		SecondaryHand.Detectors = Instantiate (GesturesDetectorsSettings, transform);
		PrimaryHand  .Detectors.name = "PrimaryHandDetectors";
		SecondaryHand.Detectors.name = "SecondaryHandDetectors";
		OnHandednessChanged ();

		FindObjectOfType<IVIToolBehaviour>().InitPointer (PrimaryHand.LeapHandModel.transform.Find("Index"));
		Clipboard = FindObjectOfType<IVIClipboardBehaviour> ().GetComponent<IVIClipboardBehaviour>();
	}

	public static void OnHandednessChanged() {
		PrimaryHand  .LeapHandModel = User.IsRightHanded ? Instance.LeapHandModelRight : Instance.LeapHandModelLeft;
		SecondaryHand.LeapHandModel = User.IsRightHanded ? Instance.LeapHandModelLeft : Instance.LeapHandModelRight;
		PrimaryHand  .LeapHandGraphicsModel = User.IsRightHanded ? Instance.LeapHandGraphicsModelRight : Instance.LeapHandGraphicsModelLeft;
		SecondaryHand.LeapHandGraphicsModel = User.IsRightHanded ? Instance.LeapHandGraphicsModelLeft : Instance.LeapHandGraphicsModelRight;
	}

	void Update() {
		Leap.Frame leapframe = LeapProvider.CurrentFrame;
		DetectBothHands(leapframe);
		PrimaryHand.UpdateSingleHandGestures();
		SecondaryHand.UpdateSingleHandGestures();
		PrimaryHand.UpdateHandPairGestures();
		SecondaryHand.UpdateHandPairGestures();
	}

	void DetectBothHands(Leap.Frame leapframe) {
		if (!LeapProvider.IsConnected()) {
			PrimaryHand.Undetect ();
			SecondaryHand.Undetect ();
			return;
		}

		int primaryhandcount=0, secondaryhandcount=0;
		foreach(Leap.Hand hand in leapframe.Hands) {
			if ((hand.IsRight && User.IsRightHanded) || (hand.IsLeft && User.IsLeftHanded)) {
				++primaryhandcount;
				PrimaryHand.Detect(hand);
			} else if ((hand.IsRight && User.IsLeftHanded) || (hand.IsLeft && User.IsRightHanded)) {
				++secondaryhandcount;
				SecondaryHand.Detect(hand);
			}
		}
		if (primaryhandcount > 1)
			Debug.LogWarning(primaryhandcount + " primary hands are detected.");
		if (secondaryhandcount > 1)
			Debug.LogWarning(secondaryhandcount + " secondary hands are detected.");
		if (primaryhandcount   == 0) PrimaryHand  .Undetect();
		if (secondaryhandcount == 0) SecondaryHand.Undetect();
	}
}