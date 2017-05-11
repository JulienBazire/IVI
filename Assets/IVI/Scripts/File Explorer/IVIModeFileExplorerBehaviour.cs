using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IVIModeFileExplorerBehaviour : MonoBehaviour {
	
	public string MyPath;

	public Transform StarPrefab;

	public int DataDepth = 2;
	public int GraphicDepth = 1;

	public float DistanceFromUser = 2;
	public float DistanceBetweenOrbites = 1;
	public int OrbitesCount = 5;
	public int OrbiteHeightRange = 10;

	public float RevolutionSpeed = 200;
	public bool IsRevolving = false;

	// Use this for initialization
	void Start () {
		if (MyPath.Length <= 0)
			MyPath = Application.dataPath;
		
		InstantiateStarSystem (transform, new IVIDirectory (MyPath, DataDepth), GraphicDepth);
		PlaceStarOnOrbite(transform);
	}

	void Update(){
		if (IsRevolving) {
			StarRevolution (transform);
		}
	}

	private void InstantiateStarSystem(Transform parent, IVIDirectory directory, int depth){
		foreach (IVIEntry element in directory.Entries) {
			Transform currentChild = Instantiate(StarPrefab, parent.position, Quaternion.identity, parent);
			currentChild.gameObject.GetComponent<IVIStar> ().Entry = element;
			if (element is IVIDirectory) {
				IVIDirectory childInfo = element as IVIDirectory;
				currentChild.gameObject.name = childInfo.Systemdirectory.Name;
				if (depth > 0) {
					InstantiateStarSystem (currentChild, childInfo, depth-1);
				}
			} else {
				IVIFile childInfo = element as IVIFile;
				currentChild.gameObject.name = childInfo.Systemfile.Name;
			}
		}
	}

	private void PlaceStarOnOrbite(Transform parentStar){
		int CountStarsOnOrbite = ( parentStar.GetComponentsInChildren<IVIStar>().Length / OrbitesCount) + 1;
		int nbStarOnOrbite = 0, nbOrbite = 1;

		foreach (Transform child in parentStar){
			if (child.GetComponent<IVIStar> () != null) {
				if (nbStarOnOrbite == CountStarsOnOrbite) {
					nbOrbite++;
					nbStarOnOrbite = 0;
				}

				Vector3 vec = new Vector3 ();
				float Radius = parentStar.localScale.x + DistanceFromUser + (nbOrbite * DistanceBetweenOrbites);
				float angle = (2 * Mathf.PI / CountStarsOnOrbite) * nbStarOnOrbite;
				vec.x = Radius * Mathf.Cos (angle); 
				vec.y = Random.Range (-OrbiteHeightRange/2.0f, OrbiteHeightRange/2.0f);
				vec.z = Radius * Mathf.Sin (angle); 
				child.transform.Translate (vec);

				if (child.GetComponentsInChildren<IVIStar> ().Length > 0) {
					PlaceStarOnOrbite (child);
				}
				nbStarOnOrbite++;
			}
		}
	}

	/*
	private void PlaceStarOnOrbite(Transform directory, int depth){
		
		int elementOnOrbite = (directory.childCount / nbrOrbites) + 1;
		int nbStarOnOrbite = 0, orbite = 0;
		float total = 1, maxSize = 1;

		foreach (Transform child in directory) {
			int nbChild = child.gameObject.GetComponentsInChildren<IVIStar> ().Length;
			if (maxSize < nbChild)
				maxSize = nbChild;
			total += nbChild + 1;
		}

		float baseScale = directory.localScale.x / (maxSize+2);
		float distanceBetweenOrbites = 2 * (directory.localScale.x - baseScale);
		float distanceFromParent = distanceFromUser * distanceBetweenOrbites;

		foreach (Transform child in directory){
			if (nbStarOnOrbite == elementOnOrbite) {
				orbite++;
				nbStarOnOrbite = 0;
			}
			Vector3 vec = new Vector3 ();
			float Radius = distanceFromParent + (orbite * distanceBetweenOrbites);
			float angle = (2 * Mathf.PI / elementOnOrbite) * nbStarOnOrbite;
			vec.x = Radius * Mathf.Cos (angle); 
			vec.y = Random.Range (-heightElements, heightElements);
			vec.z = Radius * Mathf.Sin (angle); 
			child.transform.Translate (vec);

			child.localScale = new Vector3(baseScale , baseScale, baseScale) * (child.GetComponentsInChildren<IVIStar>().Length+1);

			if (child.GetComponentsInChildren<IVIStar> ().Length > 0 && (depth-1) > 0) {
				PlaceStarOnOrbite (child, depth-1);
			}

			nbStarOnOrbite++;
		}
	}
*/

	//TODO Problemen d'addition de rotation avec les rotations sur elle meme de l'étoile
	public void StarRevolution(Transform anchor){
		foreach (Transform child in anchor) {
			var distance = Vector3.Distance (anchor.position, child.transform.position);
			child.RotateAround (anchor.position, new Vector3 (0, 1, 0), (RevolutionSpeed * Time.deltaTime)/distance); 
			if (child.childCount > 0) {
				StarRevolution (child);
			}
		}
	
	}
}
