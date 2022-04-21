using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine_Script : MonoBehaviour
{
    public float MaxRpm;
    public float Torque_Output;
    public float Input_Voltage;
    public float Resistance;
    public float Input_Current;
    public float Power;
    public float MechPower;
    public float Losses;
    public float CurrentRPM;

    // Start is called before the first frame update
    void Start()
    {
        Input_Voltage = 500f; //same voltage
        Input_Current = 2f; //same Current
        Power = Input_Voltage * Input_Current;
        Losses = 0;
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}