using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    public class FuelCell : MonoBehaviour
    {
        public float cellMass = 3.3f;
        public float fuelMass = 0.6f;
        public float fuelVolume;
        public float currentFuelMass;
        public float currentFuelVolume;
        public float energyDensity = 4200;
        public float fuelMassFlowRate = 0.05f;
        public float fuelVolFlowRate;
        [Range(0f, 1f)]
        public float fuelCellEfficiency = 0.5f;
        public float power = 500;

        public float energyIn;
        public float consumedH2_lpm;

        float fuelVolRate = 0.1042f;

        // Constants
        float mH2;
        float b;
        float lowerHeatingValueH2 = 2973.216667f;   //178.393Wmin/L
        float rhoH2 = 1f;
        float consumptionPerWatt = 0.01172f;

        Motor motor;

        public bool FuelEmpty => fuelMass <= 0;
        public float ConsumedFuel => fuelMass - currentFuelMass;
        public float TotalMass => cellMass + currentFuelMass;

        private void Awake()
        {
            mH2 = -9f / 287f;
            b = 17552f / 287f;
            fuelCellEfficiency = 0.01f * (mH2 * power + b);
            fuelVolFlowRate = power / (60 * fuelCellEfficiency * lowerHeatingValueH2);

            currentFuelMass = fuelMass;
            currentFuelVolume = fuelVolume;

            motor = GetComponent<Motor>();
        }

        public void CalculateFuelUsage(float accelerateInput, float dt)
        {
            if (accelerateInput <= 0)
            {
                energyIn = 0f;
                consumedH2_lpm = 0f;
                return;
            }

            //energyIn = motor.energyIn;
            consumedH2_lpm = (consumptionPerWatt * motor.outputPower + 0.03f);

            fuelVolFlowRate = consumedH2_lpm / 60000f;
            fuelMassFlowRate = fuelVolFlowRate * rhoH2;

            currentFuelMass -= fuelMassFlowRate * dt;
            currentFuelMass = Mathf.Clamp(currentFuelMass, 0, fuelMass);

            //fuelCellEfficiency = motor.outputPower / (fuelVolFlowRate * lowerHeatingValueH2);
            fuelCellEfficiency = 0.01f * (-0.0023f * motor.outputPower + 50.514f);    // From 2021 report
        }

        public float GetPower(float dt)
        {
            currentFuelMass -= fuelMassFlowRate * Time.deltaTime;
            if (currentFuelMass < 0) currentFuelMass = 0;

            return power;
        }

        /// Notes
        /// From 2021 report
        /// - H2 consumption linear w/ power (5.86l/min, 500W) 
        ///   -> 0.000089333kg/s per 500W
        ///   -> 0.0000001786667kg/Ws
        /// - y intercept of ~0.03l/min
        ///   -> 0.0000005kg/Ws
    }
}