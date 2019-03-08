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

        Vector3[] meshVerticesWithDuplicates = GetVertices(objectToUpdate.transform.position, objectToUpdate.transform.lossyScale, objectToUpdate);
        Vector3[] meshVertices = RemoveDuplicates(meshVerticesWithDuplicates);

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
            }
        }

        int amountShadowVertices = shadowVertices.Count;
        float[] vertx = new float[amountShadowVertices];
        float[] verty = new float[amountShadowVertices];

        for (int i = 0; i < amountShadowVertices; i++)
        {
            vertx[i] = shadowVertices[i].x;
            verty[i] = shadowVertices[i].y;
        }

        List<Vector3> outerShadowVertices = new List<Vector3>();

        for (int i = 0; i < amountShadowVertices; i++)
        {
            //IsOuterVertex(int nvert, float[] vertx, float[] verty, float testx, float testy)
            if (IsOuterVertex(amountShadowVertices, vertx, verty, shadowVertices[i].x, shadowVertices[i].y))
            {
                outerShadowVertices.Add(shadowVertices[i]);
            }
        }

        shadowVertices = outerShadowVertices;

        print(shadowVertices.Count);

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

    Vector3[] RemoveDuplicates(Vector3[] vertices) {
        List<Vector3> meshVertices = new List<Vector3>();
        meshVertices.Add(vertices[0]);
        bool gotDuplicate = false;

        for (int i = 1; i < vertices.Length; i++) {
            gotDuplicate = false;
            for (int j = 0; j < meshVertices.Count; j++) {
                if(vertices[i] == meshVertices[j]){
                    gotDuplicate = true;
                    break;
                }
            }
            if (!gotDuplicate){
                meshVertices.Add(vertices[i]);
            }
        }

        return meshVertices.ToArray();
    }

    //Prototypy
    Vector3 RotatePointAroundPivot(Vector3 point , Vector3 pivot , Vector3 angles) {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }

    bool IsOuterVertex(int nvert, float[] vertx, float[] verty, float testx, float testy){
        int j = nvert - 1;
        bool c = false;

        for (int i = 0; i < nvert; j = i++){
            if (((verty[i] > testy) != (verty[j] > testy)) &&
             (testx < (vertx[j] - vertx[i]) * (testy - verty[i]) / (verty[j] - verty[i]) + vertx[i]))
                c = !c;
        }
        return c;
    }

      /*List<Vector3> GetOuterVertices(List<Vector3> vertices) {


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
        }*/

    void GenerateMesh(Vector3[] vertices) {
        Mesh mesh = new Mesh();

        //GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }
}