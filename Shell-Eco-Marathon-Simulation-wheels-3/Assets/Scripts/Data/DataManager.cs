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

        public Vehicle subject;
        public Text DistText;
        public HUD hud;
        public Text TextObj;
        public bool writetime= false;
        [SerializeField] float timeElapsed = 0;
        [Range(1, 5)]
        [SerializeField] int sampleEveryFixedFrame = 1;
        int framesSinceLog = 0;
        public GameObject DistanceText;
        public float DistTest;
        [SerializeField] public List<DataPoint> data = new List<DataPoint>();
        public string filename = "Output-Name";

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
         //   DistText = DistanceText.GetInstanceID();   
            DistText = DistanceText.GetComponent<UnityEngine.UI.Text>();
            timeElapsed = 0;

            //DistanceText = DistanceText.GetComponent<UnityEngine.UI.Text>();
           
        }
        public void ButtonCheck2()
        {
            if (State.Instance.state == 0)
            {
                filename = InputMet.Specific.FN;

            }
        }
        public void FixedUpdate()
        { 
            
            //Debug.Log("test"+subject.distance);
           
            TickFixed();
           
            if(subject.distance>1000)
            {
                Write_Time();
                writetime = true;
              

            }
            else
            {
                writetime = false;

            }
           
           
            DistText.text = DistTest.ToString();
            DistTest = subject.distance;
        }
        public void TickFixed()
        {
            timeElapsed += Time.fixedDeltaTime;

            if (recording)
            {
                framesSinceLog++;
                if (framesSinceLog >= sampleEveryFixedFrame)
                {
                    LogData(timeElapsed,subject.Speed, subject.Distance,subject.acceleration_check, Motor_Data.instance.pRPM, subject.Torque_Output, Motor_Data.instance.cRPM, Motor_Data.instance.OutputPower, subject.Efficency, Motor_Data.instance.Energy_In, Motor_Data.instance.Useful_Energy, Motor_Data.instance.Energy_Lost_Drag, Motor_Data.instance.Energy_Lost_Electricity, Motor_Data.instance.Energy_Lost);
          //          hud.UpdateUI(data.Count, LastSample);
                    framesSinceLog = 0;
                }
            }
        }

        public void StartRecording()
        {
            data.Clear();

            recording = true;
        }

        public void StopRecording()
        {
            recording = false;
        }

       //public void LogData(float speed, float distance)
        //{
       //    data.Add(new DataPoint(timeElapsed, speed, distance));
        //}
        public void Write_Time()
        {
            string path = PathMethod();
            StreamWriter writer = new StreamWriter(path);
            writer.WriteLine("Time,Speed,Distance,acceleration,force,torque,rpm,power,efficency,EnergyIn,EnergyUseful,lost to drag,lost to electric effects,EnergyLost");

            for (int i = 0; i < data.Count; ++i)
            {
                DataPoint test = data[i];
                writer.WriteLine(test._time.ToString() + "," + test._speed.ToString() + "," + test._distance.ToString() + "," + test._acceleration.ToString() + "," + test._force.ToString() + "," + test._torque.ToString() + "," + test._rpm.ToString() + "," + test._power.ToString() + "," + test._efficency.ToString() + "," + test._EnergyIn.ToString() + "," + test._UsefulEnergy.ToString() + "," + test._EnergyLostD.ToString() + "," + test._EnergyLostE.ToString() + "," + test._EnergyLost.ToString());
            }
            writer.Flush();
            writer.Close();

        }
        public void LogData(float time, float speed, float distance,float acceleration,float force,float torque, float rpm, float power,float efficency,float EnIn,float EnUse, float EnLostD, float EnLostE,float EnLost)
        {
            data.Add(new DataPoint(time, speed, distance,acceleration,force,torque,rpm, power,efficency,EnIn,EnUse,EnLostD,EnLostE,EnLost));
        }
        public void OnApplicationQuit()
        {
            if (!writetime)
             {
                Write_Time();
             }
        }
        private string PathMethod()
        {
            return Application.dataPath +"/"+ filename +".csv";
        }
    }
}