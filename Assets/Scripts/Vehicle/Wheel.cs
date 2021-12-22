using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
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

        Collider col;

        // Returns direction of travel of wheel
        public Vector3 Drive
        {
            get
            {
                if (driving) return transform.forward;
                return vehicle.transform.forward;
            }
        }

        private void Awake()
        {
            vehicle = GetComponentInParent<Vehicle>();

            InitCol();
        }

        void InitCol()
        {
            col = GetComponentInChildren<Collider>();
        }
    }
}