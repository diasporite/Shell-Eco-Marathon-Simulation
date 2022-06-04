using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    public class CameraController : MonoBehaviour
    {
        public bool firstPersonMode = true;

        public Vehicle2 vehicle;
        public Transform follow;

        [Range(1, 10)]
        public float camDist = 5f;
        public float rotateSpeed = 120f;
        public float updateSpeed = 20f;

        float thetaY = 180f;
        float thetaXz = 10f;
        Vector3 camPos = new Vector3(0, 0, 0);

        private void Start()
        {
            if (firstPersonMode)
            {
                vehicle.vehicleBody.GetComponent<MeshRenderer>().enabled = false;
                FirstPersonFollow();
            }
            else FollowTarget();
        }

        private void Update()
        {
            if (!firstPersonMode) RotateCamera();
        }

        private void LateUpdate()
        {
            if (firstPersonMode) FirstPersonFollow();
            else FollowTarget();
        }

        void RotateCamera()
        {
            if (Input.GetKey("left")) thetaY += rotateSpeed * Time.deltaTime;
            if (Input.GetKey("right")) thetaY -= rotateSpeed * Time.deltaTime;

            if (Input.GetKey("up")) thetaXz += rotateSpeed * Time.deltaTime;
            if (Input.GetKey("down")) thetaXz -= rotateSpeed * Time.deltaTime;

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

            transform.position = Vector3.MoveTowards(transform.position, camPos, updateSpeed * Time.deltaTime);
            transform.LookAt(follow);
        }

        void LockedFollow()
        {
            camPos = follow.position - camDist * follow.forward;
            camPos.y = 3;

            transform.position = Vector3.MoveTowards(transform.position, camPos, updateSpeed * Time.deltaTime);
            transform.LookAt(follow);
        }

        void FirstPersonFollow()
        {
            transform.position = Vector3.MoveTowards(transform.position, follow.transform.position, updateSpeed * Time.deltaTime);
            transform.forward = follow.transform.forward;
        }

        void SwitchViewMode(bool firstPerson)
        {
            firstPersonMode = firstPerson;

            if (firstPerson)
                vehicle.vehicleBody.GetComponent<MeshRenderer>().enabled = false;
            else
                vehicle.vehicleBody.GetComponent<MeshRenderer>().enabled = true;
        }
    }
}