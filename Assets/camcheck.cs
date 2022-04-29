using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    public class camcheck : MonoBehaviour
    {
        public GameObject lookat;
        public Vector3 Looking;
        CameraController2 cc;
        Quaternion NewRot;
        // Start is called before the first frame update
        void Start()
        {
            cc = lookat.GetComponent<CameraController2>();
        }

        // Update is called once per frame
        void Update()
        {
            NewRot.eulerAngles = cc.camLook;
            transform.LookAt(cc.camPos);
        }
    }
}
