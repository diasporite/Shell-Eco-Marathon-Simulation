using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    public class Steering : MonoBehaviour
    {
        [Header("Properties")]
        public float steeringSpeed = 30;

        public float wheelLock = 75f;
        public float steerLock = 75f;

        [Header("Variables")]
        [SerializeField] float frontWheelTurningAngle;

        float slipAngle;
        float steerAngle;

        [SerializeField] Vector3 steerDir = new Vector3(0, 0, 0);

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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, steerDir);  
        }

        // Calculate direction of travel of body
        public Vector3 SteerDir()
        {
            if (frontWheel.driving)
            {
                frontWheelTurningAngle += steeringSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
                if (Mathf.Abs(frontWheelTurningAngle) > wheelLock)
                    frontWheelTurningAngle = wheelLock * Mathf.Sign(frontWheelTurningAngle);

                frontWheel.transform.localRotation = Quaternion.Euler(0, frontWheelTurningAngle, 0);

                // Temporary - see Vehicle Dynamics doc
                slipAngle = 0.5f * frontWheelTurningAngle;
                steerAngle = slipAngle + transform.eulerAngles.y;
                if (Mathf.Abs(steerAngle) > steerLock)
                    steerAngle = steerLock * Mathf.Sign(steerAngle);

                steerDir.x = Mathf.Sin(steerAngle);
                steerDir.z = Mathf.Cos(steerAngle);

                return steerDir;
            }

            return Vector3.zero;
        }
    }
}