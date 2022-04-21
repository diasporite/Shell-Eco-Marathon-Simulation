using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    [RequireComponent(typeof(Rigidbody))]
    public class Vehicle : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] public float speed;
        [SerializeField] public float distance;
        public bool ispulsing = false;
        public float Torque_Output;
        public float InputPower;
        public float CurrentRPM;
        public float Efficency;
        public float TopRpm;


        [Header("Constants")]
        public float topSpeed = 10;
        public float bodyLength = 2;
        public float length_fcm = 1;    // Length between front and centre of mass (l_f)
        public float length_bcm = 1;    // Length between back and centre of mass (l_r)
        public float bodyMass = 95;
        public float rideHeight = 0.25f;
        public float pulseSpeed = 10;
        public float coastSpeed = 20;
        public float acceleration_check = 0;
        public float force_check = 0;
        public float TopTorque = 240f;
        //public float MaxRpm;

        public float Input_Voltage = 500f;
        public float Resistance = 0f;
        public float Input_Current = 1f;


        public float Losses = 0f;


        [Range(0.001f, 0.1f)]
        public float stationaryThreshold = 0.04f;

        [Header("Coefficients")]
        [Range(0f, 1f)] public float liftCoefficent = 0.1f;
        [Range(0f, 1f)] public float dragCoefficent = 0.1f;

        [Header("Components")]
        public Wheel frontWheel;
        public Wheel backWheel;
        //public Accelerator accelerator;
        //public Steering steering;
        public BoxCollider undercarriage;
        Vector2 dist;
        Rigidbody rb;

        [Header("Variables")]
        [SerializeField] float driveAngle = 0;  // beta in vehicle dynamics doc
        [SerializeField] Vector3 velocity = new Vector3(0, 0, 0);

        public float Speed => speed;
        public float Distance => distance;

        public Rigidbody Rb => rb;

        public float VehicleMass => bodyMass + frontWheel.mass + backWheel.mass;

        public bool Stationary => speed <= stationaryThreshold;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();

            //accelerator = GetComponent<Accelerator>();
            //steering = GetComponent<Steering>();

            bodyLength = length_fcm + length_bcm;
        }
        public void ButtonCheck()
        {
            if (State.Instance.state == 0)
            {
                pulseSpeed = InputMet.Specific.NumericalValueP;
                coastSpeed = InputMet.Specific.NumericalValueC;
            }
        }

        private void Start()
        {
            Torque_Output = 500f;
            rb.mass = bodyMass;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.angularDrag = 0;
            dist = new Vector2(0f, 0f);
            //TopRpm = ((topSpeed / ((2 * Mathf.PI) * frontWheel.radius)));
            undercarriage.size = new Vector3(0.1f, rideHeight, 0.1f);
        }
        private void CheckPulse()
        {

            if (speed > coastSpeed) ispulsing = false;
            if (speed < pulseSpeed) ispulsing = true;
            //Debug.Log(ispulsing);
        }
        private void Update()
        {
            //velocity = CalcDriveDir();

            //accelerator.Accelerate();
            //accelerator.Acelerate(velocity);
            // EngineScript();
            CheckPulse();
            Steer();
            Accelerate();
        }

        private void FixedUpdate()
        {
            Drive();

            LogData();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, 5f * transform.forward);

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, 5f * velocity.normalized);
        }

        void Steer()
        {
            var input = Input.GetAxisRaw("Horizontal");
            frontWheel.SteerWheel(input * Time.deltaTime);
        }

        void Accelerate()
        {
            CheckPulse();

            frontWheel.DriveWheel(ispulsing, Time.deltaTime);
        }

        void Drive()
        {
            //transform.forward = driveDir;
            //Torque_Output=
            // Get velocity of front wheel
            var v = frontWheel.velocity;
            
            // Calculate velocity and angle of body
            velocity = CalculateVelocity(v);
            CurrentRPM = (60*velocity.magnitude / (2*Mathf.PI*frontWheel.radius));
            driveAngle = Vector3.SignedAngle(velocity, transform.forward, transform.up);
            Torque_Output = Motor_Data.instance.cTorque;
            InputPower = Motor_Data.instance.OutputPower;
            Efficency = Motor_Data.instance.TransientEfficency;
            // Account for drag (placeholder)
            //  velocity *= (1 - dragCoefficent);

            // Apply to rb
            rb.velocity = velocity;


            // Turn vehicle by small amount
            var delta = -driveAngle * Time.fixedDeltaTime;
            //print(driveAngle);
            transform.Rotate(0, delta, 0);
            acceleration_check = frontWheel.acceleration;
            force_check = frontWheel.force;
        }

        void LogData()
        {
            dist.x = (transform.position.x);
            dist.y = transform.position.z;
            speed = rb.velocity.magnitude;
            distance = dist.magnitude;
        }


        Vector3 CalculateVelocity(Vector3 frontWheelVelocity)
        {
            return frontWheelVelocity;
        }
    }
}
