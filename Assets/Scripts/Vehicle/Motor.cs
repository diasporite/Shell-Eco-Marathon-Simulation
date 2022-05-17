using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    public class Motor : MonoBehaviour
    {
        public Vehicle2 vehicle;
        FuelCell fuelCell;

        [Header("Wheels")]
        public Wheel2 frontLeftWheel;
        public Wheel2 frontRightWheel;
        public Wheel2 backWheel;

        [Header("Engine Variables")]
        public float topRpm = 1000;

        public float currentTorque;
        public float currentRpm;
        public float pRpm;

        float gearRatio = 6f;
        float torqueCutoff = 3.57f;

        [Header("Requirements")]
        public float reqVoltage = 24;
        public float reqCurrent = 49.7f;
        public float reqPower;

        [Header("Constants")]
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

        [Header("Variables")]
        public float pTorque;
        public float energyConsumed;
        public float energyIn;

        public float motorOutEnergy;
        public float energyLossDrag;
        public float energyLossOther;
        public float energyLoss;
        public float deltaKe;
        public float usefulEnergy;

        const float RPM_TO_RADPS = Mathf.PI / 30f;

        private void Awake()
        {
            vehicle = GetComponent<Vehicle2>();
            fuelCell = GetComponent<FuelCell>();

            reqPower = reqVoltage * reqCurrent;
            currentTorque = torqueCutoff;
            kv = 1 / ke;

            topRpm = reqPower / 8.69f;

            energyConsumed = 0;
            energyLossDrag = 0;
            motorOutEnergy = 0;
        }

        public void CalculateData(float dt)
        {
            currentRpm = vehicle.speed / (gearRatio * frontLeftWheel.radius * RPM_TO_RADPS);
            currentRpm = Mathf.Min(currentRpm, topRpm);
            pRpm = currentRpm * gearRatio / topRpm;

            xl = currentRpm * RPM_TO_RADPS * inductance * p;
            trueVoltage = (reqVoltage - (ke * currentRpm * RPM_TO_RADPS)) / 1.2f;

            currentTorque = kt * reqCurrent * gearRatio;
            pTorque = currentTorque / gearRatio;

            reqPower = reqVoltage * reqCurrent;
            outputPower = currentTorque * currentRpm * RPM_TO_RADPS;
            transientEfficiency = outputPower / reqPower;

            if (vehicle.AccelerateInput > 0) energyIn = reqPower * dt;
            energyConsumed += energyIn;
            energyLossDrag += vehicle.dragForce * vehicle.speed * dt;
            energyLossOther += vehicle.wheelBrakeForce * vehicle.speed * dt;
            motorOutEnergy += outputPower * dt;

            usefulEnergy = transientEfficiency * energyConsumed;
        }
    }
}