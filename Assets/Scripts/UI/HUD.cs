using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VirtualTwin
{
    public class HUD : MonoBehaviour
    {
        [Header("Top Left Text")]
        public Text timeElapsed;
        public Text vehicleSpeed;
        public Text distanceTravelled;
        public Text numSamples;
        public Text acceleration;
        public Text drag;

        [Header("Top Right Text")]
        public Text wheelDriveForce;
        public Text wheelRollRes;
        public Text wheelTurnAngle;

        public Text vehicleTurnAngle;
        public Text vehicleVelAngle;
        public Text vehicleAngVelocity;

        [Header("Bottom Right Text")]
        public Text vehicleMass;
        public Text fuelMass;

        public void UpdateUI(int numSample, DataPoint sample)
        {
            if (sample == null) sample = new DataPoint(0, 0, 0);

            timeElapsed.text = "Time Elapsed, t = " + 
                sample.time.ToString("0.00") + "s";
            vehicleSpeed.text = "Vehicle Speed, |v| = " + 
                sample.speed.ToString("0.000") + "m/s";
            distanceTravelled.text = "Distance Travelled, s = " + 
                sample.distance.ToString("0.000") + "m";
            numSamples.text = "# of Data Samples, n = " + numSample;
            acceleration.text = "Acceleration, a = " + 
                sample.acceleration.ToString("0.0000") + "m/s2";
            drag.text = "Drag, Fd = " + sample.vehicleDrag.ToString("0.0000") + "N";

            wheelTurnAngle.text = "Wheel Turning Angle, = " + 
                sample.wheelTurnAngle.ToString("0.00") + "deg";
            vehicleTurnAngle.text = "Vehicle Turning Angle, = " + 
                sample.turnAngle.ToString("0.00") + "deg";
            vehicleVelAngle.text = "Vehicle Velocity Angle, = " + 
                sample.velAngle.ToString("0.00") + "deg";
            vehicleAngVelocity.text = "Vehicle Angular Velocity, = " + 
                sample.angularVelocity.ToString("0.00") + "deg/s";

            vehicleMass.text = "Vehicle Mass, m = " + sample.mass.ToString("0.000") + "kg";
            fuelMass.text = "Fuel Mass, mf = " + sample.fuelMass.ToString("0.000") + "kg";
        }
    }
}