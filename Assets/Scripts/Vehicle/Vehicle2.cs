using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    [RequireComponent(typeof(Rigidbody))]
    public class Vehicle2 : MonoBehaviour
    {
        [Header("Settings")]
        public bool enableDrag = true;
        public bool enableLift = false;
        public bool enableReaction = false;

        [Header("Constants")]
        public float bodyMass = 40;
        public float driverMass = 50;
        public float frontalArea = 0.39f;

        [Header("Dimensions")]
        [SerializeField] Vector3 relCentreOfMass;
        [SerializeField] Vector3 relCentreOfSteering;
        public Vector3 centreOfMass;
        public Vector3 centreOfSteering;

        [SerializeField] float wheelSeparation = 1f;
        [SerializeField] float rearToCoM;
        [SerializeField] float frontToCoM;

        [SerializeField] bool grounded;

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
        public GameObject vehicleBody;
        public FuelCell fuelCell;
        public BoxCollider undercarriage;

        [Header("Variables - Vectors")]
        public Vector3 velocity;
        public Vector3 velocityDir;
        public Vector3 motionCentre;        // Centre of circular motion at centre of mass
        public Vector3 dirOfCircularMotion;

        [Header("Variables - Speed")]
        public float speed = 0;

        [Header("Variables - Linear")]
        public float distanceTravelled;
        public float turningRadius;

        [Header("Variables - Rotation")]
        public float rearAngularVelocity;   // Speed at which back wheel rotates wheen steered
        public float angularVelocity;       // Speed at which vehicle CoM rotates when steered
        public float velocityAngle;         // Angle between vehicle axis (CoM) and direction of velocity
        public float globalAngle;           // Angle between vehicle axis (CoM) and world z axis

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
        public float resultantAcceleration = 0;
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

            relCentreOfMass = GetRelCentreOfMass();

            relCentreOfSteering = 0.5f * (frontLeftWheel.transform.localPosition +
                frontRightWheel.transform.localPosition);

            wheelSeparation = (relCentreOfSteering - backWheel.transform.position).z;

            rearToCoM = relCentreOfMass.z - backWheel.transform.localPosition.z;
            frontToCoM = relCentreOfSteering.z - relCentreOfMass.z;
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

            Gizmos.color = Color.red;
            //Gizmos.DrawRay(centreOfSteering + 0.25f * Vector3.up, 10f * Mathf.Sign(frontLeftWheel.steerAngle) * 
            //    frontLeftWheel.transform.right);
            Gizmos.DrawWireSphere(centreOfSteering, 0.15f);

            if (rb != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(centreOfMass, 2.5f * rb.velocity.normalized);
            }

            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(centreOfMass, 4f * dirOfCircularMotion);
            Gizmos.DrawWireSphere(motionCentre, 0.4f);

            Gizmos.color = Color.white;
            Gizmos.DrawRay(centreOfMass, 3f * velocityDir);
        }

        void GetInputs()
        {
            steerInput = Input.GetAxisRaw("Horizontal");

            if (Input.GetKey("j")) accelerateInput = 1;
            else accelerateInput = 0;

            if (Input.GetKey("l")) brakeInput = 1;
            else brakeInput = 0;
        }

        bool IsGrounded()
        {
            foreach (var wheel in wheels)
                if (!wheel.groundCheck.IsGrounded) return false;

            return true;
        }

        void Drive()
        {
            foreach (var wheel in wheels)
            {
                wheel.Steer(steerInput, Time.fixedDeltaTime);
                wheel.Accelerate(accelerateInput, brakeInput, Time.fixedDeltaTime);
            }

            wheelDriveForce = GetResultantForce();

            corneringResistanceForce = GetCorneringResistance();

            rb.mass = VehicleMass;

            velocity = rb.velocity;
            velocity.y = 0;
            speed = velocity.magnitude;

            grounded = IsGrounded();

            CalculateCentreOfMass();
            CalculateCentreOfSteering();
            //CalculateCentreOfMotion();

            dragForce = 0.5f * airDensity * speed * speed * dragCoefficent * frontalArea;
            liftForce = 0.5f * airDensity * speed * speed * liftCoefficent * frontalArea;

            //AccelerateVehicle();
            SteerVehicle();
            AccelerateVehicle();
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

        void CalculateCentreOfMass()
        {
            //centreOfMass.x = (transform.position + relCentreOfMass.x * transform.right).x;
            //centreOfMass.y = (transform.position + relCentreOfMass.y * transform.up).y;
            //centreOfMass.z = (transform.position + relCentreOfMass.z * transform.forward).z;

            centreOfMass = transform.position + (transform.rotation * relCentreOfMass);
        }

        void CalculateCentreOfSteering()
        {
            //centreOfSteering.x = (transform.position + relCentreOfSteering.x * transform.right).x;
            //centreOfSteering.y = (transform.position + relCentreOfSteering.y * transform.up).y;
            //centreOfSteering.z = (transform.position + relCentreOfSteering.z * transform.forward).z;

            //centreOfSteering = transform.position + (transform.rotation * relCentreOfSteering);
            //centreOfSteering = 0.5f * (frontLeftWheel.transform.position + frontRightWheel.transform.position);
            centreOfSteering = Vector3.Lerp(frontLeftWheel.transform.position, frontRightWheel.transform.position, 0.5f);
        }

        void CalculateCentreOfMotion()
        {
            var zeta = frontLeftWheel.steerAngle;

            if (zeta != 0)
            {
                var r0 = wheelSeparation / Mathf.Tan(zeta * Mathf.Deg2Rad);
                motionCentre = transform.position + r0 * transform.right;
                turningRadius = r0 / Mathf.Cos(velocityAngle * Mathf.Deg2Rad);

                dirOfCircularMotion = (motionCentre - centreOfMass).normalized;
                dirOfCircularMotion.y = 0;

                centripetalForce = VehicleMass * speed * speed / turningRadius;
                lateralForce = centripetalForce * Mathf.Cos(Mathf.Atan(dirOfCircularMotion.z / dirOfCircularMotion.x));
                tensionForce = centripetalForce * Mathf.Sin(Mathf.Atan(dirOfCircularMotion.z / dirOfCircularMotion.x));
            }
        }

        void SteerVehicle()
        {
            var zeta = frontLeftWheel.steerAngle;
            globalAngle = transform.eulerAngles.y;

            var tan_zeta = Mathf.Tan(zeta * Mathf.Deg2Rad);

            // [3, 4]           
            velocityAngle = Mathf.Atan(rearToCoM * tan_zeta / wheelSeparation) * Mathf.Rad2Deg;

            var cos_velAngle = Mathf.Cos(velocityAngle * Mathf.Deg2Rad);

            rearAngularVelocity = speed / wheelSeparation;
            angularVelocity = speed * tan_zeta * cos_velAngle * Mathf.Rad2Deg / wheelSeparation;

            globalAngle += angularVelocity * Time.fixedDeltaTime;
            rb.rotation = Quaternion.Euler(0, globalAngle, 0);

            velocityDir.x = Mathf.Sin((globalAngle + velocityAngle) * Mathf.Deg2Rad);
            velocityDir.z = Mathf.Cos((globalAngle + velocityAngle) * Mathf.Deg2Rad);

            if (tan_zeta != 0 && cos_velAngle != 0)
            {
                var r0 = wheelSeparation / tan_zeta;
                turningRadius = r0 / cos_velAngle;
                motionCentre = backWheel.transform.position + Mathf.Sign(zeta) * 
                    r0 * backWheel.transform.right;
                dirOfCircularMotion = (motionCentre - centreOfMass).normalized;
                dirOfCircularMotion.y = 0;
            }
        }

        void AccelerateVehicle()
        {
            if (enableDrag) resultantDriveForce = wheelDriveForce - dragForce;
            else resultantDriveForce = wheelDriveForce;

            resultantAcceleration = resultantDriveForce * InverseVehicleMass;

            liftAcceleration = liftForce * InverseVehicleMass;

            speed += resultantDriveForce * InverseVehicleMass * Time.fixedDeltaTime;
            //rb.velocity = speed * velocityDir.normalized;
            rb.velocity = speed * transform.forward;

            if (enableLift)
                rb.AddRelativeForce(liftForce * transform.up, ForceMode.Force);

            if (enableReaction && rb.useGravity && grounded)
                    rb.AddRelativeForce(VehicleMass * 9.81f * transform.up, ForceMode.Force);
        }

        void CalculateVariables()
        {

        }

        void ApplyVariables()
        {

        }

        Vector3 GetRelCentreOfMass()
        {
            var com = Vector3.zero;

            com += (bodyMass + driverMass + fuelCell.TotalMass) * vehicleBody.transform.localPosition;

            foreach (var wheel in wheels) com += wheel.mass * wheel.transform.localPosition;

            com *= InverseVehicleMass;

            return com;
        }
    }
}