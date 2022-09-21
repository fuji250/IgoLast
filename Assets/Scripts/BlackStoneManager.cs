using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackStoneManager : MonoBehaviour
{
    GameController gamecontroller;

    public int destroyTime;

    // Start is called before the first frame update
    void Start()
    {
        gamecontroller = GameObject.Find("GameController").GetComponent<GameController>();
        Invoke("Destroy", destroyTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Destroy()
    {
        //int x = this.gameObject.transform.position.x;
        int x = Mathf.FloorToInt(this.gameObject.transform.position.x);
        int z = Mathf.FloorToInt(this.gameObject.transform.position.z);

        gamecontroller.DestroyStone(x, z);
        Destroy(gameObject);
    }
}
