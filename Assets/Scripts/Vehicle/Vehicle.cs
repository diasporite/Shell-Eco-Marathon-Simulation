using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    [RequireComponent(typeof(Rigidbody))]
    public class Vehicle : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] float time;
        [SerializeField] float speed;
        [SerializeField] float distance;

        [Header("Constants")]
        public float topSpeed = 10;

        public float bodyMass;
        public float rideHeight;

        [Header("Components")]
        public Wheel frontWheel;
        public Wheel backWheel;

        public Accelerator accelerator;
        public Steering steering;

        Rigidbody rb;

        Vector3 driveDir;

        public Rigidbody Rb => rb;

        public Vector3 Forward
        {
            get => transform.up;
            set => transform.up = value;
        }

        public Vector3 DriveDir
        {
            get
            {
                return frontWheel.Drive;
            }
        }

        public bool Stationary => rb.velocity.sqrMagnitude <= 0.12f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();

            accelerator = GetComponent<Accelerator>();
            steering = GetComponent<Steering>();
        }

        private void Start()
        {
            rb.mass = bodyMass;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.angularDrag = 0;

            rb.MovePosition(transform.position + rideHeight * Vector3.up);
        }

        private void Update()
        {
            Drive();
        }

        private void FixedUpdate()
        {
            LogData();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, Forward);
        }

        void Drive()
        {
            //driveDir = steering.SteerDir();
            steering.Steer();
            accelerator.Accelerate();
        }

        void LogData()
        {
            time += Time.fixedDeltaTime;
            speed = rb.velocity.magnitude;
            distance += speed * Time.fixedDeltaTime;
        }
    }
}