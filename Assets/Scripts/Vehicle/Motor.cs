using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    public class Motor : MonoBehaviour
    {
        public Vehicle2 vehicle;
        FuelCell fuelCell;

        public Wheel frontLeftWheel;
        public Wheel frontRightWheel;
        public Wheel backWheel;

        public float currentTorque;
        public float currentRpm;
        public float topRpm;
        public float pRpm;

        float gearRatio = 6f;
        float torqueCutoff = 3.57f;

        public float reqVoltage = 24;
        public float reqCurrent = 49.7f;
        public float reqPower;

        public float p = 16;
        public float kt = 0.076f;
        public float ke = 0.015f;
        public float kv;
        public float r = 0.035f;

        public float trueVoltage;
        public float xl;

        public float inductance = 0.0000658f;

        public float outputPower;
        public float consumedEnergy;
        public float transientEfficiency;

        public float pTorque;
        public float energyIn;

        public float motorOutEnergy;
        public float energyLossDrag;
        public float energyLossOther;
        public float energyLoss;
        public float deltaKe;
        public float usefulEnergy;

        private void Awake()
        {
            vehicle = GetComponent<Vehicle2>();
            fuelCell = GetComponent<FuelCell>();

            reqPower = reqVoltage * reqCurrent;
            currentTorque = torqueCutoff;
            kv = 1 / ke;

            energyIn = 0;
            energyLossDrag = 0;
            motorOutEnergy = 0;
        }

        public void CalculateData(float dt)
        {
            currentRpm = Mathf.Min(currentRpm, topRpm);
            pRpm = currentRpm * gearRatio / topRpm;

            xl = (currentRpm / 60) * 2 * Mathf.PI * inductance * p;
            trueVoltage = (reqVoltage - (ke * (currentRpm / 60) * 2 * Mathf.PI)) / 1.2f;

            currentTorque = kt * reqCurrent * gearRatio;
            pTorque = currentTorque / gearRatio;

            reqPower = reqVoltage * reqCurrent;
            transientEfficiency = outputPower / reqPower;

            energyIn += reqPower * dt;
            energyLossDrag += vehicle.dragForce * vehicle.speed * dt;
            motorOutEnergy += outputPower * dt;
        }
    }
}