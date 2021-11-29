using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    public class Steering : MonoBehaviour
    {
        public float steeringSpeed = 90;

        float steerLock = 360;

        float dy;
        Vector3 ds;

        Vehicle vehicle;

        Wheel frontWheel;
        Wheel backWheel;

        private void Awake()
        {
            vehicle = GetComponentInParent<Vehicle>();
        }

        private void Update()
        {
            //SimpleSteer();
        }

        public void SimpleSteer()
        {
            dy += steeringSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
            if (Mathf.Abs(dy) > steerLock) dy = steerLock * Mathf.Sign(dy);

            if (dy != 0 && vehicle.Stationary)
                vehicle.transform.rotation = Quaternion.Euler(0, dy, 90);
        }

        public void Steer()
        {

        }
    }
}