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
        public float bodyLength = 2.54f;
        public float length_fcm = 1;    // Length between front and centre of mass (l_f)
        public float length_bcm = 1;    // Length between back and centre of mass (l_r)
        public float bodyMass = 40;
        public float driverMass = 50;
        public float rideHeight = 0.25f;
        public float referenceArea = 0.39f;
        public float wheelSeparation = 1f;

        [Range(0.001f, 0.1f)]
        public float stationaryThreshold = 0.04f;

        [Header("Coefficients")]
        [Range(0f, 1f)] public float liftCoefficent = 0.1f;
        [Range(0f, 1f)] public float dragCoefficent = 0.1f;

        [Header("Environment")]
        public float airDensity = 1.225f;

        [Header("Components")]
        public Wheel frontLeftWheel;
        public Wheel frontRightWheel;
        public Wheel backWheel;
        public FuelCell fuelCell;
        public BoxCollider undercarriage;
        public Wheel[] wheels;

        [SerializeField] Vector3 centreOfSteering;

        Rigidbody rb;

        [Header("Variables")]
        [SerializeField] float distance;

        [SerializeField] float speed;
        [SerializeField] float dv = 0;
        [SerializeField] Vector3 velocity = new Vector3(0, 0, 0);

        [SerializeField] float driveAcceleration = 0;
        [SerializeField] float dragAcceleration = 0;
        [SerializeField] float resultantAcceleration = 0;

        [SerializeField] float driveForce = 0;
        [SerializeField] float liftForce = 0;
        [SerializeField] float dragForce = 0;
        [SerializeField] float resultantForce = 0;

        [SerializeField] float driveAngle = 0;  // beta in vehicle dynamics doc
        [SerializeField] Vector3 driveDir = new Vector3(0, 0, 0);

        [SerializeField] float fuelEfficiency = 0;

        [Header("Input")]
        [SerializeField] float steerInput;
        [SerializeField] string keyInput;

        public float Speed => speed;
        public float Distance => distance;
        public float Acceleration => resultantAcceleration;
        public float Drag => dragForce;
        public float FuelEfficiency => fuelCell.ConsumedFuel / distance;

        public Rigidbody Rb => rb;

        public float VehicleMass => bodyMass + driverMass + frontLeftWheel.mass + 
            frontRightWheel.mass + backWheel.mass + fuelCell.TotalMass;

        public float InverseVehicleMass => 1 / VehicleMass;

        public bool Stationary => speed <= stationaryThreshold;

        private void Awake()
        {
            fuelCell = GetComponent<FuelCell>();

            rb = GetComponent<Rigidbody>();

            wheels = new Wheel[] { frontLeftWheel, frontRightWheel, backWheel };

            centreOfSteering = 0.5f * (frontLeftWheel.transform.position + frontRightWheel.transform.position);
            wheelSeparation = (centreOfSteering - backWheel.transform.position).z;
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
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, 5f * transform.forward);

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, 5f * driveDir.normalized);
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
            foreach(var w in wheels)
                if (w.steering)
                    w.SteerWheel(steerInput, Time.fixedDeltaTime);
        }

        void Accelerate()
        {
            var sign = 0;

            if (!fuelCell.FuelEmpty)
            {
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
            }

            foreach(var w in wheels)
                if (w.driving)
                    w.DriveWheel(sign, Time.fixedDeltaTime);
        }

        void Drive()
        {
            rb.mass = VehicleMass;

            // Get current frame data
            velocity = rb.velocity;

            driveDir = CalculateSteerDir(frontLeftWheel.Velocity).normalized;
            driveAngle = Vector3.SignedAngle(driveDir, transform.forward, transform.up);

            // Get resultant force from front wheel
            if (fuelCell.FuelEmpty) driveForce = 0;
            else driveForce = GetWheelDrive();

            // Calculate drag (and lift)
            dragForce = 0.5f * airDensity * speed * speed * dragCoefficent * referenceArea;
            liftForce = 0.5f * airDensity * speed * speed * liftCoefficent * referenceArea;

            // Calculate new frame data
            resultantForce = driveForce - dragForce;
            resultantAcceleration = resultantForce * InverseVehicleMass;

            dv = resultantAcceleration * Time.fixedDeltaTime;
            //print(speed + " + " + dv + " = " + (speed + dv));
            speed += dv;
            if (speed < 0) speed = 0;
            distance += speed * Time.fixedDeltaTime;

            // Apply new frame data
            var v = speed * driveDir.normalized;
            v.y = rb.velocity.y;    // Conserve y velocity for gravity
            rb.velocity = v;
            //print(rb.velocity + " " + v);


            // Rotate vehicle




            // Accelerate vehicle


            // Rotate vehicle
            var delta = -driveAngle * Time.fixedDeltaTime;
            transform.Rotate(0, delta, 0);

            //fuelCell.CalculateFuelUsage(keyInput, Time.fixedDeltaTime);
        }

        void Drive2()
        {
            rb.mass = VehicleMass;

            // Get current frame data
            velocity = rb.velocity;

            driveDir = CalculateSteerDir(frontLeftWheel.Velocity).normalized;
            driveAngle = Vector3.SignedAngle(driveDir, transform.forward, transform.up);

            // Get resultant force from front wheel
            if (fuelCell.FuelEmpty) driveForce = 0;
            else driveForce = GetWheelDrive();

            // Calculate drag (and lift)
            dragForce = 0.5f * airDensity * speed * speed * dragCoefficent * referenceArea;
            liftForce = 0.5f * airDensity * speed * speed * liftCoefficent * referenceArea;

            // Calculate new frame data
            resultantForce = driveForce - dragForce;
            resultantAcceleration = resultantForce * InverseVehicleMass;

            SteerVehicle();

            AccelerateVehicle();

            //fuelCell.CalculateFuelUsage(keyInput, Time.fixedDeltaTime);
        }

        float GetWheelDrive()
        {
            float drive = 0;

            foreach(var w in wheels)
                if (w.driving)
                    drive += w.ResultantForce;
            print("drive " + drive);
            return drive;
        }

        Vector3 CalculateSteerDir(Vector3 frontWheelVelocity)
        {
            Vector3 velocity = Vector3.zero;

            foreach (var w in wheels)
                if (w.steering)
                    velocity += w.Velocity.normalized;

            velocity.Normalize();

            return speed * velocity;
        }



        void SteerVehicle()
        {
            var steerX = driveDir.x;
            var steerTorque = steerX * transform.up * wheelSeparation;
            print(12);
            rb.AddRelativeTorque(steerTorque);
        }

        void AccelerateVehicle()
        {
            dv = resultantAcceleration * Time.fixedDeltaTime;
            speed += dv;
            if (speed < 0) speed = 0;
            distance += speed * Time.fixedDeltaTime;

            rb.AddRelativeForce(dv * transform.forward, ForceMode.VelocityChange);
        }
    }
}