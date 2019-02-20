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