using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    public class FuelCell : MonoBehaviour
    {
        public float fuelMass = 2;
        public float energyDensity = 4200;
        public float fuelMassFlowRate = 0.05f;
        [Range(0f, 1f)]
        public float fuelEfficiency = 0.5f;

        public bool FuelEmpty => fuelMass <= 0;

        public void CalculateFuelUsage(string input, float dt)
        {
            if (input != "j") return;

            fuelMass -= fuelMassFlowRate * Time.deltaTime;
            if (fuelMass < 0) fuelMass = 0;
        }
    }
}