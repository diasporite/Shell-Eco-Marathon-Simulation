using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    [RequireComponent(typeof(Rigidbody))]
    public class Vehicle : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] float speed;
        [SerializeField] float distance;

        [Header("Constants")]
        public float topSpeed = 10;

        public float bodyMass = 200;
        public float rideHeight = 0.25f;

        [Range(0.001f, 0.1f)]
        public float stationaryThreshold = 0.04f;

        [Header("Wheels")]
        public Wheel frontWheel;
        public Wheel backWheel;

        [Header("Components")]
        public Accelerator accelerator;
        public Steering steering;
        public BoxCollider undercarriage;

        Rigidbody rb;

        Vector3 driveDir = new Vector3(0, 0, 1);

        public float Speed => speed;
        public float Distance => distance;

        public Rigidbody Rb => rb;

        public Vector3 DriveDir => frontWheel.Drive;

        public bool Stationary => speed <= stationaryThreshold;

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

            undercarriage.size = new Vector3(0.1f, rideHeight, 0.1f);
        }

        private void Update()
        {
            driveDir = CalcDriveDir();
        }

        private void FixedUpdate()
        {
            accelerator.Accelerate(driveDir);

            Drive();

            LogData();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, 2f * transform.forward);

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, 2f * driveDir);
        }

        void Drive()
        {
            transform.forward = driveDir;
        }

        void LogData()
        {
            speed = rb.velocity.magnitude;
            distance += speed * Time.fixedDeltaTime;
        }

        Vector3 CalcDriveDir()
        {
            return steering.SteerDir();
            //return transform.forward;
        }
    }
}