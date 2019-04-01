using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour{
    float spotAngle;
    float range;

    void Awake(){
        spotAngle = GetComponent<Light>().spotAngle / 2;
        range = GetComponent<Light>().range;
    }

    void Update(){
        //For testing purposes; ClearPath shouldn't be called every update in the last version
        ClearPath();
    }

    //Called every time this light or an object (non-character) from the scene moves
    void ClearPath() {
        RaycastHit hit;

        float wallDistance = 0f;
        float spotLightWidth = 0f;

        int layerMask = LayerMask.GetMask("Wall");

        if (Physics.Raycast(transform.position, Vector3.forward, out hit, 50, layerMask)) {
            Debug.DrawRay(transform.position, Vector3.forward * hit.distance, Color.cyan);
            wallDistance = hit.distance;
            
            //print(hit.distance);
        }

        print(spotAngle);

        spotLightWidth = Mathf.Tan(180 - 90 - spotAngle) / range;
        print(spotLightWidth);

        Debug.DrawRay(transform.position, (new Vector3(spotLightWidth,0,range)) * 30, Color.cyan);
        /*Debug.DrawRay(transform.position, Vector3.forward * 30, Color.cyan);
        Debug.DrawRay(transform.position, Vector3.forward * 30, Color.cyan);
        Debug.DrawRay(transform.position, Vector3.forward * 30, Color.cyan);*/
    }
}