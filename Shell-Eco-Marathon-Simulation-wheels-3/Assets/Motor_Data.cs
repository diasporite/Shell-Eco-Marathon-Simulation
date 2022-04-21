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
        public float p = 16;
        //public Vector2 motor_zero = new Vector2(0f,3.5f );
        //public Vector2 motor_one = new Vector2(100f,3.43f);
        //public Vector2 motor_two = new Vector2(200f, 3.37f);
        //public Vector2 motor_three = new Vector2(300f, 3.23f);
        //public Vector2 motor_four = new Vector2(400f, 3.13f);
        //public Vector2 motor_five = new Vector2(500f, 2.97f);
        //public Vector2 motor_six = new Vector2(600f, 2.76f);
        //public Vector2 motor_seven = new Vector2(700f, 2.61f);
        //public Vector2 motor_eight = new Vector2(800f, 2.47f);
        //public Vector2 motor_nine = new Vector2(900f, 2.47f);
        //public Vector2 motor_ten = new Vector2(1000f, 0f);
        public float cTorque;
        public float cRPM;
        public float pRPM;
        public Vehicle T;
        public float GEAR_RATIO = 6f;
        public float TopRpm = 10300f;
        public float TORQUE_CUTOFF = 3.57f;
        public float MaxTorque;
        public float ReqVolt = 24f;
        public float ReqCurrent = 49.7f;
        public float K_t = 0.076f;
        public float K_e = 0.015f;
        public float K_v;
        public float R = 0.035f;
        public float trueVoltage;
        public float XL;
        public float inductance = 0.0000658f;
        public float ReqPower;
        public float OutputPower;
        public float ConsumedEnergy;
        public float TransientEfficency;
        public float PTorque;
        public float Energy_In;
        public float Motor_Out_Energy=0f;
        public float Energy_Lost_Drag=0f;
        public float Energy_Lost_Electricity;
        public float Energy_Lost_Other=0f;
        public float Energy_Lost = 0f;
        public float Change_in_KE;
        public float Useful_Energy = 0f;

        public Wheel frontWheel;
        public Wheel backWheel;

        // Start is called before the first frame update
        void Awake()
        {
            T = GetComponentInParent<Vehicle>();
            frontWheel = T.frontWheel;
            backWheel = T.backWheel;
            instance = this;
            ReqPower = ReqVolt * ReqCurrent;
            //cTorque = TORQUE_CUTOFF;
            //inductance=inductance
            K_v = 1 / K_e;
            Energy_In = 0;
        }
        public void ButtonCheck()
        {
            if(State.Instance.state==0)
            {
                GEAR_RATIO = InputMet.Specific.NumericalValue;
                K_t = InputMet.Specific.Kt;
                K_e = InputMet.Specific.Ke;
                R = InputMet.Specific.R;
               inductance = InputMet.Specific.I;
                //As in Micro-H
                inductance = inductance / (1e6f);
             
            }
        }

        // Update is called once per frame
        void Update()
        {
            cRPM = T.CurrentRPM * GEAR_RATIO;
            // Debug.Log(cRPM);
            cRPM = Mathf.Min(cRPM, TopRpm);
            pRPM = cRPM / (TopRpm/GEAR_RATIO);
            XL = ((cRPM )/60)*2*Mathf.PI * inductance*p;
            trueVoltage = (ReqVolt - (K_e * (cRPM / 60) *2 * Mathf.PI))/1.2f;
            ReqCurrent =(((trueVoltage) /(Mathf.Sqrt(Mathf.Pow(XL,2)+ Mathf.Pow(R,2)))));
            // if (pRPM <= 0.3f)
            //{
            //    //
            //       cTorque = ((-4.347f*Mathf.Pow((pRPM),3f))-(-9.7826f* Mathf.Pow((pRPM), 2f)) +(7.0217f*pRPM)+1.5f)*GEAR_RATIO ;
            //    //cTorque = 200f;
            //}
            //else
            //{
            //    cTorque = (0.775f * Mathf.Log( pRPM) + 3.5161f)*GEAR_RATIO;
            //    //cTorque = 200f;

            //}
            // cTorque = (30 / (K_v * (Mathf.PI))) * ((Mathf.Sqrt(3 / 4) * ReqVolt) - (((cTorque / K_t) * (cRPM * 2 * Mathf.PI) / 60)* (cRPM * 2 * Mathf.PI) / 60) - (cTorque * K_v / K_t)) - cRPM;
            cTorque = K_t*ReqCurrent * GEAR_RATIO;
            PTorque = cTorque / GEAR_RATIO;
            ReqPower = ReqVolt * ReqCurrent;
            OutputPower = cTorque * T.CurrentRPM * (Mathf.PI / 30); 
            TransientEfficency = OutputPower / ReqPower;
            Energy_In += Mathf.Abs(ReqPower * Time.deltaTime);
            Motor_Out_Energy = OutputPower * Time.deltaTime;
            // Change_in_KE=((T.acceleration_check)*T.bodyMass)*T.speed;
            Energy_Lost_Drag += T.speed * Time.deltaTime * (frontWheel.Rolling_Resistance_Value + frontWheel.Air_Resistance_Value);
            if (T.ispulsing)
            {
                Energy_Lost_Electricity += ((ReqPower-(((cTorque / GEAR_RATIO) * (T.CurrentRPM * GEAR_RATIO *(Mathf.PI/30))))) * Time.deltaTime)-(T.speed * Time.deltaTime * (frontWheel.Rolling_Resistance_Value + frontWheel.Air_Resistance_Value));
            }
            else
            {
                Motor_Out_Energy=0;
            }
            Useful_Energy += Mathf.Max(0, Motor_Out_Energy);

           
           
            Energy_Lost = Energy_Lost_Electricity + Energy_Lost_Drag;

        }
    }
}