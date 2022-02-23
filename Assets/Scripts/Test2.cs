using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : MonoBehaviour
{
    public float speed = 8f;

    public string key;

    Rigidbody rb;

    float relativeFrontWheels = 0.75f;
    float relativeBackWheel = -0.75f;
    float leverDist;

    Vector3 appliedPos = new Vector3(0, 0);
    Vector3 appliedTorque = new Vector3(0, 0);

    Vector3 SteerDir => (transform.forward - transform.right).normalized;
    Vector3 TorqueDir => transform.up;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        leverDist = relativeFrontWheels - relativeBackWheel;
        appliedPos = transform.position + relativeFrontWheels * transform.forward;
    }

    private void Update()
    {
        if (Input.GetKey("space")) key = "space";
        else key = "";
    }

    private void FixedUpdate()
    {
        appliedPos = transform.position + relativeFrontWheels * transform.forward;

        if (key == "space")
        {
            //AddForceAtPosition();
            //AddTorque();
            AddForce();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawRay(appliedPos, -2f * SteerDir);
    }

    void AddForceAtPosition()
    {
        rb.AddForceAtPosition(-speed * SteerDir * Time.fixedDeltaTime,
            appliedPos, ForceMode.VelocityChange);
    }

    void AddTorque()
    {
        //appliedTorque = -speed * transform.up * leverDist;
        appliedTorque = -speed * TorqueDir * leverDist;
        rb.AddRelativeTorque(appliedTorque);
        //rb.AddTorque(appliedTorque);
    }

    void AddForce()
    {
        var f = 2f - 50f * rb.velocity.magnitude * rb.velocity.magnitude;

        rb.AddRelativeForce(f * transform.forward, ForceMode.Acceleration);
    }

    /// NOTES
    /// 
    /// For steering, only x component is relevant
    /// AddTorque() rotates AROUND axis
    /// AddRelativeTorque() = local, AddTorque() = world
}
