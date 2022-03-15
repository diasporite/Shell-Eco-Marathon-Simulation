using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    public class Wheel2 : MonoBehaviour
    {
        public bool steering = true;
        public bool driving = true;

        [Header("Steering")]
        public float steeringSpeed = 60f;
        public float wheelLock = 75f;

        [Header("Dimensions")]
        public float mass = 0.4f;
        public float radius = 0.254f;
        public float tyrePressure = 500000f;

        [Header("Power")]
        public float driveTorque = 8.69f;
        public float brakeTorque = 25f;

        [Header("Resistances")]
        [Range(0f, 1f)] public float rollingResistance = 0.0081f;
        [Range(0f, 1f)] public float corneringResistance = 0.12f;

        [Header("Coefficients")]
        [Range(0f, 1f)] public float brakingCoefficient = 0.5f;

        [Header("Variables - Steering")]
        public float dtheta = 0;
        public float steerAngle = 0;    // delta in dynamics doc
        public float slipAngle = 0;     // alpha in dynamics doc
        public Vector3 steerDir = new Vector3(0, 0, 1);

        [Header("Variables - Forces")]
        public float weightForce = 0;
        public float drivingForce = 0;
        public float brakingForce = 0;
        public float rollingResForce = 0;
        public float cornerResForce = 0;
        public float resultantForce = 0;

        Vehicle2 vehicle;

        float curvature;
        float vehicleMass;
        float inverseVehicleMass;

        // See vehicle dynamics doc
        float c1 = 148f;
        float c2 = 1000f;

        public Vector3 LocalSteerDir => steerDir;

        private void Awake()
        {
            vehicle = GetComponentInParent<Vehicle2>();

            weightForce = mass * 9.81f;

            curvature = 1 / radius;
        }

        private void Start()
        {
            vehicleMass = vehicle.VehicleMass;
            inverseVehicleMass = vehicle.InverseVehicleMass;
        }

        private void OnDrawGizmos()
        {
            if (steering)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, 
                    1f * (transform.root.rotation * steerDir).normalized);
            }
        }

        public void Steer(float input, float dt)
        {
            if (steering)
            {
                dtheta = input * steeringSpeed * dt;
                steerAngle += dtheta;

                if (Mathf.Abs(steerAngle) > wheelLock)
                    steerAngle = Mathf.Sign(steerAngle) * wheelLock;

                slipAngle = 0.5f * steerAngle;

                steerDir.x = Mathf.Sin(steerAngle * Mathf.Deg2Rad);
                steerDir.z = transform.forward.z + Mathf.Cos(steerAngle * Mathf.Deg2Rad);

                // TEMPORARY
                if (Mathf.Abs(steerAngle) < wheelLock)
                    transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + dtheta, 0);
            }
        }

        public void Accelerate(float driveInput, float brakeInput, float dt)
        {
            drivingForce = Mathf.Abs(driveInput) * driveTorque * curvature;
            brakingForce = Mathf.Abs(brakeInput) * BrakingForce(brakeTorque);
            rollingResForce = RollingResistanceForce();

            cornerResForce = CorneringResistanceForce(); // TBD

            resultantForce = drivingForce - brakingForce - rollingResForce;
        }

        float RollingResistanceForce()
        {
            //var speedForce = (0.005f + (0.01f + 0.0095f * (0.01f * 0.01f * vehicle.groundSpeed * vehicle.groundSpeed) / tyrePressure) * weightForce);
            //var speedForce = (0.0095f * (0.01f * 0.01f * vehicle.groundSpeed * vehicle.groundSpeed) / tyrePressure) * weightForce;
            var speedForce = 0.5f * rollingResistance * vehicleMass * 9.81f * vehicle.groundSpeed * vehicle.groundSpeed;
            //var flatForce = rollingResistance * vehicleMass * 9.81f;
            // Source: https://www.engineeringtoolbox.com/rolling-friction-resistance-d_1303.html
            var flatForce = rollingResistance * weightForce * curvature;

            //if (vehicle.groundSpeed < forceSpeedThreshold) return speedForce;
            if (speedForce < flatForce) return speedForce;

            //return speedForce;
            return flatForce;
        }

        float CorneringResistanceForce()
        {
            var f = c1 * Mathf.Sin(2 * Mathf.Atan(weightForce / c2)) * slipAngle;
            //return -weightForce * Mathf.Sin(steerAngle * Mathf.Deg2Rad);
            return f;
        }

        float BrakingForce(float torque)
        {
            var speedForce = 0.5f * brakingCoefficient * vehicleMass * 9.81f * vehicle.groundSpeed * vehicle.groundSpeed;
            var flatForce = brakeTorque * curvature;

            //if (vehicle.groundSpeed < forceSpeedThreshold) return speedForce;
            if (speedForce < flatForce) return speedForce;

            return flatForce;
        }
    }
}