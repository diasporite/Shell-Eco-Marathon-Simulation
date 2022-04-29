using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    public class CameraController2 : MonoBehaviour
    {
        public Transform follow;

        public float speed = 120f;
        [Range(-15, 15)]
        public float camDist = -100f;
        [Range(-15, 15)]
        public float camDist2 = 0.8f;

       public  float thetaY = 180f;
        float thetaXz = 15f;
        public Vector3 camPos = new Vector3(0, 0, 0);
        public Vector3 camLook = new Vector3(0, 0, 0);
        public Vector3 test;
        public Vector3 test2;
        private void Start()
        {
           
        }

        private void Update()
        {
            test = camPos - transform.position;
            RotateCamera();
            FollowTarget();
           
        }

       

        void RotateCamera()
        {
            //if (Input.GetKey("left")) thetaY += speed * Time.deltaTime;
            //if (Input.GetKey("right")) thetaY -= speed * Time.deltaTime;

            //if (Input.GetKey("up")) thetaXz += speed * Time.deltaTime;
            //if (Input.GetKey("down")) thetaXz -= speed * Time.deltaTime;

            //if (Mathf.Abs(thetaXz) > 20) thetaXz = 20 * Mathf.Sign(thetaXz);
            test2 = follow.rotation.eulerAngles;
            thetaY = test2.y ;
            //thetaY = speed * Time.deltaTime;

            thetaXz = follow.rotation.y;
            //t/hetaXz = speed * Time.deltaTime;

             thetaXz = 20 * Mathf.Sign(thetaXz);
        }

        void FollowTarget()
        {
            if (follow != null)
            {
                camPos.x = follow.position.x - camDist2   * Mathf.Sin(thetaY * Mathf.Deg2Rad);
                camPos.y = follow.position.y  ;
                camPos.z = follow.position.z - camDist2 * Mathf.Cos(thetaY * Mathf.Deg2Rad);

                camLook.x = follow.position.x - camDist * Mathf.Sin(thetaY * Mathf.Deg2Rad);
                camLook.y = follow.position.y  ;
                camLook.z = follow.position.z - camDist * Mathf.Cos(thetaY * Mathf.Deg2Rad);

                transform.position = camPos;
                transform.LookAt(camLook);
                // transform.rotation = follow.rotation;
                
            }
        }
    }
}