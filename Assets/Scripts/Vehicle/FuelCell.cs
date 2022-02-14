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
        public float fuelFlowRate;
        [Range(0f, 1f)]
        public float fuelCellEfficiency = 0.5f;
        public float power = 500;

        float fuelVolRate = 0.1042f;

        // Constants
        float mH2;
        float b;
        float lowerHeatingValue = 178.393f;
        float rhoH2 = 1f;

        public bool FuelEmpty => fuelMass <= 0;
        public float ConsumedFuel => fuelMass - currentFuelMass;
        public float TotalMass => cellMass + currentFuelMass;

        private void Start()
        {
            mH2 = -9f / 287f;
            b = 17552f / 287f;
            fuelCellEfficiency = 0.01f * (mH2 * power + b);
            fuelFlowRate = power / (60 * fuelCellEfficiency * lowerHeatingValue);

            currentFuelMass = fuelMass;
            currentFuelVolume = fuelVolume;
        }

        public void CalculateFuelUsage(string input, float dt)
        {
            if (input != "j") return;

            currentFuelMass -= fuelMassFlowRate * Time.deltaTime;
            if (currentFuelMass < 0) currentFuelMass = 0;
        }

        public float GetPower(float dt)
        {
            currentFuelMass -= fuelMassFlowRate * Time.deltaTime;
            if (currentFuelMass < 0) currentFuelMass = 0;

            return power;
        }
    }
}