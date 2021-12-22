using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    public class Steering : MonoBehaviour
    {
        [Header("Properties")]
        public float steeringSpeed = 30;

        float wheelLock = 45;

        float thetaY;

        Vehicle vehicle;

        Wheel frontWheel;
        Wheel backWheel;

        public float ThetaY => thetaY;

        private void Awake()
        {
            vehicle = GetComponentInParent<Vehicle>();

            frontWheel = vehicle.frontWheel;
            backWheel = vehicle.backWheel;
        }

        // Calculate direction of travel of body
        public Vector3 SteerDir()
        {
            if (frontWheel.driving)
            {
                thetaY += steeringSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
                if (Mathf.Abs(thetaY) > wheelLock) thetaY = wheelLock * Mathf.Sign(thetaY);

                frontWheel.transform.rotation = Quaternion.Euler(0, thetaY, 0);
                return frontWheel.transform.forward;
                //if (dx != 0 && vehicle.Stationary)
                //    frontWheel.transform.localRotation = Quaternion.Euler(90 + dx, 0, 0);
            }

            return Vector3.zero;
        }
    }
}