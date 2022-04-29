using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace VirtualTwin
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] float steer;

        [SerializeField] float accelerate;
        [SerializeField] float brake;

        public float Steer => steer;

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

        public void OnToggleRecording(InputValue value)
        {
            FindObjectOfType<DataManager>().Record();
            FindObjectOfType<RecordButton>().SwitchText();
        }
    }
}