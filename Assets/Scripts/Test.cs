using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public float rotation;
    public float eulerAngle;

    public float driveForce = 100;

    public string input;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //rotation = transform.rotation.y;
        //eulerAngle = transform.eulerAngles.y;

        if (Input.GetKey("j")) input = "j";
        else if (Input.GetKey("l")) input = "l";
        else input = "";
    }

    private void FixedUpdate()
    {
        switch (input)
        {
            case "j":
                rb.AddForce(5 * transform.forward, ForceMode.Acceleration);
                break;
            case "l":
                rb.AddForce(-5 * transform.forward, ForceMode.Acceleration);
                break;
        }
    }
}
