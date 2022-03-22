using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    [System.Serializable]
    public class DataPoint
    {
        Vehicle subject;
        Vehicle2 subject2;

        public float time;
        public float speed;
        public float distance;

        // acceleration
        public float acceleration;

        // drag
        public float vehicleDrag;

        // resistances
        // tyre forces
        public float wheelDrive;
        public float rollingRes;
        public float cornerRes;
        public float wheelTurnAngle;

        public float turnAngle;
        public float velAngle;
        public float angularVelocity;

        public float mass;
        public float fuelMass;

        public float fuelCellEfficiency;
        public float vehicleFuelEfficiency;

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
            fuelCellEfficiency = subject.FuelEfficiency;
        }

        public DataPoint(float time, Vehicle2 subject)
        {
            this.time = time;

            speed = subject.speed;
            distance = subject.distanceTravelled;
            acceleration = subject.resultantAcceleration;
            vehicleDrag = subject.dragForce;

            wheelDrive = subject.frontLeftWheel.drivingForce;
            rollingRes = subject.frontLeftWheel.rollingResForce;
            cornerRes = subject.frontLeftWheel.cornerResForce;
            wheelTurnAngle = subject.frontLeftWheel.steerAngle;

            turnAngle = subject.globalAngle;
            velAngle = subject.velocityAngle;
            angularVelocity = subject.angularVelocity;

            mass = subject.VehicleMass;
            fuelMass = subject.fuelCell.currentFuelMass;
            fuelCellEfficiency = subject.fuelCell.fuelCellEfficiency;
        }
    }
}