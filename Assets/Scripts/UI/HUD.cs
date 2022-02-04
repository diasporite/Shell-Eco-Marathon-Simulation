using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VirtualTwin
{
    public class HUD : MonoBehaviour
    {
        [Header("Text")]
        public Text timeElapsed;
        public Text vehicleSpeed;
        public Text distanceTravelled;
        public Text numSamples;
        public Text acceleration;
        public Text drag;

        public Text vehicleMass;
        public Text fuelMass;

        public void UpdateUI(int numSample, DataPoint sample)
        {
            if (sample == null) sample = new DataPoint(0, 0, 0);

            timeElapsed.text = "Time Elapsed, t = " + 
                sample.Time.ToString("0.00") + "s";
            vehicleSpeed.text = "Vehicle Speed, |v| = " + 
                sample.Speed.ToString("0.000") + "m/s";
            distanceTravelled.text = "Distance Travelled, s = " + 
                sample.Distance.ToString("0.000") + "m";
            numSamples.text = "# of Data Samples, n = " + numSample;
            acceleration.text = "Acceleration, a = " + 
                sample.Acceleration.ToString("0.0000") + "m/s2";
            drag.text = "Drag, Fd = " + sample.VehicleDrag.ToString("0.0000") + "N";

            vehicleMass.text = "Vehicle Mass, m = " + sample.Mass.ToString("0.000") + "kg";
            fuelMass.text = "Fuel Mass, mf = " + sample.FuelMass.ToString("0.000") + "kg";
        }
    }
}