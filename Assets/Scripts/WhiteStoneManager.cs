using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteStoneManager : MonoBehaviour
{
    public GameObject prefabObj;
    List<Vector3> PointList;
    public List<GameObject> lineList = new List<GameObject>();

    GameController gamecontroller;


    public bool touchBlackFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        gamecontroller = GameObject.Find("GameController").GetComponent<GameController>();
        //2?b?????I?u?W?F?N?g??????
        Invoke("Destroy", 12);
    }

    // Update is called once per frame
    void Update()
    {


    }

    private void OnTriggerEnter(Collider collision)
    {
        //BlackStone???C???[????
        if (collision.gameObject.layer == 10)
        {
            touchBlackFlag = true;
        }

        //WhiteStone???C???[????
        if (collision.gameObject.layer == 9)
        {
            if (touchBlackFlag)
            {
                // ???????g???K?[?????G???????A?P?x????????????
                // ?Q?[???I?u?W?F?N?g?????????????B
                GameObject beam = Instantiate(prefabObj, Vector3.zero, Quaternion.identity);
                // LineRenderer????
                LineRenderer line = beam.GetComponent<LineRenderer>();

                lineList.Add(beam);

                if (collision.GetComponent<WhiteStoneManager>() != null)
                {
                    if (collision.GetComponent<WhiteStoneManager>().touchBlackFlag)
                    {
                        // ???_????????
                        //line.positionCount = PointList.Count;
                        line.positionCount = 2;
                        line.SetPosition(0, this.transform.position);
                        line.SetPosition(1, collision.transform.position);
                    }
                }
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        //Debug.Log("???????????I");
    }

    void Destroy()
    {
        //int x = this.gameObject.transform.position.x;
        int x = Mathf.FloorToInt(this.gameObject.transform.position.x);
        int z = Mathf.FloorToInt(this.gameObject.transform.position.z);

        gamecontroller.DestroyStone(x, z);
        Destroy(gameObject);

        //???C????????
        for (int i = 0; i < lineList.Count; i++)
        {
            Destroy(lineList[i]);
        }
    }
}
