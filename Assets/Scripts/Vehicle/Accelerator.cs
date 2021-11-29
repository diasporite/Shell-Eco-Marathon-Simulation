using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    public class Accelerator : MonoBehaviour
    {
        public string accelerate = "f";
        public string brake = "j";

        public float acceleration = 0.1f;

        Vehicle vehicle;
        Vector3 ds;

        private void Awake()
        {
            vehicle = GetComponentInParent<Vehicle>();
        }

        private void Update()
        {
            Accelerate();

            Decelerate();
        }

        public void Accelerate()
        {
            if (Input.GetKey(accelerate))
            {
                ds = acceleration * vehicle.Forward * Time.deltaTime;
                vehicle.Rb.velocity += ds;

                if (vehicle.Rb.velocity.sqrMagnitude > vehicle.topSpeed * vehicle.topSpeed)
                    vehicle.Rb.velocity = vehicle.Rb.velocity.normalized * vehicle.topSpeed;
            }
        }

        public void Decelerate()
        {
            if (Input.GetKey(brake))
            {
                ds = -acceleration * vehicle.Forward * Time.deltaTime;
                vehicle.Rb.velocity += ds;
                if (Vector3.Dot(vehicle.Forward, vehicle.Rb.velocity) < 0)
                    vehicle.Rb.velocity = Vector3.zero;
            }
        }
    }
}