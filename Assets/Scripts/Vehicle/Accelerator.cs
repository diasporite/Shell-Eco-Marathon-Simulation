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

        private void Awake()
        {
            vehicle = GetComponentInParent<Vehicle>();
        }

        public void Accelerate()
        {
            if (Input.GetKey(accelerateKey))
            {
                ds = acceleration * vehicle.DriveDir * Time.deltaTime;
                vehicle.Rb.velocity += ds;

                if (vehicle.Rb.velocity.sqrMagnitude > vehicle.topSpeed * vehicle.topSpeed)
                    vehicle.Rb.velocity = vehicle.Rb.velocity.normalized * vehicle.topSpeed;
            }
            else if (Input.GetKey(brakeKey))
            {
                ds = -acceleration * vehicle.DriveDir * Time.deltaTime;
                vehicle.Rb.velocity += ds;
                if (Vector3.Dot(vehicle.Forward, vehicle.Rb.velocity) < 0)
                    vehicle.Rb.velocity = Vector3.zero;
            }
        }

        public void Accelerate(Vector3 drive)
        {
            if (Input.GetKey(accelerateKey))
            {
                ds = acceleration * drive * Time.deltaTime;
                vehicle.Rb.velocity += ds;

                if (vehicle.Rb.velocity.sqrMagnitude > vehicle.topSpeed * vehicle.topSpeed)
                    vehicle.Rb.velocity = vehicle.Rb.velocity.normalized * vehicle.topSpeed;
            }
            else if (Input.GetKey(brakeKey))
            {
                ds = -acceleration * drive * Time.deltaTime;
                vehicle.Rb.velocity += ds;
                if (Vector3.Dot(vehicle.Forward, vehicle.Rb.velocity) < 0)
                    vehicle.Rb.velocity = Vector3.zero;
            }
        }
    }
}