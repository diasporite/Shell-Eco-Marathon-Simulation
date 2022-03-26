using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    public class GroundCheck : MonoBehaviour
    {
        public float radius = 0.01f;

        LayerMask ground;

        public bool IsGrounded => 
            Physics.CheckSphere(transform.position, radius, ground);

        private void Awake()
        {
            ground = LayerMask.GetMask("Ground");
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}