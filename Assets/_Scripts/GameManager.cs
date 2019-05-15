using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public Avatar avatarScript;
    public ShadeAnimated shadowCharacterScript;

    public GameObject mainCamera;
    public GameObject platformingCamera;

    public GameObject[] renderedObjects;
    GameObject[] projectedObjects;
    ShadowProjection[] sourcesOfLight;

    public GameObject instructionCanvas;
    public Animator instructionAnim;

    public AudioSource elliotTheme;

    GameObject player;

    public string nextLevel;

	void Awake () {
        avatarScript.enabled = true;
        shadowCharacterScript.enabled = false;

        renderedObjects = GetRelevantRenderedObjects();
        projectedObjects = GetRelevantRealObjects();
        sourcesOfLight = FindObjectsOfType<ShadowProjection>();

        //The real world objects and player are not rendered while in platforming mode
        /*platformingCamera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("RealWorldObjects"));
        platformingCamera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("RealWorldPlayer"));*/

        for (int i = 0; i < projectedObjects.Length; i++) {
            projectedObjects[i].transform.hasChanged = false;
        }

        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        instructionAnim = instructionCanvas.GetComponent<Animator>();
        instructionAnim.SetBool("shadeTurn", false);
    }

    void Update () {
        //Update the shadows of all objects from all sources of light when they're moving
        /*for (int i = 0; i < projectedObjects.Length; i++) {
            if (projectedObjects[i].transform.hasChanged) {
                for (int j = 0; j < sourcesOfLight.Length; j++) {
                    sourcesOfLight[j].UpdateShadow(projectedObjects[i]);
                }
                projectedObjects[i].transform.hasChanged = false;
            }
        }*/

        //Switch between 3D and 2D worlds
        if (Input.GetButtonDown("Switch")) {
            avatarScript.enabled = !avatarScript.enabled;
            shadowCharacterScript.enabled = !shadowCharacterScript.enabled;

            mainCamera.SetActive(!mainCamera.activeInHierarchy);
            platformingCamera.SetActive(!platformingCamera.activeInHierarchy);

            if (instructionAnim.GetBool("shadeTurn"))
            {
                instructionAnim.SetBool("shadeTurn", false);
            }
            else instructionAnim.SetBool("shadeTurn", true);




            if (platformingCamera.activeInHierarchy) {
                for (int i = 0; i < projectedObjects.Length; i++) {
                    for (int j = 0; j < sourcesOfLight.Length; j++) {
                        sourcesOfLight[j].UpdateShadow(projectedObjects[i]);
                    }
                }

                foreach (GameObject GO in renderedObjects) {
                    if (GO.layer != 12) {
                        GO.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                    }
                    else {
                        GO.SetActive(false);
                    }
                }

                player.SetActive(false);
            }
            else {
                foreach (GameObject GO in renderedObjects) {
                    if (GO.layer != 12) {
                        GO.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.On;
                    }
                    else {
                        GO.SetActive(true);
                    }
                }

                player.SetActive(true);
            }
        }
	}

    //Go through all the GameObjects from the scene and return only those that will get projected to the wall
    GameObject[] GetRelevantRenderedObjects() {
        MeshRenderer[] allRenderersFromTheScene = FindObjectsOfType<MeshRenderer>();
        List<GameObject> objectsToProject = new List<GameObject>();

        foreach (MeshRenderer rend in allRenderersFromTheScene) {
            //The layer Wall is the 10th layer
            if (rend.gameObject.layer != 10 && rend.gameObject.layer != 11 && rend.gameObject.layer != 12 && rend.gameObject.layer != 13) {
                objectsToProject.Add(rend.gameObject);
            }
        }

        //objectsToProject.Add(GameObject.FindGameObjectWithTag("Player"));

        return objectsToProject.ToArray();
    }

    GameObject[] GetRelevantRealObjects() {
        GameObject[] allObjectsFromScene = GameObject.FindObjectsOfType<GameObject>();
        List<GameObject> objectsToProject = new List<GameObject>();

        foreach (GameObject go in allObjectsFromScene) {
            //The layer RealWorld is the 9th layer
            if (go.layer == 9) {
                objectsToProject.Add(go);
            }
        }

        return objectsToProject.ToArray();
    }

    public void NextLevel() {
        shadowCharacterScript.enabled = false;
        avatarScript.enabled = false;
        SceneManager.LoadScene(nextLevel);
    }
}