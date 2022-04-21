using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    [System.Serializable]
    public class DataPoint
    {
        [SerializeField] float time;
        [SerializeField] float speed;
        [SerializeField] float distance;
        [SerializeField] float acceleration;
        [SerializeField] float force;
        [SerializeField] float torque;
        [SerializeField] float rpm;
        [SerializeField] float power;
        [SerializeField] float efficency;
        [SerializeField] float UsefulEnergy;
        [SerializeField] float EnergyIn;
        [SerializeField] float EnergyLostDrag;
        [SerializeField] float EnergyLostElec;
        [SerializeField] float EnergyLost;


        public float _time => time;
        public float _speed => speed;
        public float _distance => distance;
        public float _acceleration => acceleration;
        public float _force => force;
        public float _torque => torque;
        public float _rpm => rpm;
        public float _power => power;
        public float _efficency => efficency;
        public float _UsefulEnergy => UsefulEnergy;
        public float _EnergyIn => EnergyIn;
        public float _EnergyLost => EnergyLost;
        public float _EnergyLostE => EnergyLostElec;
        public float _EnergyLostD => EnergyLostDrag;

        public DataPoint(float t, float v, float s, float a, float f,float to,float r, float p,float eff,float EnIn, float Use, float LostDrag, float LostElec, float Lost)
        {
            time = t;
            speed = v;
            distance = s;
            acceleration = a;
            force = f;
            torque = to;
            rpm = r;
            power = p;
            efficency = eff;
            UsefulEnergy = Use;
            EnergyIn = EnIn;
            EnergyLostDrag = LostDrag;
            EnergyLostElec = LostElec;
            EnergyLost = Lost;
            //power = p;
        }
    }
}