using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyBoxManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Destroy", 0.5f);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Destroy()
    {
        
        Destroy(gameObject);
    }
}
