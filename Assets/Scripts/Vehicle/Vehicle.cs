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
        public float driverMass = 60;
        public float rideHeight = 0.25f;
        public float referenceArea = 1f;

        [Range(0.001f, 0.1f)]
        public float stationaryThreshold = 0.04f;

        [Header("Coefficients")]
        [Range(0f, 1f)] public float liftCoefficent = 0.1f;
        [Range(0f, 1f)] public float dragCoefficent = 0.1f;

        [Header("Environment")]
        public float airDensity = 1000f;

        [Header("Components")]
        public Wheel frontLeftWheel;
        public Wheel frontRightWheel;
        public Wheel backWheel;
        public FuelCell fuelCell;
        public BoxCollider undercarriage;
        public Wheel[] wheels;

        Rigidbody rb;

        [Header("Variables")]
        [SerializeField] float distance;

        [SerializeField] float speed;
        [SerializeField] Vector3 velocity = new Vector3(0, 0, 0);
        [SerializeField] float dv = 0;

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

        [Header("Current Variables")]
        [SerializeField] float currentSpeed = 0;
        [SerializeField] float currentResAcceleration = 0;
        [SerializeField] float currentDriveForce = 0;
        [SerializeField] float currentDragForce = 0;
        [SerializeField] float currentResForce = 0;
        [SerializeField] float currentLiftForce = 0;

        [Header("New Variables")]
        [SerializeField] float newSpeed = 0;
        [SerializeField] float newResAcceleration = 0;
        [SerializeField] float newDriveForce = 0;
        [SerializeField] float newDragForce = 0;
        [SerializeField] float newResForce = 0;
        [SerializeField] float newLiftForce = 0;

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
            backWheel.mass + fuelCell.currentFuelMass;

        public float InverseVehicleMass => 1 / VehicleMass;

        public bool Stationary => speed <= stationaryThreshold;

        private void Awake()
        {
            fuelCell = GetComponent<FuelCell>();
            wheels = GetComponentsInChildren<Wheel>();
            rb = GetComponent<Rigidbody>();
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

            Drive2();
            //Drive4();
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
            frontLeftWheel.SteerWheel(steerInput, Time.fixedDeltaTime);
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

            frontLeftWheel.DriveWheel(sign, Time.fixedDeltaTime);
        }

        void Drive2()
        {
            rb.mass = VehicleMass;

            // Get current frame data
            velocity = rb.velocity;

            driveDir = CalculateVelocity(frontLeftWheel.Velocity).normalized;
            driveAngle = Vector3.SignedAngle(driveDir, transform.forward, transform.up);

            // Get resultant force from front wheel
            if (fuelCell.FuelEmpty) driveForce = 0;
            else driveForce = frontLeftWheel.ResultantForce;

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
            rb.velocity = v;
            //print(rb.velocity + " " + v);

            // Rotate vehicle
            var delta = -driveAngle * Time.fixedDeltaTime;
            transform.Rotate(0, delta, 0);

            fuelCell.CalculateFuelUsage(keyInput, Time.fixedDeltaTime);
        }

        float GetWheelDrive()
        {
            float drive = 0;

            foreach(var w in wheels)
                if (w.driving)
                    drive += Mathf.Abs(w.ResultantForce);

            return drive;
        }

        Vector3 CalculateVelocity(Vector3 frontWheelVelocity)
        {
            var velocity = frontWheelVelocity;

            return speed * velocity;
        }
    }
}