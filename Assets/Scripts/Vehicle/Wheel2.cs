using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    public class Wheel2 : MonoBehaviour
    {
        public bool steering = true;
        public bool driving = true;

        public float steeringSpeed = 60f;

        [Header("Dimensions")]
        public float mass = 0.4f;
        public float radius = 0.254f;

        [Header("Power")]
        public float driveTorque = 8.69f;
        public float brakeTorque = 25f;

        [Header("Resistances")]
        [Range(0f, 1f)] public float rollingResistance = 0.0081f;
        [Range(0f, 1f)] public float corneringResistance = 0.12f;

        [Header("Coefficients")]
        [Range(0f, 1f)] public float brakingCoefficient = 0.5f;

        [Header("Variables")]
        public Vector3 steerDir = new Vector3(0, 0, 1);

        public float weightForce = 0;
        public float drivingForce = 0;
        public float brakingForce = 0;
        public float rollingResForce = 0;
        public float cornerResForce = 0;
        public float resultantForce = 0;

        public float steerAngle = 0;

        Vehicle2 vehicle;

        float curvature;
        float vehicleMass;
        float inverseVehicleMass;

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
                steerAngle += input * steeringSpeed * dt;
                if (Mathf.Abs(steerAngle) > 90f) steerAngle = Mathf.Sign(steerAngle) * 90f;
                steerDir.x = Mathf.Sin(steerAngle * Mathf.Deg2Rad);
                steerDir.z = transform.forward.z + Mathf.Cos(steerAngle * Mathf.Deg2Rad);
                transform.rotation = Quaternion.Euler(0, steerAngle, 0);
            }
        }

        public void Accelerate(float input, float dt)
        {
            drivingForce = Mathf.Abs(input) * driveTorque * curvature;
            brakingForce = Mathf.Abs(input) * BrakingForce(brakeTorque);
            rollingResForce = RollingResistanceForce();
            cornerResForce = 0; // TBD

            resultantForce = drivingForce - brakingForce - rollingResForce;

            //if (vehicle.Stationary)
            //    if (resultantForce < 0)
            //        resultantForce = 0;
        }

        public void Accelerate(float driveInput, float brakeInput, float dt)
        {
            drivingForce = Mathf.Abs(driveInput) * driveTorque * curvature;
            brakingForce = Mathf.Abs(brakeInput) * BrakingForce(brakeTorque);
            rollingResForce = RollingResistanceForce();
            cornerResForce = 0; // TBD

            resultantForce = drivingForce - brakingForce - rollingResForce;

            //if (vehicle.Stationary)
            //    if (resultantForce < 0)
            //        resultantForce = 0;
        }

        float RollingResistanceForce()
        {
            var speedForce = 0.5f * rollingResistance * vehicleMass * 9.81f * vehicle.groundSpeed * vehicle.groundSpeed;
            var flatForce = rollingResistance * vehicleMass * 9.81f;

            //if (vehicle.groundSpeed < forceSpeedThreshold) return speedForce;
            if (speedForce < flatForce) return speedForce;

            return flatForce;
        }

        float CorneringResistanceForce()
        {
            // Fy = c1 * sin(2 * arctan(weightForce/c2)) * alpha
            return 0;
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