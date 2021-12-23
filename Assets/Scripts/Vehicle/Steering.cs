using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    public class Steering : MonoBehaviour
    {
        [Header("Properties")]
        public float steeringSpeed = 30;

        float wheelLock = 45f;
        float steerLock = 75f;

        float frontWheelTurningAngle;

        Vehicle vehicle;

        Wheel frontWheel;
        Wheel backWheel;

        public float FrontWheelTurningAngle => frontWheelTurningAngle;

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
                frontWheelTurningAngle += steeringSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
                if (Mathf.Abs(frontWheelTurningAngle) > wheelLock) frontWheelTurningAngle = wheelLock * Mathf.Sign(frontWheelTurningAngle);

                frontWheel.transform.rotation = Quaternion.Euler(0, frontWheelTurningAngle, 0);

                // Temporary - see Vehicle Dynamics doc
                var slipAngle = 0.5f * frontWheelTurningAngle;
                var steerAngle = slipAngle;
                return Quaternion.Euler(0, steerAngle, 0) * Vector3.forward;
            }

            return Vector3.zero;
        }
    }
}