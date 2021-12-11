using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    public class DataManager : MonoBehaviour
    {
        [SerializeField] List<DataPoint> data = new List<DataPoint>();

        public void LogData(float time, float speed, float distance)
        {
            data.Add(new DataPoint(time, speed, distance));
        }
    }
}