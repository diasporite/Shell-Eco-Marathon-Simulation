using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    [RequireComponent(typeof(Rigidbody))]
    public class Vehicle : MonoBehaviour
    {
        [Header("Constants")]
        public float topSpeed = 10;
        public float bodyLength = 2;
        public float length_fcm = 1;    // Length between front and centre of mass (l_f)
        public float length_bcm = 1;    // Length between back and centre of mass (l_r)
        public float bodyMass = 200;
        public float rideHeight = 0.25f;
        public float surfaceArea = 0.2f;

        [Range(0.001f, 0.1f)]
        public float stationaryThreshold = 0.04f;

        [Header("Coefficients")]
        [Range(0f, 1f)] public float liftCoefficent = 0.1f;
        [Range(0f, 1f)] public float dragCoefficent = 0.1f;

        [Header("Environment")]
        public float airDensity = 1000f;

        [Header("Components")]
        public Wheel frontWheel;
        public Wheel backWheel;
        public BoxCollider undercarriage;

        Rigidbody rb;

        [Header("Variables")]
        [SerializeField] float distance;

        [SerializeField] float speed;
        [SerializeField] Vector3 velocity = new Vector3(0, 0, 0);

        [SerializeField] float driveAcceleration = 0;
        [SerializeField] float dragAcceleration = 0;
        [SerializeField] float resultantAcceleration = 0;

        [SerializeField] float driveForce = 0;
        [SerializeField] float liftForce = 0;
        [SerializeField] float dragForce = 0;
        [SerializeField] float resultantForce = 0;

        [SerializeField] float driveAngle = 0;  // beta in vehicle dynamics doc
        [SerializeField] Vector3 driveVelocity = new Vector3(0, 0, 0);

        [SerializeField] float dv = 0;

        [Header("Input")]
        [SerializeField] float steerInput;
        [SerializeField] string keyInput;

        public float Speed => speed;
        public float Distance => distance;

        public Rigidbody Rb => rb;

        public float VehicleMass => bodyMass + frontWheel.mass + backWheel.mass;

        public float InverseVehicleMass => 1 / VehicleMass;

        public bool Stationary => speed <= stationaryThreshold;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();

            //accelerator = GetComponent<Accelerator>();
            //steering = GetComponent<Steering>();

            bodyLength = length_fcm + length_bcm;
        }

        private void Start()
        {
            rb.mass = VehicleMass;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.angularDrag = 0;

            undercarriage.size = new Vector3(0.1f, rideHeight, 0.1f);
        }

        private void Update()
        {
            GetInputs();
        }

        private void FixedUpdate()
        {
            Steer();
            Accelerate();

            //Drive();
            Drive2();
            //Drive3();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, 5f * transform.forward);

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, 5f * driveVelocity.normalized);
        }

        void GetInputs()
        {
            steerInput = Input.GetAxisRaw("Horizontal");

            if (Input.GetKey("j")) keyInput = "j";
            else if (Input.GetKey("l")) keyInput = "l";
            else keyInput = "";
        }

        void Steer()
        {
            frontWheel.SteerWheel(steerInput, Time.fixedDeltaTime);
        }

        void Accelerate()
        {
            var sign = 0;

            switch (keyInput)
            {
                case "j":
                    sign = 1;
                    break;
                case "l":
                    sign = -1;
                    break;
                default:
                    sign = 0;
                    break;
            }

            frontWheel.DriveWheel(sign, Time.fixedDeltaTime);
        }

        void Drive()
        {
            driveForce = frontWheel.ResultantForce;
            driveAcceleration = driveForce * InverseVehicleMass;

            dragForce = 0.5f * airDensity * speed * speed * surfaceArea;
            dragAcceleration = dragForce * InverseVehicleMass;

            resultantForce = driveForce - dragForce;
            resultantAcceleration = resultantForce * InverseVehicleMass;

            // Get initial values

            // Get velocity vector of front wheel
            var v = CalculateVelocity(frontWheel.Velocity);

            // Calculate velocity and angle of body
            driveVelocity = v;
            driveAngle = Vector3.SignedAngle(driveVelocity, transform.forward, transform.up);

            speed = driveVelocity.magnitude;

            // Account for drag (placeholder)
            //velocity *= (1 - dragCoefficent);
            //driveForce = frontWheel.ResultantForce;
            //driveAcceleration = driveForce * InverseVehicleMass;

            //dragForce = 0.5f * airDensity * speed * speed * dragCoefficient * surfaceArea;
            //dragAcceleration = dragForce * InverseVehicleMass;

            //resultantForce = driveForce - dragForce;
            //resultantAcceleration = resultantForce * InverseVehicleMass;

            // Apply to rb
            speed += resultantAcceleration * Time.fixedDeltaTime;
            if (speed < 0) speed = 0;
            rb.velocity = speed * driveVelocity.normalized;

            // Turn vehicle by small amount
            var delta = -driveAngle * Time.fixedDeltaTime;
            //print(driveAngle);
            transform.Rotate(0, delta, 0);

            distance += speed * Time.fixedDeltaTime;
        }

        void Drive2()
        {
            // Get current frame data
            speed = rb.velocity.magnitude;
            velocity = rb.velocity;

            driveVelocity = CalculateVelocity(frontWheel.Velocity);
            driveAngle = Vector3.SignedAngle(driveVelocity, transform.forward, transform.up);

            // Get resultant force from front wheel
            driveForce = frontWheel.ResultantForce;

            // Calculate drag (and lift)
            dragForce = 0.5f * airDensity * speed * speed * dragCoefficent * surfaceArea;

            // Calculate new frame data
            resultantForce = driveForce - dragForce;
            resultantAcceleration = resultantForce * InverseVehicleMass;

            dv = resultantAcceleration * Time.fixedDeltaTime;
            speed = speed + dv;
            if (speed < 0) speed = 0;
            distance += speed * Time.fixedDeltaTime;

            // Apply new frame data
            var v = speed * driveVelocity.normalized;
            rb.velocity = v;
            print(rb.velocity + " " + v);

            // Rotate vehicle
            var delta = -driveAngle * Time.fixedDeltaTime;
            transform.Rotate(0, delta, 0);
        }

        // Force based approach
        void Drive3()
        {
            // Get current frame data
            speed = rb.velocity.magnitude;
            velocity = CalculateVelocity(frontWheel.Velocity);

            // Get resultant force from front wheel
            driveForce = frontWheel.ResultantForce;

            // Calculate drag (and lift)
            dragForce = 0.5f * airDensity * speed * speed * dragCoefficent * surfaceArea;

            // Calculate new frame data
            resultantForce = driveForce - dragForce;
            resultantAcceleration = resultantForce * InverseVehicleMass;

            // Apply new frame data
            var v = driveVelocity.normalized;
            //rb.AddForce(resultantForce * v * Time.fixedDeltaTime, ForceMode.Impulse);
            //rb.velocity += resultantAcceleration * v * InverseVehicleMass * Time.fixedDeltaTime;
            rb.AddForce(resultantAcceleration * v * InverseVehicleMass * Time.fixedDeltaTime, ForceMode.VelocityChange);
            velocity = rb.velocity;

            // Rotate vehicle
            var delta = -driveAngle * Time.fixedDeltaTime;
            transform.Rotate(0, delta, 0);
        }

        Vector3 CalculateVelocity(Vector3 frontWheelVelocity)
        {
            var velocity = frontWheelVelocity;

            return speed * velocity;
        }
    }
}