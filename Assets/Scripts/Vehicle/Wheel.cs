using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    [RequireComponent(typeof(Rigidbody))]
    public class Wheel : MonoBehaviour
    {
        public GameObject model;

        public bool driving = true;

        [Header("Dimensions")]
        public float mass = 5;
        public float radius = 1;

        [Header("Power")]
        public float torque = 1;
        public float rollingRes = 0.5f;

        [Header("Orientation")]
        // Rotation about axis, in degrees
        public float toe = 0;
        public float camber = 0;

        Vehicle vehicle;

        Rigidbody rb;
        WheelCollider wc;

        public Vector3 Forward
        {
            get => transform.forward;
            set => transform.forward = value;
        }

        // Returns direction of travel of wheel
        public Vector3 Drive
        {
            get
            {
                if (driving) return Forward;
                return vehicle.Forward;
            }
        }

        private void Awake()
        {
            vehicle = GetComponentInParent<Vehicle>();

            InitRb();
            InitWc();
        }

        void InitRb()
        {
            rb = GetComponent<Rigidbody>();

            rb.mass = mass;
        }

        void InitWc()
        {
            wc = GetComponentInChildren<WheelCollider>();

            wc.mass = mass;
            wc.radius = radius;
        }
    }
}