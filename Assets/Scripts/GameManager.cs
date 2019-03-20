using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour {
    public Avatar avatarScript;
    public ShadowCharacter shadowCharacterScript;

    public GameObject mainCamera;
    public GameObject platformingCamera;

    public GameObject[] realWorldObjects;

	void Awake () {
        avatarScript.enabled = true;
        shadowCharacterScript.enabled = false;

        realWorldObjects = GetRelevantRealObjects();

        //The real world objects and player are not rendered while in platforming mode
        /*platformingCamera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("RealWorldObjects"));
        platformingCamera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("RealWorldPlayer"));*/
    }
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            avatarScript.enabled = !avatarScript.enabled;
            shadowCharacterScript.enabled = !shadowCharacterScript.enabled;

            mainCamera.SetActive(!mainCamera.activeInHierarchy);
            platformingCamera.SetActive(!platformingCamera.activeInHierarchy);

            if (platformingCamera.activeInHierarchy) {
                foreach (GameObject GO in realWorldObjects) {
                    if (GO.tag != "Player") {
                        GO.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                    }
                    else {
                        GO.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
                    }
                }
            }
            else {
                foreach (GameObject GO in realWorldObjects) {
                    if (GO.tag != "Player") {
                        GO.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.On;
                    }
                    else {
                        GO.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
                    }
                }
            }
        }
	}

    //Go through all the GameObjects from the scene and return only those that will get projected to the wall
    GameObject[] GetRelevantRealObjects() {
        GameObject[] allObjectsFromScene = GameObject.FindObjectsOfType<GameObject>();
        List<GameObject> objectsToProject = new List<GameObject>();

        foreach (GameObject go in allObjectsFromScene) {
            //The layer RealWorld is the 9th layer
            if (go.layer == 9) {
                objectsToProject.Add(go);
            }
        }

        objectsToProject.Add(GameObject.FindGameObjectWithTag("Player"));

        return objectsToProject.ToArray();
    }
}