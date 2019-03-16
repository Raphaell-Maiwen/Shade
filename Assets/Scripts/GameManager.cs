using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public Avatar avatarScript;
    public ShadowCharacter shadowCharacterScript;

    public GameObject mainCamera;
    public GameObject platformingCamera;

	void Awake () {
        avatarScript.enabled = true;
        shadowCharacterScript.enabled = false;

        //The real world objects and player are not rendered while in platforming mode
        platformingCamera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("RealWorldObjects"));
        platformingCamera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("RealWorldPlayer"));
    }
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            avatarScript.enabled = !avatarScript.enabled;
            shadowCharacterScript.enabled = !shadowCharacterScript.enabled;

            mainCamera.SetActive(!mainCamera.activeInHierarchy);
            platformingCamera.SetActive(!platformingCamera.activeInHierarchy);
        }
	}
}