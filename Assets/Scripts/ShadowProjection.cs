using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowProjection : MonoBehaviour{
    GameObject[] objectsToProject;
    //Vector3 

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

        Vector3[] meshVertices = GetVertices(objectToUpdate.transform.position, objectToUpdate.transform.lossyScale, objectToUpdate);
        List<Vector3> shadowVertices = new List<Vector3>();

        RaycastHit hit;
        
        int layerMask = LayerMask.GetMask("Wall");

        for (int i = 0; i < meshVertices.Length; i++) {
            //print(vertices[i]);

            //vertices[i] += objectToUpdate.transform.position;
            if (Physics.Raycast(transform.position, Vector3.Normalize(meshVertices[i] - transform.position), out hit, 50, layerMask))
            {
                Debug.DrawRay(transform.position, Vector3.Normalize((meshVertices[i] - transform.position)) * hit.distance, Color.red);
                shadowVertices.Add(hit.point);
                //print(hit.point);
            }
        }

        shadowVertices = GetOuterVertices(shadowVertices);

        GenerateMesh(shadowVertices.ToArray());
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
        /*float width = size.x / 2;
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
        vertices.Add(center + GO.transform.up * height + GO.transform.right * width - transform.forward * depth);*/

        List<Vector3> MeshVertices = new List<Vector3>();
        List<Vector3> vertices = new List<Vector3>();
        GO.GetComponent<MeshFilter>().mesh.GetVertices(MeshVertices);

        int i = 0;
        foreach(Vector3 vertex in MeshVertices)
        {
            vertices.Add(GO.transform.TransformPoint(vertex));
        }

        return vertices.ToArray();
    }

    //Prototypy
    Vector3 RotatePointAroundPivot(Vector3 point , Vector3 pivot , Vector3 angles) 
    {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }

List<Vector3> GetOuterVertices(List<Vector3> vertices) {
        float maxY = vertices[0].y;
        float minY = vertices[0].y;
        float maxX = vertices[0].x;
        float minX = vertices[0].x;

        List<Vector3> outerVertices = new List<Vector3>();

        //Finding the greatest and smallest points
        for (int i = 1; i < vertices.Count; i++) {
            //Finding the biggest and smallest x points
            if (vertices[i].x > maxX){
                maxX = vertices[i].x;
            }
            else if (vertices[i].x < minX) {
                minX = vertices[i].x;
            }

            //Finding the biggest and smallest y points
            if (vertices[i].y > maxY){
                maxY = vertices[i].y;
            }
            else if (vertices[i].y < minY) {
                minY = vertices[i].y;
            }
        }

        //Transferring only the outer vertices to the new list
        for (int i = 0; i < vertices.Count; i++) {
            //Transferring the biggest and smallest x points
            if (vertices[i].x == maxX)
            {
                outerVertices.Add(vertices[i]);
                continue;
            }
            else if (vertices[i].x == minX)
            {
                outerVertices.Add(vertices[i]);
                continue;
            }

            //Transferring the biggest and smallest y points
            if (vertices[i].y == maxY)
            {
                outerVertices.Add(vertices[i]);
                continue;
            }
            else if (vertices[i].y == minY)
            {
                outerVertices.Add(vertices[i]);
                continue;
            }
        }

        return outerVertices;
    }

    void GenerateMesh(Vector3[] vertices) {
        Mesh mesh = new Mesh();

        //GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }
}