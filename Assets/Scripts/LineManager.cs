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
    private Rigidbody2D rBody;

    private bool FalledFlag = false;

    public float accelerationScale; // 加速度の大きさ

    // Use this for initialization
    void Start () {
		prevMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        rb = gameObject.GetComponent<Rigidbody>();
        rBody = gameObject.GetComponent<Rigidbody2D>();
        MakeList();

        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        //rBody.AddForce(new Vector2(0,10), ForceMode2D.Force);
    }

    /*

	// Update is called once per frame
	void Update () {
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		if ( Input.GetMouseButton(0) ) {
			// ?}?E?X???????????????x?^????
			rb.velocity = (mousePos - prevMousePos) / Time.deltaTime;
		}

		prevMousePos = mousePos;
	}

    // ????
    void OnMouseDown()
    {
        rb.useGravity = false;
        this.screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        this.offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }
    // ????
    void OnMouseDrag()
    {
        Vector3 currentScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenPoint) + this.offset;
        transform.position = currentPosition;
    }
    */

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //WallLayer??
        if (collision.gameObject.layer == 13)
        {
            FalledFlag = true;
            rBody = this.GetComponent<Rigidbody2D>();
            Debug.Log("???");
            rBody.simulated = false; //??????rigidBody???d?????g??????????
            rBody.velocity = Vector2.zero;
            rBody.GetComponent<PolygonCollider2D>().isTrigger = false;
        }
    }

    private void FixedUpdate()
    {
        if (FalledFlag)
        {
            SetLocalGravity(); //?d????AddForce???????????\?b?h???????BFixedUpdate???D???????B
            Debug.Log("!!!!!");
        }
        else
        {
            //rBody.AddForce(localGravity, ForceMode2D.Force);
            rBody.simulated = true;
            rBody.AddForce(new Vector2(0, 15), ForceMode2D.Force);
            transform.position = Vector3.MoveTowards(transform.position, cam.transform.position　- new Vector3(5,0,5), 0.1f);
            Debug.Log("SSS");
        }
    }

    private void SetLocalGravity()
    {
        rBody.AddForce(localGravity, ForceMode2D.Force);
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
