using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace VirtualTwin
{
    public class InputManager : MonoBehaviour
    {
        public float angle;
        [SerializeField] float steer;
        
        [SerializeField] float accelerate;
        [SerializeField] float brake;

        [SerializeField] Vector2 steerDir;

        const float InversePi = 1 / Mathf.PI;

        public float Steer => steer;
        public Vector2 SteerDir => steerDir;

        public float Accelerate => accelerate;
        public float Brake => brake;

        public void OnSteer(InputValue value)
        {
            steer = value.Get<float>();
        }

        public void OnAccelerate(InputValue value)
        {
            accelerate = value.Get<float>();
        }

        public void OnBrake(InputValue value)
        {
            brake = value.Get<float>();
        }

        public void OnSteer2D(InputValue value)
        {
            steerDir = value.Get<Vector2>().normalized;
            steer = Mathf.Atan2(steerDir.x, steerDir.y) * InversePi;
        }

        public void OnToggleRecording(InputValue value)
        {
            FindObjectOfType<DataManager>().Record();
            FindObjectOfType<RecordButton>().SwitchText();
        }
    }
}