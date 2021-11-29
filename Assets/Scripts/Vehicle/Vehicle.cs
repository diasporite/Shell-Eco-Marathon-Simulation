using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    [RequireComponent(typeof(Rigidbody))]
    public class Vehicle : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] float time;
        [SerializeField] float speed;
        [SerializeField] float distance;

        [Header("Constants")]
        public float topSpeed = 10;

        public float bodyMass;

        [Header("Components")]
        public Wheel frontWheel;
        public Wheel backWheel;

        public Accelerator accel;

        Rigidbody rb;

        public Rigidbody Rb => rb;

        public Vector3 Forward
        {
            get => transform.up;
            set => transform.up = value;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();

            accel = GetComponentInChildren<Accelerator>();
        }

        private void Start()
        {
            rb.mass = bodyMass;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.angularDrag = 0;
        }

        private void FixedUpdate()
        {
            time += Time.fixedDeltaTime;
            speed = rb.velocity.magnitude;
            distance += speed * Time.fixedDeltaTime;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, Forward);
        }
    }
}