using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    public class Wheel : MonoBehaviour
    {
        public GameObject model;

        public bool driving = true;

        [Header("Dimensions")]
        public float mass = 5;
        public float radius = 1;
        [SerializeField] Vector3 contactPoint;

        [Header("Power")]
        public float driveTorque = 500f;
        public float brakeTorque = 1500f;
        [Range(0f, 0.5f)] public float rollingResistance = 0.25f;
        [Range(0f, 0.5f)] public float corneringResistance = 0.25f;

        [Header("Constants")]
        public float wheelLock = 45f;
        public float steeringSpeed = 15f;

        [Header("Variables - Wheel Angles")]
        [SerializeField] float globalWheelTurningAngle = 0;
        [SerializeField] float wheelTurningAngle = 0;
        [SerializeField] float slipAngle = 0;
        [SerializeField] float wheelSpeedDeflectionAngle = 0;

        [Header("Variables - Forces")]
        [SerializeField] float drivingForce = 0;
        [SerializeField] float resistanceForce = 0;
        [SerializeField] float resultantForce = 0;
        [SerializeField] float lateralForce = 0;
        [SerializeField] float wheelAcceleration = 0;
        [SerializeField] float speed = 0;
        [SerializeField] Vector3 velocity = new Vector3(0, 0, 0);

        Vehicle vehicle;

        float curvature;
        float inverseVehicleMass;

        public Vector3 ContactPoint => contactPoint;

        public float WheelTurningAngle => wheelTurningAngle;
        public float SlipAngle => slipAngle;
        public float WheelSpeedDeflectionAngle => wheelSpeedDeflectionAngle;

        public float ResultantForce => resultantForce;
        public float WheelAcceleration => wheelAcceleration;
        public Vector3 Velocity => velocity;

        private void Awake()
        {
            vehicle = GetComponentInParent<Vehicle>();

            contactPoint = transform.position + radius * Vector3.down;

            curvature = 1 / radius;
        }

        private void Start()
        {
            inverseVehicleMass = vehicle.InverseVehicleMass;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, 5f * transform.forward);

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, 5f * velocity.normalized);
        }

        public void SteerWheel(float input, float dt)
        {
            if (driving)
            {
                // Turn wheel
                wheelTurningAngle += steeringSpeed * input * dt;
                globalWheelTurningAngle += steeringSpeed * input * dt;
                if (Mathf.Abs(wheelTurningAngle) > wheelLock)
                    wheelTurningAngle = wheelLock * Mathf.Sign(wheelTurningAngle);

                slipAngle = CalculateSlipAngle(wheelTurningAngle);
                wheelSpeedDeflectionAngle = wheelTurningAngle - slipAngle;

                //transform.localRotation = Quaternion.Euler(0, wheelTurningAngle, 0);
                //transform.localRotation = Quaternion.Euler(0, globalWheelTurningAngle, 0);
                transform.rotation = Quaternion.Euler(0, globalWheelTurningAngle, 0);
            }
        }

        public void DriveWheel(float input, float dt)
        {
            if (driving)
            {
                var torque = 0f;

                if (input > 0) torque = input * driveTorque;
                else if (input < 0) torque = input * brakeTorque;
                else torque = 0;

                // Placeholder calculation
                drivingForce = torque * curvature;
                resistanceForce = (rollingResistance + corneringResistance) * drivingForce;
                resultantForce = drivingForce - resistanceForce;
                lateralForce = resultantForce * Mathf.Sin(wheelSpeedDeflectionAngle * Mathf.Deg2Rad);
                wheelAcceleration = resultantForce * inverseVehicleMass;
                speed += input * wheelAcceleration * dt;
                if (speed < 0) speed = 0;

                velocity.x = speed * Mathf.Sin((globalWheelTurningAngle + wheelSpeedDeflectionAngle) * Mathf.Deg2Rad);
                velocity.z = speed * Mathf.Cos((globalWheelTurningAngle + wheelSpeedDeflectionAngle) * Mathf.Deg2Rad);

                // Prevents wheel from reversing when braking
                var dot = Vector3.Dot(transform.forward, velocity.normalized);
                if (dot < 0) velocity = Vector3.zero;
            }
        }

        float CalculateSlipAngle(float angle)
        {
            return 0.1f * angle;
        }
    }
}