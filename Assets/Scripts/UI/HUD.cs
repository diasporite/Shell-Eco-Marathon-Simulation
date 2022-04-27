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
        public Text lift;
        public Text centripetal;
        public Text turningRadius;

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
            vehicleSpeed.text = "Vehicle Speed, v = " + 
                sample.speed.ToString("0.000") + "m/s";
            distanceTravelled.text = "Distance Travelled, s = " + 
                sample.distance.ToString("0.000") + "m";
            numSamples.text = "# of Data Samples, n = " + numSample;
            acceleration.text = "Acceleration, a = " + 
                sample.acceleration.ToString("0.0000") + "m/s2";
            drag.text = "Drag, Fd = " + sample.vehicleDrag.ToString("0.0000") + "N";
            lift.text = "Lift, Fl = " + sample.lift.ToString("0.0000") + "N";
            centripetal.text = "Centripetal (CoM), Fc = " + 
                sample.centripetal + "N";
            turningRadius.text = "Turning Radius (CoM), R = " + 
                sample.turningRadius.ToString("0.000") + "m";

            wheelDriveForce.text = "Wheel Drive Force, Fw = " + 
                sample.wheelDrive.ToString("0.000") + "N";
            wheelRollRes.text = "Wheel Rolling Res, Fr = " + 
                sample.rollingRes.ToString("0.000") + "N";
            wheelTurnAngle.text = "Wheel Turning Angle, δ = " + 
                sample.wheelTurnAngle.ToString("0.00") + "deg";
            vehicleTurnAngle.text = "Vehicle Orientation (CoM), θ = " + 
                sample.orientation.ToString("0.00") + "deg";
            vehicleVelAngle.text = "Vehicle Velocity Angle (CoM), β = " + 
                sample.velAngle.ToString("0.00") + "deg";
            vehicleAngVelocity.text = "Vehicle Angular Velocity (CoM), β_dot = " + 
                sample.angularVelocity.ToString("0.00") + "deg/s";

            vehicleMass.text = "Vehicle Mass, m = " + sample.mass.ToString("0.000") + "kg";
            fuelMass.text = "Fuel Mass, mf = " + sample.fuelMass.ToString("0.000") + "kg";
        }
    }
}

// α β γ δ ε ζ η θ ι κ λ μ ν ξ ο π ρ σ τ υ φ χ ψ ω
// Α Β Γ Δ Ε Ζ Η Θ Ι Κ Λ Μ Ν Ξ Ο Π Ρ Σ Τ Υ Φ Χ Ψ Ω