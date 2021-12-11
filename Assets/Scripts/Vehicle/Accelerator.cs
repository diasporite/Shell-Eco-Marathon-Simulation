using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    public class Accelerator : MonoBehaviour
    {
        public string accelerateKey = "f";
        public string brakeKey = "j";

        public float acceleration = 0.1f;

        Vehicle vehicle;
        Vector3 ds;

        Rigidbody rb;

        private void Awake()
        {
            vehicle = GetComponentInParent<Vehicle>();

            rb = GetComponent<Rigidbody>();
        }

        public void Accelerate()
        {
            if (Input.GetKey(accelerateKey))
            {
                ds = acceleration * vehicle.DriveDir * Time.deltaTime;
                rb.velocity += ds;

                if (rb.velocity.sqrMagnitude > vehicle.topSpeed * vehicle.topSpeed)
                    rb.velocity = rb.velocity.normalized * vehicle.topSpeed;
            }
            else if (Input.GetKey(brakeKey))
            {
                ds = -acceleration * vehicle.DriveDir * Time.deltaTime;
                rb.velocity += ds;
                if (Vector3.Dot(vehicle.Forward, rb.velocity) < 0)
                    rb.velocity = Vector3.zero;
            }
        }

        public void Accelerate(Vector3 drive)
        {
            if (Input.GetKey(accelerateKey))
            {
                ds = acceleration * drive * Time.deltaTime;
                rb.velocity += ds;

                if (rb.velocity.sqrMagnitude > vehicle.topSpeed * vehicle.topSpeed)
                    rb.velocity = rb.velocity.normalized * vehicle.topSpeed;
            }
            else if (Input.GetKey(brakeKey))
            {
                ds = -acceleration * drive * Time.deltaTime;
                rb.velocity += ds;
                if (Vector3.Dot(vehicle.Forward, rb.velocity) < 0)
                    rb.velocity = Vector3.zero;
            }
        }
    }
}