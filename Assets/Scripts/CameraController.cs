﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    public class CameraController : MonoBehaviour
    {
        public Transform follow;

        public float speed = 120f;
        [Range(1, 10)]
        public float camDist = 5f;

        float thetaY = 180f;
        float thetaXz = 10f;
        Vector3 camPos = new Vector3(0, 0, 0);

        private void Start()
        {
            FollowTarget();
        }

        private void Update()
        {
            RotateCamera();
        }

        private void LateUpdate()
        {
            FollowTarget();
        }

        void RotateCamera()
        {
            print(5);
            if (Input.GetKey("left")) thetaY += speed * Time.deltaTime;
            if (Input.GetKey("right")) thetaY -= speed * Time.deltaTime;

            if (Input.GetKey("up")) thetaXz += speed * Time.deltaTime;
            if (Input.GetKey("down")) thetaXz -= speed * Time.deltaTime;

            if (Mathf.Abs(thetaXz) > 20) thetaXz = 20 * Mathf.Sign(thetaXz);
        }

        void FollowTarget()
        {
            if (follow != null)
            {
                //FreeFollow();
                LockedFollow();
            }
        }

        void FreeFollow()
        {
            camPos.x = follow.position.x + camDist * (-follow.forward.x + Mathf.Sin(thetaY * Mathf.Deg2Rad));
            camPos.y = follow.position.y + camDist * (-follow.forward.y + Mathf.Sin(thetaXz * Mathf.Deg2Rad));
            camPos.z = follow.position.z + camDist * (-follow.forward.z + Mathf.Cos(thetaY * Mathf.Deg2Rad));

            transform.position = camPos;
            transform.LookAt(follow);
        }

        void LockedFollow()
        {
            camPos = follow.position - camDist * follow.forward;
            camPos.y = 3;

            transform.position = camPos;
            transform.LookAt(follow);
        }
    }
}