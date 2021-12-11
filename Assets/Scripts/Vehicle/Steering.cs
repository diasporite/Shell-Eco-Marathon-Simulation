using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    public class Steering : MonoBehaviour
    {
        public float steeringSpeed = 90;

        float steerLock = 360;
        float wheelLock = 90;

        float dx;

        Vehicle vehicle;

        Wheel frontWheel;
        Wheel backWheel;

        public float Dx => dx;

        private void Awake()
        {
            vehicle = GetComponentInParent<Vehicle>();

            frontWheel = vehicle.frontWheel;
            backWheel = vehicle.backWheel;
        }

        public void Steer()
        {
            if (frontWheel.driving)
            {
                dx += steeringSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
                if (Mathf.Abs(dx) > wheelLock) dx = wheelLock * Mathf.Sign(dx);

                //if (dx != 0 && vehicle.Stationary)
                //    frontWheel.transform.localRotation = Quaternion.Euler(90 + dx, 0, 0);
            }
        }

        public Vector3 SteerDir()
        {
            if (frontWheel.driving)
            {
                dx += steeringSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
                if (Mathf.Abs(dx) > wheelLock) dx = wheelLock * Mathf.Sign(dx);

                //if (dx != 0 && vehicle.Stationary)
                //    frontWheel.transform.localRotation = Quaternion.Euler(90 + dx, 0, 0);
            }

            return Vector3.zero;
        }
    }
}