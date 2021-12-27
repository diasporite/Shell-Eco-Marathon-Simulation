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

        [SerializeField] Vector3 contactPoint;

        Vehicle vehicle;

        public Vector3 ContactPoint => contactPoint;

        private void Awake()
        {
            vehicle = GetComponentInParent<Vehicle>();

            contactPoint = transform.position + radius * Vector3.down;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, 1f * transform.forward);
        }
    }
}