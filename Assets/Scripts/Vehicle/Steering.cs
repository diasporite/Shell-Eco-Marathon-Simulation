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
        float dy;

        Vector3 ds;

        Vehicle vehicle;

        Wheel frontWheel;
        Wheel backWheel;

        private void Awake()
        {
            vehicle = GetComponentInParent<Vehicle>();

            frontWheel = vehicle.frontWheel;
            backWheel = vehicle.backWheel;
        }

        public void SimpleSteer()
        {
            dy += steeringSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
            if (Mathf.Abs(dy) > steerLock) dy = steerLock * Mathf.Sign(dy);

            if (dy != 0 && vehicle.Stationary)
                vehicle.transform.rotation = Quaternion.Euler(0, dy, 90);
        }

        public void Steer()
        {
            if (frontWheel.driving)
            {
                dx += steeringSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
                if (Mathf.Abs(dx) > wheelLock) dx = wheelLock * Mathf.Sign(dx);

                if (dx != 0 && vehicle.Stationary)
                    frontWheel.transform.localRotation = Quaternion.Euler(90 + dx, 0, 0);
            }
        }
    }
}