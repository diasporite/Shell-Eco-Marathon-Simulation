using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    public class Steering : MonoBehaviour
    {
        public float steeringSpeed = 90;

        float steerLock = 360;

        float dy;
        Vector3 ds;

        Vehicle vehicle;

        private void Awake()
        {
            vehicle = GetComponentInParent<Vehicle>();
        }

        private void Update()
        {
            Steer();
        }

        public void Steer()
        {
            dy += steeringSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
            if (Mathf.Abs(dy) > steerLock) dy = steerLock * Mathf.Sign(dy);

            if (dy != 0 && vehicle.Rb.velocity != Vector3.zero)
                vehicle.transform.rotation = Quaternion.Euler(0, dy, 90);
        }
    }
}