using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO;
using System;

namespace VirtualTwin
{
    public class DataManager : MonoBehaviour
    {
        [SerializeField] bool recording = false;

        public Vehicle2 subject;
        public HUD hud;

        [SerializeField] float timeElapsed = 0;
        [Range(1, 5)]
        [SerializeField] int sampleEveryFixedFrame = 1;
        int framesSinceLog = 0;
        [SerializeField] List<DataPoint> data = new List<DataPoint>();

        bool writetime = false;
        public string filename = "Output-Name";

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
            LogData(subject);
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
                string path = PathMethod();
                StreamWriter writer = new StreamWriter(path);
                writer.WriteLine(DataHeader);

                for (int i = 0; i < data.Count; ++i)
                {
                    DataPoint test = data[i];
                    writer.WriteLine(test.time + "," + test.x + "," + test.z + "," + 
                        test.speed + "," + test.distance.ToString() + "," + 
                        test.acceleration + "," + test.rollingRes + "," + test.h2Consumption + 
                        "," + test.currentTorque + "," + test.currentRpm); 
                }

                writer.Flush();
                writer.Close();
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

        public void LogData(Vehicle2 subject)
        {
            //data.Add(new DataPoint(timeElapsed, subject.Speed, subject.Distance, 
            //    subject.Acceleration, subject.Drag));
            data.Add(new DataPoint(timeElapsed, subject));
        }

        public void LogData(float speed, float distance)
        {
            data.Add(new DataPoint(timeElapsed, speed, distance));
        }

        public void LogData(float time, float speed, float distance)
        {
            data.Add(new DataPoint(time, speed, distance));
        }

        public void OnApplicationQuit()
        {
            if (!writetime)
            {
                // we can have another write method here for when the sim is closed?
                //ExportData();
            }
        }

        private string PathMethod()
        {
            return Application.dataPath + "/" + filename + ".csv";
        }

        string DataHeader => "Time (s),Position X, Position Z,Speed (m/s),Distance (m),Acceleration (m/s2)," +
            "Rolling Resistance (N),H2 Consumption (L/min),Torque (Nm),Motor RPM";

    }
}