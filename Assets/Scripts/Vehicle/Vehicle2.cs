using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    [RequireComponent(typeof(Rigidbody))]
    public class Vehicle2 : MonoBehaviour
    {
        [Header("Settings")]
        public bool enableLift = false;
        public bool enableReaction = false;

        [Header("Constants")]
        public float bodyMass = 40;
        public float driverMass = 50;
        public float frontalArea = 0.39f;

        [Header("Dimensions")]
        [SerializeField] Vector3 centreOfMass;
        [SerializeField] Vector3 centreOfSteering;

        float wheelSeparation = 1f;
        float rearToCoM;
        float frontToCoM;

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
        public Vector3 motionCentre;    // Centre of circular motion
        public Vector3 dirOfCircularMotion;

        [Header("Variables - Speed")]
        public float speed = 0;
        public float groundSpeed = 0;

        [Header("Variables - Linear Displacement")]
        public float distanceTravelled;
        public float turningRadius;

        [Header("Variables - Angular Displacement")]
        public float circleAngle;
        public float angularVelocity;
        public float vehicleRotation;

        [Header("Variables - Forces")]
        public float wheelDriveForce = 0;
        public float dragForce = 0;
        public float liftForce = 0;
        public float resultantDriveForce = 0;

        [Header("Variables - Lateral Forces")]
        public float corneringResistanceForce = 0;
        public float centripetalForce = 0;
        public float lateralForce = 0;
        public float tensionForce = 0;
        public float resultantLateralForce = 0;

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

            centreOfMass = GetCentreOfMass(centreOfMass);

            centreOfSteering = 0.5f * (frontLeftWheel.transform.position +
                frontRightWheel.transform.position);

            wheelSeparation = (centreOfSteering - backWheel.transform.position).z;

            rearToCoM = centreOfMass.z;
            frontToCoM = centreOfSteering.z - centreOfMass.z;
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

            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position + centreOfSteering, 
                5f * Mathf.Sign(frontLeftWheel.steerAngle) * frontLeftWheel.transform.right);
            Gizmos.DrawRay(backWheel.transform.position, 5f * backWheel.transform.right);
            Gizmos.DrawWireSphere(transform.position + motionCentre, 0.2f);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position + centreOfMass, 4f * dirOfCircularMotion);
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

            corneringResistanceForce = GetCorneringResistance();

            rb.mass = VehicleMass;

            velocity = rb.velocity;
            speed = velocity.magnitude;

            CalculateCentreOfMotion();

            groundVelocity = velocity;
            groundVelocity.y = 0;
            groundSpeed = groundVelocity.magnitude;

            dragForce = 0.5f * airDensity * groundSpeed * groundSpeed * dragCoefficent * frontalArea;
            liftForce = 0.5f * airDensity * groundSpeed * groundSpeed * liftCoefficent * frontalArea;

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

        float GetCorneringResistance()
        {
            var res = 0f;

            foreach (var wheel in wheels)
                res += wheel.cornerResForce * Mathf.Sin(wheel.steerAngle);

            return res;
        }

        void CalculateCentreOfMotion()
        {
            var delta = frontLeftWheel.steerAngle;

            if (delta != 0)
            {
                var r0 = wheelSeparation / Mathf.Sin(delta * Mathf.Deg2Rad);
                motionCentre = centreOfSteering + r0 * frontLeftWheel.transform.right;

                dirOfCircularMotion = (motionCentre - centreOfMass).normalized;
                dirOfCircularMotion.y = 0;
                turningRadius = dirOfCircularMotion.magnitude;

                centripetalForce = VehicleMass * groundSpeed * groundSpeed / turningRadius;
                lateralForce = centripetalForce * Mathf.Cos(Mathf.Atan(dirOfCircularMotion.z / dirOfCircularMotion.x));
                tensionForce = centripetalForce * Mathf.Sin(Mathf.Atan(dirOfCircularMotion.z / dirOfCircularMotion.x));
            }
        }

        void SteerVehicle(Vector3 steerDir)
        {
            // [3]
            vehicleRotation = Mathf.Atan(rearToCoM * frontLeftWheel.steerAngle * Mathf.Deg2Rad / wheelSeparation) * Mathf.Rad2Deg;
            rb.MoveRotation(Quaternion.Euler(0, vehicleRotation, 0));
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

        Vector3 GetCentreOfMass(Vector3 com)
        {
            com = Vector3.zero;

            com += (bodyMass + driverMass + fuelCell.TotalMass) * vehicleBody.transform.localPosition;

            foreach (var wheel in wheels) com += wheel.mass * wheel.transform.localPosition;

            com *= InverseVehicleMass;

            return com;
        }
    }
}