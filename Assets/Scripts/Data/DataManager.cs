using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    public class DataManager : MonoBehaviour
    {
        public Vehicle subject;

        [SerializeField] float timeElapsed = 0;
        [Range(1, 5)]
        [SerializeField] int sampleEveryFixedFrame = 1;
        int framesSinceLog = 0;

        [SerializeField] List<DataPoint> data = new List<DataPoint>();

        private void Start()
        {
            timeElapsed = 0;
        }

        private void FixedUpdate()
        {
            TickFixed();
        }

        void TickFixed()
        {
            timeElapsed += Time.fixedDeltaTime;

            framesSinceLog++;
            if (framesSinceLog >= sampleEveryFixedFrame)
            {
                LogData(subject.Speed, subject.Distance);
                framesSinceLog = 0;
            }
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