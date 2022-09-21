using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetWhiteStoneManager : MonoBehaviour
{

    private Rigidbody rBody;

    [SerializeField] private Vector3 localGravity;

    // Start is called before the first frame update
    void Start()
    {
        rBody = gameObject.GetComponent<Rigidbody>();


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rBody.AddForce(localGravity, ForceMode.Force);

    }
}
