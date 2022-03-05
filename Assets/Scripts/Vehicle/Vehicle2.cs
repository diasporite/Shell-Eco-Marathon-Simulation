using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    [RequireComponent(typeof(Rigidbody))]
    public class Vehicle2 : MonoBehaviour
    {
        public bool enableLift = false;
        public bool enableReaction = false;

        [Header("Constants")]
        public float bodyMass = 40;
        public float driverMass = 50;
        public float referenceArea = 0.39f;

        float wheelSeparation = 1f;
        [SerializeField] Vector3 centreOfMass;
        [SerializeField] Vector3 centreOfSteering;

        [Header("Coefficients")]
        [Range(0f, 1f)] public float liftCoefficent = 0.02f;
        [Range(0f, 1f)] public float dragCoefficent = 0.12f;

        [Header("Environment")]
        public float airDensity = 1.225f;

        [Header("Wheels")]
        public Wheel2 frontLeftWheel;
        public Wheel2 frontRightWheel;
        public Wheel2 backWheel;
        Wheel2[] wheels;

        [Header("Other Components")]
        public FuelCell fuelCell;
        public BoxCollider undercarriage;
        public GameObject vehicleBody;

        [Header("Variables - Vectors")]
        public Vector3 steerDir;
        public Vector3 velocity;
        public Vector3 groundVelocity;

        [Header("Variables - Speed")]
        public float speed = 0;
        public float groundSpeed = 0;
        public float distanceTravelled;

        [Header("Variables - Forces")]
        public float wheelDriveForce = 0;
        public float dragForce = 0;
        public float liftForce = 0;
        public float resultantDriveForce = 0;

        [Header("Variables - Acceleration")]
        public float driveAcceleration = 0;
        public float liftAcceleration = 0;

        [Header("Inputs")]
        [SerializeField] float steerInput = 0;
        [SerializeField] float accelerateInput = 0;
        [SerializeField] float brakeInput = 0;

        float gravity = 9.81f;

        Rigidbody rb;

        public Rigidbody Rb => rb;

        public float VehicleMass => bodyMass + driverMass + frontLeftWheel.mass +
            frontRightWheel.mass + backWheel.mass + fuelCell.TotalMass;

        public float InverseVehicleMass => 1 / VehicleMass;

        public bool Stationary => speed <= 0.01f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();

            fuelCell = GetComponent<FuelCell>();

            wheels = new Wheel2[] { frontLeftWheel, frontRightWheel, backWheel };

            centreOfMass = CalculateCOM(centreOfMass);

            centreOfSteering = 0.5f * (frontLeftWheel.transform.position +
                frontRightWheel.transform.position);

            wheelSeparation = (centreOfSteering - backWheel.transform.position).z;
        }

        private void Start()
        {
            rb.mass = VehicleMass;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.angularDrag = 0;

            undercarriage.size = new Vector3(0.1f, 0.25f, 0.1f);
        }

        private void Update()
        {
            GetInputs();
        }

        private void FixedUpdate()
        {
            Drive();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position + wheelSeparation * transform.forward,
                1f * transform.forward);

            if (rb != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position + centreOfMass, 2.5f * rb.velocity.normalized);
            }

            //Gizmos.color = Color.yellow;
            //Gizmos.DrawRay(transform.position, 2f * (transform.rotation * steerDir).normalized);
        }

        void GetInputs()
        {
            steerInput = Input.GetAxisRaw("Horizontal");

            if (Input.GetKey("j")) accelerateInput = 1;
            else accelerateInput = 0;

            if (Input.GetKey("l")) brakeInput = 1;
            else brakeInput = 0;
        }

        void Drive()
        {
            foreach (var wheel in wheels)
            {
                wheel.Steer(steerInput, Time.fixedDeltaTime);
                wheel.Accelerate(accelerateInput, brakeInput, Time.fixedDeltaTime);
            }

            steerDir = GetSteerDir();
            wheelDriveForce = GetResultantForce();

            rb.mass = VehicleMass;

            velocity = rb.velocity;
            speed = velocity.magnitude;

            groundVelocity = velocity;
            groundVelocity.y = 0;
            groundSpeed = velocity.magnitude;

            dragForce = 0.5f * airDensity * groundSpeed * groundSpeed * dragCoefficent * referenceArea;
            liftForce = 0.5f * airDensity * groundSpeed * groundSpeed * liftCoefficent * referenceArea;

            SteerVehicle(steerDir);
            AccelerateVehicle();
        }

        Vector3 GetSteerDir()
        {
            var steerDir = Vector3.zero;

            foreach (var wheel in wheels)
                if (wheel.steering)
                    //steerDir += wheel.transform.forward;
                    steerDir += Quaternion.Euler(0, transform.eulerAngles.y, 0 ) * wheel.LocalSteerDir;

            if (steerDir != Vector3.zero) return steerDir.normalized;

            return transform.forward;
        }

        float GetResultantForce()
        {
            var f = 0f;

            foreach (var wheel in wheels)
                f += wheel.resultantForce;

            return f;
        }

        void SteerVehicle(Vector3 steerDir)
        {
            float steerY = steerDir.x;
            var steerTorque = steerY * transform.up * wheelSeparation;
            rb.AddRelativeTorque(steerTorque, ForceMode.Force);

            // Apply a constant angular velocity
            //rb.angularVelocity = ... * Mathf.Deg2Rad;
        }

        void AccelerateVehicle()
        {
            resultantDriveForce = wheelDriveForce - dragForce;
            driveAcceleration = resultantDriveForce * InverseVehicleMass;

            liftAcceleration = liftForce * InverseVehicleMass;

            rb.AddRelativeForce(resultantDriveForce * transform.forward, ForceMode.Force);

            if (enableLift)
                rb.AddRelativeForce(liftForce * transform.up, ForceMode.Force);

            if (enableReaction && rb.useGravity)
                rb.AddRelativeForce(VehicleMass * 9.81f * transform.up, ForceMode.Force);

            //rb.AddRelativeForce(a * transform.forward * Time.fixedDeltaTime, ForceMode.VelocityChange);
            //rb.velocity += a * transform.forward;
        }

        Vector3 CalculateCOM(Vector3 com)
        {
            com = Vector3.zero;

            com += (bodyMass + driverMass + fuelCell.TotalMass) * vehicleBody.transform.localPosition;

            foreach (var wheel in wheels) com += wheel.mass * wheel.transform.localPosition;

            com *= InverseVehicleMass;

            return com;
        }
    }
}