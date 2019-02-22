using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowProjection : MonoBehaviour{
    GameObject[] objectsToProject;

    void Start(){
        objectsToProject = GetRelevantRealObjects();
        for (int i = 0; i < objectsToProject.Length; i++) {
            UpdateShadow(objectsToProject[i]);
        }
    }

    private void Update()
    {
        UpdateShadow(objectsToProject[0]);
    }

    public void UpdateShadow(GameObject objectToUpdate) {
        Vector3[] vertices = objectToUpdate.GetComponent<MeshFilter>().mesh.vertices;

        for (int i = 0; i < vertices.Length; i++) {
            vertices[i] += objectToUpdate.transform.position;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, vertices[0] - transform.position, out hit, 50)) {
            //Something
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            Debug.DrawRay(transform.position, (vertices[i] - transform.position) * 50, Color.red);
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

        return objectsToProject.ToArray();
    }
}