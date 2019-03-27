using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShadowProjection : MonoBehaviour{
    GameObject[] objectsToProject;

    //The real world object as the key, the shadow object as the value
    Dictionary<GameObject, GameObject> shadowObjects = new Dictionary<GameObject,GameObject>();

    void Start(){
        objectsToProject = GetRelevantRealObjects();
        //shadowObjects = new GameObject[objectsToProject.Length];
        for (int i = 0; i < objectsToProject.Length; i++) {
            UpdateShadow(objectsToProject[i]);
        }
    }

    private void Update(){
        //Only for testing purposes; once the extrusion works, UpdateShadows shouldn't be called on every frame
        /*for (int i = 0; i < objectsToProject.Length; i++)
        {
            UpdateShadow(objectsToProject[i]);
        }*/

        if (Input.GetKeyDown(KeyCode.H)) {
            for (int i = 0; i < objectsToProject.Length; i++) {
                UpdateShadow(objectsToProject[i]);
            }
        }
    }

    public void UpdateShadow(GameObject objectToUpdate) {
        Vector3[] meshVerticesWithDuplicates = GetVertices(objectToUpdate.transform.position, objectToUpdate.transform.lossyScale, objectToUpdate);
        Vector3[] meshVertices = RemoveDuplicates(meshVerticesWithDuplicates);

        List<Vector3> shadowVertices = new List<Vector3>();

        RaycastHit hit;
        
        int layerMask = LayerMask.GetMask("Wall");

        for (int i = 0; i < meshVertices.Length; i++) {
            if (Physics.Raycast(transform.position, Vector3.Normalize(meshVertices[i] - transform.position), out hit, 100, layerMask)){
                shadowVertices.Add(hit.point);
            }
        }

        shadowVertices = GetConvexHull(shadowVertices);

        //For testing purposes
        for (int i = 0; i < shadowVertices.Count; i++) {
            if (Physics.Raycast(transform.position, Vector3.Normalize(shadowVertices[i] - transform.position), out hit, 100, layerMask)) {
                Debug.DrawRay(transform.position, Vector3.Normalize((shadowVertices[i] - transform.position)) * hit.distance, Color.red, 1f);
            }
        }

        int shadowVerticesCount = shadowVertices.Count;
        for (int i = 0; i < shadowVerticesCount; i++) {
            Vector3 newVertex = shadowVertices[i];
            newVertex.z -= 1;
            shadowVertices.Add(newVertex);
        }

        GenerateMesh(objectToUpdate, shadowVertices);
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
        foreach(Vector3 vertex in MeshVertices){
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

    public static List<Vector3> GetConvexHull(List<Vector3> points) {
        //The list with points on the convex hull
        List<Vector3> convexHull = new List<Vector3>();

        //Find the vertex with the smallest x coordinate
        //If several have the same x coordinate, find the one with the smallest y
        Vector3 startVertex = points[0];

        Vector3 startPos = startVertex;

        for (int i = 1; i < points.Count; i++) {
            Vector3 testPos = points[i];

            //Because of precision issues, we use Mathf.Approximately to test if the x positions are the same
            if (testPos.x < startPos.x || (Mathf.Approximately(testPos.x, startPos.x) && testPos.y < startPos.y)) {
                startVertex = points[i];

                startPos = startVertex;
            }
        }

        //This vertex is necessarily on the convex hull
        convexHull.Add(startVertex);
        points.Remove(startVertex);


        Vector3 currentPoint = convexHull[0];

        //Store colinear points here - better to create this list once than each loop
        List<Vector3> colinearPoints = new List<Vector3>();

        int counter = 0;

        //Loop to generate the convex hull
        while (true) {
            //After 2 iterations we have to add the start position again so we can terminate the algorithm
            //Can't use convexhull.count because of colinear points, so we need a counter
            if (counter == 2) {
                points.Add(convexHull[0]);
            }

            //Pick next point randomly
            Vector3 nextPoint = points[Random.Range(0, points.Count)];

            //To 2d space so we can see if a point is to the left is the vector ab
            Vector2 a = currentPoint;

            Vector2 b = nextPoint;

            //Test if there's a point to the right of ab, if so then it's the new b
            for (int i = 0; i < points.Count; i++) {
                //Dont test the point we picked randomly
                if (points[i].Equals(nextPoint)) {
                    continue;
                }

                Vector2 c = points[i];

                //Where is c in relation to a-b
                // < 0 -> to the right
                // = 0 -> on the line
                // > 0 -> to the left
                float relation = IsAPointLeftOfVectorOrOnTheLine(a, b, c);

                //Colinear points
                //Can't use exactly 0 because of floating point precision issues
                //This accuracy is smallest possible, if smaller points will be missed if we are testing with a plane
                float accuracy = 0.00001f;

                if (relation < accuracy && relation > -accuracy) {
                    colinearPoints.Add(points[i]);
                }
                //To the right = better point, so pick it as next point on the convex hull
                else if (relation < 0f) {
                    nextPoint = points[i];

                    b = nextPoint;

                    //Clear colinear points
                    colinearPoints.Clear();
                }
                //To the left = worse point so do nothing
            }

            //If we have colinear points
            if (colinearPoints.Count > 0) {
                colinearPoints.Add(nextPoint);

                //Sort this list, so we can add the colinear points in correct order
                colinearPoints = colinearPoints.OrderBy(n => Vector3.SqrMagnitude(n - currentPoint)).ToList();

                convexHull.AddRange(colinearPoints);

                currentPoint = colinearPoints[colinearPoints.Count - 1];

                //Remove the points that are now on the convex hull
                for (int i = 0; i < colinearPoints.Count; i++) {
                    points.Remove(colinearPoints[i]);
                }

                colinearPoints.Clear();
            }
            else {
                convexHull.Add(nextPoint);

                points.Remove(nextPoint);

                currentPoint = nextPoint;
            }

            //Have we found the first point on the hull? If so we have completed the hull
            if (currentPoint.Equals(convexHull[0])) {
                //Then remove it because it is the same as the first point, and we want a convex hull with no duplicates
                convexHull.RemoveAt(convexHull.Count - 1);

                break;
            }

            counter += 1;
        }

        return convexHull;
    }

    static float IsAPointLeftOfVectorOrOnTheLine(Vector2 a, Vector2 b, Vector2 p) {
        float determinant = (a.x - p.x) * (b.y - p.y) - (a.y - p.y) * (b.x - p.x);

        return determinant;
    }

    ///////////////////////////////////////////////////////////

    void GenerateMesh(GameObject realWorldObject, List<Vector3> shadowVertices){
        float height = shadowVertices[1].y;
        // Compute the center point of the polygon both on the ground, and at height
        // Add center vertices to end of list
        Vector3 center = findCenter(shadowVertices);
        //shadowVertices.Add(center);
        Vector3 raisedCenter = center;
        raisedCenter.y += height;
        //shadowVertices.Add(raisedCenter);

        List<int> tris = new List<int>();
        // Convert vertices to array for mesh
        Vector3[] vertices = shadowVertices.ToArray();

        // Do the triangles for the roof and the floor of the mesh
        // Roof points are at odd indeces
        for (int j = vertices.Length - 3; j >= 0; j--) {
            // Add the point
            tris.Add(j);
            // Check for wrap around
            if (j - 2 >= 0) {
                tris.Add(j - 2);
            }
            else {
                // If wrap around, add the first vertex
                int diff = j - 2;
                tris.Add(vertices.Length - 2 + diff);
            }
            // Check if its at ground or mesh height level, choose proper center point
            if (j % 2 == 0) {
                tris.Add(vertices.Length - 2);
            }
            else {
                tris.Add(vertices.Length - 1);
            }
        }

        // Do triangles which connect roof to ground
        for (int j = vertices.Length - 3; j >= 2; j--) {
            if (j % 2 == 1) {
                tris.Add(j);
                tris.Add(j - 1);
                tris.Add(j - 2);
            }
            else {
                tris.Add(j);
                tris.Add(j - 2);
                tris.Add(j - 1);
            }
        }

        int[] triangles = tris.ToArray();

        //Find which GameObject to apply the new mesh to
        //If it doesn't exist, create it
        GameObject shadowObject;
        if (shadowObjects.ContainsKey(realWorldObject)) {
            shadowObject = shadowObjects[realWorldObject];
        }
        else {
            shadowObject = new GameObject();
            shadowObject.AddComponent<MeshFilter>();
            shadowObject.AddComponent<MeshRenderer>();
            shadowObjects.Add(realWorldObject, shadowObject);
        }


        // Create and apply the mesh
        Destroy(shadowObject.GetComponent<MeshCollider>());

        MeshFilter mf = shadowObject.GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mf.mesh = mesh;
        shadowObject.GetComponent<MeshRenderer>().enabled = false;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        shadowObject.AddComponent<MeshCollider>();
    }

    // Find the center X-Z position of the polygon.
    Vector3 findCenter(List<Vector3> verts) {
        Vector3 center = Vector3.zero;
        // Only need to check every other spot since the odd indexed vertices are in the air, but have same XZ as previous
        for (int i = 0; i < verts.Count; i += 2) {
            center += verts[i];
        }
        return center / (verts.Count / 2);

    }
}