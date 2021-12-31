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
        public float torque = 1;
        [Range(0f, 1f)] public float resistance = 0.5f;

        [Header("Constants")]
        public float wheelLock = 45f;

        [Header("Variables")]
        [SerializeField] float wheelTurningAngle = 0;
        [SerializeField] float slipAngle = 0;
        [SerializeField] float wheelSpeedDeflectionAngle = 0;
        [SerializeField] float acceleration = 0;
        [SerializeField] Vector3 velocity = new Vector3(0, 0, 0);

        Vehicle vehicle;

        float curvature;
        float inverseVehicleMass;

        public Vector3 ContactPoint => contactPoint;

        public float WheelTurningAngle => wheelTurningAngle;
        public float SlipAngle => slipAngle;
        public float WheelSpeedDeflectionAngle => wheelSpeedDeflectionAngle;
        public Vector3 Velocity => velocity;

        private void Awake()
        {
            vehicle = GetComponentInParent<Vehicle>();

            contactPoint = transform.position + radius * Vector3.down;

            curvature = 1 / radius;
        }

        private void Start()
        {
            inverseVehicleMass = 1 / vehicle.VehicleMass;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, 5f * transform.forward);
        }

        public void SteerWheel(float delta)
        {
            if (driving)
            {
                // Turn wheel
                wheelTurningAngle += 60 * delta;
                if (Mathf.Abs(wheelTurningAngle) > wheelLock)
                    wheelTurningAngle = wheelLock * Mathf.Sign(wheelTurningAngle);

                slipAngle = CalculateSlipAngle(wheelTurningAngle);
                wheelSpeedDeflectionAngle = wheelTurningAngle - slipAngle;

                transform.localRotation = Quaternion.Euler(0, wheelTurningAngle, 0);
            }
        }

        public void DriveWheel(float dt)
        {
            if (driving)
            {
                var force = (1 - resistance) * torque * curvature;
                acceleration = force * inverseVehicleMass;

                velocity += acceleration * transform.forward * dt;
            }
        }

        float CalculateSlipAngle(float angle)
        {
            return 0.1f * angle;
        }
    }
}