using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    public class Steering : MonoBehaviour
    {
        [Header("Properties")]
        public float steeringSpeed = 45;
        public float wheelLock = 75f;
        public float steerLock = 75f;

        [Header("Variables")]
        [SerializeField] float frontWheelTurningAngle;
        [SerializeField] float vehicleVelocityAngle;
        [SerializeField] float slipAngle;
        [SerializeField] float steerAngle;

        float turningSpeed;

        [SerializeField] Vector3 steerDir = new Vector3(0, 0, 0);

        float dtheta = 0;

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

        private void Start()
        {
            steerDir = Vector3.forward;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, 2f * steerDir);  
        }

        // Calculate direction of travel of body
        public Vector3 SteerDir()
        {
            if (frontWheel.driving)
            {
                var dx = Input.GetAxisRaw("Horizontal");

                if (dx != 0)
                {
                    var yRotation = transform.eulerAngles.y;

                    frontWheelTurningAngle += steeringSpeed * dx * Time.deltaTime;
                    var frRot = frontWheel.transform.eulerAngles.y + frontWheelTurningAngle;
                    if (Mathf.Abs(frRot) > wheelLock)
                        frRot = wheelLock * Mathf.Sign(frRot);
                    frontWheel.transform.localRotation = Quaternion.Euler(0, frRot, 0);

                    // Temporary - see Vehicle Dynamics doc
                    vehicleVelocityAngle += 0.5f * steeringSpeed * dx * Time.deltaTime;
                    var vRot = transform.eulerAngles.y + vehicleVelocityAngle;
                    if (Mathf.Abs(vRot) > 0.5f * wheelLock) vRot = wheelLock * Mathf.Sign(vRot);
                    slipAngle = 0.2f * frontWheelTurningAngle;
                    steerAngle = frontWheelTurningAngle - slipAngle;

                    transform.rotation = Quaternion.Euler(0, vehicleVelocityAngle, 0);

                    steerDir.x = Mathf.Sin((transform.eulerAngles.y + steerAngle) * Mathf.Deg2Rad);
                    steerDir.z = Mathf.Cos((transform.eulerAngles.y + steerAngle) * Mathf.Deg2Rad);

                    return steerDir.normalized;
                }
            }

            return transform.forward;
        }
    }
}