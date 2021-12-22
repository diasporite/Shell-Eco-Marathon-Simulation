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

        public float bodyMass = 200;
        public float rideHeight = 0.25f;

        [Header("Wheels")]
        public Wheel frontWheel;
        public Wheel backWheel;

        [Header("Components")]
        public Accelerator accelerator;
        public Steering steering;
        public BoxCollider undercarriage;

        Rigidbody rb;

        Vector3 driveDir;

        public Rigidbody Rb => rb;

        public Vector3 DriveDir => frontWheel.Drive;

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

            undercarriage.size = new Vector3(0.25f, rideHeight, 0.25f);
        }

        private void Update()
        {
            driveDir = CalcDriveDir();

            //Drive();
        }

        private void FixedUpdate()
        {
            accelerator.Accelerate(driveDir);

            LogData();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.forward);
        }

        void LogData()
        {
            time += Time.fixedDeltaTime;
            speed = rb.velocity.magnitude;
            distance += speed * Time.fixedDeltaTime;
        }

        Vector3 CalcDriveDir()
        {
            return transform.forward;
        }
    }
}