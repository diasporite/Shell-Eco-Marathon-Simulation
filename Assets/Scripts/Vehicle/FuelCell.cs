using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualTwin
{
    public class FuelCell : MonoBehaviour
    {
        public float fuelMass = 10;
        public float energyDensity = 4200;
        [Range(0f, 1f)]
        public float fuelEfficiency = 0.5f;

        [SerializeField] float fuelUsage = 0;

        public void CalculateFuelUsage()
        {

        }
    }
}