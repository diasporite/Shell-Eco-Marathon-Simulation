using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO;
using System;

namespace VirtualTwin
{
    public class Motor_Data : MonoBehaviour
    {
        public static Motor_Data instance;

        public float MaxSpeed = 1000f;

        public Vector2 motor_zero = new Vector2(0f,3.5f );
        public Vector2 motor_one = new Vector2(100f,3.43f);
        public Vector2 motor_two = new Vector2(200f, 3.37f);
        public Vector2 motor_three = new Vector2(300f, 3.23f);
        public Vector2 motor_four = new Vector2(400f, 3.13f);
        public Vector2 motor_five = new Vector2(500f, 2.97f);
        public Vector2 motor_six = new Vector2(600f, 2.76f);
        public Vector2 motor_seven = new Vector2(700f, 2.61f);
        public Vector2 motor_eight = new Vector2(800f, 2.47f);
        public Vector2 motor_nine = new Vector2(900f, 2.47f);
        public Vector2 motor_ten = new Vector2(1000f, 0f);

        public float cTorque;
        public float cRPM;
        public float pRPM;

        public Vehicle T;


        // Start is called before the first frame update
        void Awake()
        {
            instance = this;
        }

        // Update is called once per frame
        void Update()
        {
            //cRPM = T.CurrentRPM;
            //Debug.Log(cRPM);
            //pRPM = cRPM / T.TopRpm;
            if (pRPM >= 1)
            {
                cTorque = motor_ten.y;
            }
            else if (pRPM >= 0.9)
            {
                cTorque = motor_nine.y;
                Debug.Log("");
            }
            else if (pRPM >= 0.8)
            {
                cTorque = motor_eight.y;
            }
            else if (pRPM >= 0.7)
            {
                cTorque = motor_seven.y;
            }
            else if (pRPM >= 0.6)
            {
                cTorque = motor_six.y;
            }
            else if (pRPM >= 0.5)
            {
                cTorque = motor_five.y;
            }
            else if (pRPM >= 0.4)
            {
                cTorque = motor_four.y;
            }
            else if (pRPM >= 0.3)
            {
                cTorque = motor_three.y;
            }
            else if (pRPM >= 0.2)
            {
                cTorque = motor_two.y;
            }
            else if (pRPM >= 0.1)
            {
                cTorque = motor_one.y;
            }
            else if (pRPM >= 0)
            {
                cTorque = motor_zero.y;
                //Debug.Log(pRPM);
            }

        }
    }
}