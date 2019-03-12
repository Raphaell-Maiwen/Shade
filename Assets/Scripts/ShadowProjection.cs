using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShadowProjection : MonoBehaviour{
    public GameObject testMesh;
    GameObject[] objectsToProject;
    Mesh mesh;

    void Start(){
        objectsToProject = GetRelevantRealObjects();
        for (int i = 0; i < objectsToProject.Length; i++) {
            UpdateShadow(objectsToProject[i]);
        }
        mesh = new Mesh();
        
        print("mesh " + mesh.name);
        testMesh.GetComponent<MeshFilter>().mesh = mesh;
        mesh.Clear();
    }

    private void Update()
    {
        //Only for testing purposes; once the extrusion works, UpdateShadows shouldn't be called on every frame
        for (int i = 0; i < objectsToProject.Length; i++)
        {
            UpdateShadow(objectsToProject[i]);
        }
    }

    public void UpdateShadow(GameObject objectToUpdate) {
        Vector3[] meshVerticesWithDuplicates = GetVertices(objectToUpdate.transform.position, objectToUpdate.transform.lossyScale, objectToUpdate);
        Vector3[] meshVertices = RemoveDuplicates(meshVerticesWithDuplicates);

        List<Vertex> shadowVertices = new List<Vertex>();
        List<Triangle> triangles = new List<Triangle>();

        RaycastHit hit;
        
        int layerMask = LayerMask.GetMask("Wall");

        for (int i = 0; i < meshVertices.Length; i++) {
            if (Physics.Raycast(transform.position, Vector3.Normalize(meshVertices[i] - transform.position), out hit, 50, layerMask))
            {
                shadowVertices.Add(new Vertex(hit.point));
            }
        }

        //Why not just calling GetConvexHull and store the returned value in shadowVertices?
        List<Vertex> outerShadowVertices = new List<Vertex>();
        outerShadowVertices = GetConvexHull(shadowVertices);

        //For testing purposes
        for (int i = 0; i < outerShadowVertices.Count; i++) {
            if (Physics.Raycast(transform.position, Vector3.Normalize(outerShadowVertices[i].position - transform.position), out hit, 50, layerMask)) {
                Debug.DrawRay(transform.position, Vector3.Normalize((outerShadowVertices[i].position - transform.position)) * hit.distance, Color.red);
            }
        }

        triangles = TriangulateConvexPolygon(outerShadowVertices);
        
        GenerateMesh(outerShadowVertices.ToArray());
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

    public static List<Vertex> GetConvexHull(List<Vertex> points) {
        //The list with points on the convex hull
        List<Vertex> convexHull = new List<Vertex>();

        //Find the vertex with the smallest x coordinate
        //If several have the same x coordinate, find the one with the smallest y
        Vertex startVertex = points[0];

        Vector3 startPos = startVertex.position;

        for (int i = 1; i < points.Count; i++) {
            Vector3 testPos = points[i].position;

            //Because of precision issues, we use Mathf.Approximately to test if the x positions are the same
            if (testPos.x < startPos.x || (Mathf.Approximately(testPos.x, startPos.x) && testPos.y < startPos.y)) {
                startVertex = points[i];

                startPos = startVertex.position;
            }
        }

        //This vertex is necessarily on the convex hull
        convexHull.Add(startVertex);
        points.Remove(startVertex);


        Vertex currentPoint = convexHull[0];

        //Store colinear points here - better to create this list once than each loop
        List<Vertex> colinearPoints = new List<Vertex>();

        int counter = 0;

        //Loop to generate the convex hull
        while (true) {
            //After 2 iterations we have to add the start position again so we can terminate the algorithm
            //Can't use convexhull.count because of colinear points, so we need a counter
            if (counter == 2) {
                points.Add(convexHull[0]);
            }

            //Pick next point randomly
            Vertex nextPoint = points[Random.Range(0, points.Count)];

            //To 2d space so we can see if a point is to the left is the vector ab
            Vector2 a = currentPoint.GetPos2D_XY();

            Vector2 b = nextPoint.GetPos2D_XY();

            //Test if there's a point to the right of ab, if so then it's the new b
            for (int i = 0; i < points.Count; i++) {
                //Dont test the point we picked randomly
                if (points[i].Equals(nextPoint)) {
                    continue;
                }

                Vector2 c = points[i].GetPos2D_XY();

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

                    b = nextPoint.GetPos2D_XY();

                    //Clear colinear points
                    colinearPoints.Clear();
                }
                //To the left = worse point so do nothing
            }



            //If we have colinear points
            if (colinearPoints.Count > 0) {
                colinearPoints.Add(nextPoint);

                //Sort this list, so we can add the colinear points in correct order
                colinearPoints = colinearPoints.OrderBy(n => Vector3.SqrMagnitude(n.position - currentPoint.position)).ToList();

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

    public static List<Triangle> TriangulateConvexPolygon(List<Vertex> convexHullpoints) {
        List<Triangle> triangles = new List<Triangle>();

        for (int i = 2; i < convexHullpoints.Count; i++) {
            Vertex a = convexHullpoints[0];
            Vertex b = convexHullpoints[i - 1];
            Vertex c = convexHullpoints[i];

            triangles.Add(new Triangle(a, b, c));
        }

        return triangles;
    }

    static float IsAPointLeftOfVectorOrOnTheLine(Vector2 a, Vector2 b, Vector2 p) {
        float determinant = (a.x - p.x) * (b.y - p.y) - (a.y - p.y) * (b.x - p.x);

        return determinant;
    }

    void GenerateMesh(Vertex[] vertices) {
        //testMesh.GetComponent<MeshFilter>().mesh.Clear();
        //mesh.Clear();
        Vector3[] testVertices = new Vector3[] {
            vertices[0].position,
            vertices[1].position,
            vertices[2].position
        };

        int[] triangles = new int[] {
            0,1,2
        };


        testMesh.GetComponent<MeshFilter>().mesh.vertices = testVertices;
        testMesh.GetComponent<MeshFilter>().mesh.triangles = triangles;
        testMesh.GetComponent<MeshFilter>().mesh.RecalculateNormals();
    }

    //////////////////////////////////////////////////////////////

    //Geometrical data structures

    public class Vertex {
        public Vector3 position;

        //The outgoing halfedge (a halfedge that starts at this vertex)
        //Doesnt matter which edge we connect to it
        public HalfEdge halfEdge;

        //Which triangle is this vertex a part of?
        public Triangle triangle;

        //The previous and next vertex this vertex is attached to
        public Vertex prevVertex;
        public Vertex nextVertex;

        //Properties this vertex may have
        //Reflex is concave
        public bool isReflex;
        public bool isConvex;
        public bool isEar;

        public Vertex(Vector3 position) {
            this.position = position;
        }

        //Get 2d pos of this vertex
        public Vector2 GetPos2D_XY() {
            Vector2 pos_2d_xy = new Vector2(position.x, position.y);

            return pos_2d_xy;
        }
    }

    public class HalfEdge {
        //The vertex the edge points to
        public Vertex v;

        //The face this edge is a part of
        public Triangle t;

        //The next edge
        public HalfEdge nextEdge;
        //The previous
        public HalfEdge prevEdge;
        //The edge going in the opposite direction
        public HalfEdge oppositeEdge;

        //This structure assumes we have a vertex class with a reference to a half edge going from that vertex
        //and a face (triangle) class with a reference to a half edge which is a part of this face 
        public HalfEdge(Vertex v) {
            this.v = v;
        }
    }

    public class Triangle {
        //Corners
        public Vertex v1;
        public Vertex v2;
        public Vertex v3;

        //If we are using the half edge mesh structure, we just need one half edge
        public HalfEdge halfEdge;

        public Triangle(Vertex v1, Vertex v2, Vertex v3) {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }

        public Triangle(Vector3 v1, Vector3 v2, Vector3 v3) {
            this.v1 = new Vertex(v1);
            this.v2 = new Vertex(v2);
            this.v3 = new Vertex(v3);
        }

        public Triangle(HalfEdge halfEdge) {
            this.halfEdge = halfEdge;
        }

        //Change orientation of triangle from cw -> ccw or ccw -> cw
        public void ChangeOrientation() {
            Vertex temp = this.v1;

            this.v1 = this.v2;

            this.v2 = temp;
        }
    }
}