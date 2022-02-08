using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    [System.Serializable]
    public class DataPoint
    {
        Vehicle subject;

        [SerializeField] float time;
        [SerializeField] float speed;
        [SerializeField] float distance;

        // acceleration
        [SerializeField] float acceleration;

        // drag
        [SerializeField] float vehicleDrag;

        // resistances
        // tyre forces
        [SerializeField] float wheelDrive;
        [SerializeField] float rollingRes;

        [SerializeField] float mass;
        [SerializeField] float fuelMass;

        [SerializeField] float fuelEfficiency;

        public float Time => time;
        public float Speed => speed;
        public float Distance => distance;
        public float Acceleration => acceleration;
        public float VehicleDrag => vehicleDrag;

        public float Mass => mass;
        public float FuelMass => fuelMass;
        public float FuelEfficiency => fuelEfficiency;

        public DataPoint(float t, float v, float s)
        {
            time = t;
            speed = v;
            distance = s;
        }

        public DataPoint(float t, float v, float s, float a, float fd)
        {
            time = t;
            speed = v;
            distance = s;
            acceleration = a;
            vehicleDrag = fd;
        }

        public DataPoint(float time, Vehicle subject)
        {
            this.time = time;

            speed = subject.Speed;
            distance = subject.Distance;
            acceleration = subject.Acceleration;
            vehicleDrag = subject.Drag;

            wheelDrive = subject.frontLeftWheel.WheelDrive;
            rollingRes = subject.frontLeftWheel.ResistanceForce;

            mass = subject.VehicleMass;
            fuelMass = subject.fuelCell.currentFuelMass;
            fuelEfficiency = subject.FuelEfficiency;
        }
    }
}