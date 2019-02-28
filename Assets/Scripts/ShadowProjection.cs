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
        //Only for testing purposes; once the extrusion work, UpdateShadows shouldn't be called on every frame
        for (int i = 0; i < objectsToProject.Length; i++)
        {
            UpdateShadow(objectsToProject[i]);
        }
    }

    public void UpdateShadow(GameObject objectToUpdate) {
        //print(objectToUpdate.transform.forward);

        Vector3[] vertices = GetVertices(objectToUpdate.transform.position, objectToUpdate.transform.lossyScale, objectToUpdate);

        //Vector3[] vertices = objectToUpdate.GetComponent<MeshFilter>().mesh.vertices;
        RaycastHit hit;
        
        int layerMask = LayerMask.GetMask("Wall");

        for (int i = 0; i < vertices.Length; i++) {
            //print(vertices[i]);

            //vertices[i] += objectToUpdate.transform.position;
            if (Physics.Raycast(transform.position, Vector3.Normalize(vertices[i] - transform.position), out hit, 50, layerMask))
            {
                Debug.DrawRay(transform.position, Vector3.Normalize((vertices[i] - transform.position)) * hit.distance, Color.red);
                //print("Found an object - distance: " + hit.distance + " " + hit.transform.position);
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

        return objectsToProject.ToArray();
    }

    Vector3[] GetVertices(Vector3 center, Vector3 size, GameObject GO) {
        float width = size.x / 2;
        float height = size.y / 2;
        float depth = size.z / 2;

        List<Vector3> vertices = new List<Vector3>();
        vertices.Add(center + GO.transform.up * height + GO.transform.right * width + transform.forward * depth);
        vertices.Add(center - GO.transform.up * height + GO.transform.right * width + transform.forward * depth);
        vertices.Add(center - GO.transform.up * height - GO.transform.right * width + transform.forward * depth);
        vertices.Add(center - GO.transform.up * height + GO.transform.right * width - transform.forward * depth);
        vertices.Add(center - GO.transform.up * height - GO.transform.right * width - transform.forward * depth);
        vertices.Add(center + GO.transform.up * height - GO.transform.right * width - transform.forward * depth);
        vertices.Add(center + GO.transform.up * height - GO.transform.right * width + transform.forward * depth);
        vertices.Add(center + GO.transform.up * height + GO.transform.right * width - transform.forward * depth);

        return vertices.ToArray();
    }
}