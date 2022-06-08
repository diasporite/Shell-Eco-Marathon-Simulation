using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace VirtualTwin
{
    public enum SteerInput
    {
        _1D = 0,
        _2D = 1,
    }

    public class InputManager : MonoBehaviour
    {
        Vehicle2 vehicle;
        PlayerInput carControls;
        CameraController camControl;
        DataManager data;

        public SteerInput input;

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

        private void Awake()
        {
            vehicle = GetComponent<Vehicle2>();
            carControls = GetComponent<PlayerInput>();
            camControl = FindObjectOfType<CameraController>();
            data = FindObjectOfType<DataManager>();
        }

        public void OnSteer(InputValue value)
        {
            if (input == SteerInput._1D) steer = value.Get<float>();
        }

        public void OnAccelerate(InputValue value)
        {
            //accelerate = value.Get<float>();
            accelerate = Mathf.Clamp(value.Get<float>(), 0f, 1f);
        }

        public void OnBrake(InputValue value)
        {
            //brake = value.Get<float>();

            if (carControls.currentControlScheme == "Steering Wheel")
            {
                // Inverting the input
                brake = 1 - Mathf.Clamp(value.Get<float>(), 0f, 1f);
            }
            else brake = Mathf.Clamp(value.Get<float>(), 0f, 1f);
        }

        public void OnSteer2D(InputValue value)
        {
            if (input == SteerInput._2D)
            {
                steerDir = value.Get<Vector2>().normalized;
                steer = Mathf.Atan2(steerDir.x, steerDir.y) * InversePi;
            }
        }

        public void OnToggleRecording(InputValue value)
        {
            FindObjectOfType<DataManager>().Record();
            FindObjectOfType<RecordButton>().SwitchText();
        }

        public void OnFullscreen(InputValue value)
        {
            if (Screen.fullScreen) Screen.fullScreenMode = FullScreenMode.Windowed;
            else if (Screen.fullScreenMode == FullScreenMode.Windowed)
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }

        public void OnReset(InputValue value)
        {
            vehicle.ResetVehicle();
        }

        public void OnToggleView(InputValue value)
        {
            camControl.ToggleViewMode();
        }

        public void OnExport(InputValue value)
        {
            data.ExportData();
        }
    }
}