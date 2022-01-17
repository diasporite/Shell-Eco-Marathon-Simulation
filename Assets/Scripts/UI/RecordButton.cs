using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VirtualTwin
{
    public class RecordButton : MonoBehaviour
    {
        [SerializeField] DataManager data;
        Text text;

        private void Awake()
        {
            data = FindObjectOfType<DataManager>();
            text = GetComponentInChildren<Text>();
        }

        private void Start()
        {
            text.text = "Start Recording";
        }

        public void SwitchText()
        {
            var recording = data.Recording;

            if (recording) text.text = "Stop Recording";
            else text.text = "Start Recording";
        }
    }
}