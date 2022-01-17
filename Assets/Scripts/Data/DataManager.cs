using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    public class DataManager : MonoBehaviour
    {
        [SerializeField] bool recording = false;

        public Vehicle subject;
        public HUD hud;

        [SerializeField] float timeElapsed = 0;
        [Range(1, 5)]
        [SerializeField] int sampleEveryFixedFrame = 1;
        int framesSinceLog = 0;

        [SerializeField] List<DataPoint> data = new List<DataPoint>();

        public bool Recording => recording;

        public DataPoint LastSample
        {
            get
            {
                if (data.Count > 0)
                    return data[data.Count - 1];
                return null;
            }
        }

        private void Start()
        {
            recording = false;

            timeElapsed = 0;
        }

        private void FixedUpdate()
        {
            TickFixed();
        }

        void TickFixed()
        {
            if (recording)
            {
                timeElapsed += Time.fixedDeltaTime;
                framesSinceLog++;

                if (framesSinceLog >= sampleEveryFixedFrame)
                    LogSubjectData();
            }
        }

        void LogSubjectData()
        {
            LogData(subject.Speed, subject.Distance);
            hud.UpdateUI(data.Count, LastSample);
            framesSinceLog = 0;
        }

        public void Record()
        {
            if (!recording) StartRecording();
            else StopRecording();
        }

        public void ExportData()
        {
            if (!recording)
            {
                // Export code goes here
            }
        }

        public void StartRecording()
        {
            data.Clear();

            timeElapsed = 0;

            LogSubjectData();

            recording = true;
        }

        public void StopRecording()
        {
            recording = false;
        }

        public void LogData(float speed, float distance)
        {
            data.Add(new DataPoint(timeElapsed, speed, distance));
        }

        public void LogData(float time, float speed, float distance)
        {
            data.Add(new DataPoint(time, speed, distance));
        }
    }
}