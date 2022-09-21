using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    private  Camera cam;
    // ????
    private Vector3 screenPoint;
    private Vector3 offset;

    // Start is called before the first frame update
    private Vector3 prevMousePos;

    Rigidbody rb;

    [SerializeField] private Vector2 localGravity;
    private Rigidbody rBody;

    private bool FalledFlag = false;

    public float accelerationScale; // 加速度の大きさ

    public GameObject whiteStone;

    public int StoneCount = 0;

    //点座標X
    public int X;
    //点座標Y
    public int Z;

    // Use this for initialization
    void Start () {
		prevMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        rb = gameObject.GetComponent<Rigidbody>();
        rBody = gameObject.GetComponent<Rigidbody>();
        MakeList();

        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        //rBody.AddForce(new Vector2(0,10), ForceMode2D.Force);
    }

    private void OnTriggerEnter(Collider collision)
    {
        //WallLayer??
        if (collision.gameObject.layer == 2)
        {
            FalledFlag = true;
            rBody = this.GetComponent<Rigidbody>();
            rBody.useGravity = false; //??????rigidBody???d?????g??????????
            rBody.velocity = Vector2.zero;
            rBody.GetComponent<BoxCollider>().isTrigger = false;
            Debug.Log("!!!!!");
            LineRenderer lineRenderer = this.GetComponent<LineRenderer>();
            for (var i = 0; i < lineRenderer.positionCount; i++)
            {
                X += (int)lineRenderer.GetPosition(i).x;
                Z += (int)lineRenderer.GetPosition(i).z;
            }
            for (int i = 0; i < StoneCount; i++)
            {
                //Instantiate(whiteStone, this.gameObject.transform.position + new Vector3(5, 0, 5), Quaternion.identity);
                Instantiate(whiteStone, new Vector3(X/ lineRenderer.positionCount + this.transform.position.x, this.transform.position.y, Z/ lineRenderer.positionCount + this.transform.position.z), Quaternion.identity);
                Debug.Log("AAA");

            }
            Destroy(this.gameObject);

        }
    }

    private void FixedUpdate()
    {
        if (FalledFlag)
        {
            SetLocalGravity(); //?d????AddForce???????????\?b?h???????BFixedUpdate???D???????B
        }
        else
        {
            //rBody.AddForce(localGravity, ForceMode2D.Force);
            rBody.useGravity = true;
            rBody.AddForce(new Vector2(0, 15), ForceMode.Force);
            transform.position = Vector3.MoveTowards(transform.position, cam.transform.position　- new Vector3(5,0,5), 0.1f);
            
        }
    }

    private void SetLocalGravity()
    {
        rBody.AddForce(localGravity, ForceMode.Force);
    }

    public void MakeList()
    {
        //CreateFilledGraphShape(this.GetComponent<LineRenderer>())
    }

    public void CreateFilledGraphShape(Vector3[] linePoints)
    {
        int i = 0;
        Vector3[] filledGraphPoints = new Vector3[linePoints.Length * 2]; // one point below each line point
        for (i = 0; i < linePoints.Length; ++i)
        {
            filledGraphPoints[2 * i] = new Vector3(linePoints[i].x, 0, 0);
            filledGraphPoints[2 * i + 1] = linePoints[i];
        }

        int numTriangles = (linePoints.Length - 1) * 2;
        int[] triangles = new int[numTriangles * 3];

        i = 0;
        for (int t = 0; t < numTriangles; t += 2)
        {
            // lower left triangle
            triangles[i++] = 2 * t;
            triangles[i++] = 2 * t + 1;
            triangles[i++] = 2 * t + 2;
            // upper right triangle - you might need to experiment what are the correct indices
            triangles[i++] = 2 * t + 1;
            triangles[i++] = 2 * t + 2;
            triangles[i++] = 2 * t + 3;
        }

        // create mesh
        Mesh filledGraphMesh = new Mesh();
        filledGraphMesh.vertices = filledGraphPoints;
        filledGraphMesh.triangles = triangles;
        // you might need to assign texture coordinates as well

        // create game object and add renderer and mesh to it
        GameObject filledGraph = new GameObject("Filled graph");
        MeshRenderer renderer = filledGraph.AddComponent<MeshRenderer>();
        //renderer.mesh = mesh;
    }
}
