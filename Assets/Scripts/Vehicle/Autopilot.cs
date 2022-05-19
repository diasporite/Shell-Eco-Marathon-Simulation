using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VirtualTwin
{
    public class AutopilotPath
    {
        public bool curvedPath;

        public float speed = 5f;
        public float steerAngle = 0f;
    }

    public class Autopilot : MonoBehaviour
    {
        Vehicle2 vehicle;

        [SerializeField] List<AutopilotPath> paths = new List<AutopilotPath>();

        [SerializeField] GameObject waypointHolder;
        [SerializeField] List<Waypoint> waypoints = new List<Waypoint>();
        [SerializeField] int currentWaypointIndex = 0;

        [Header("Parameters")]
        [SerializeField] float speedLimit = 5f;
        [SerializeField] float steerTurnLimit = 7.5f;
        [SerializeField] float maxDeviation = 1f;
        [Range(0.0001f, 1f)]
        [SerializeField] float steerCoefficient = 0.05f;
        [SerializeField] float forwardDeadZone = 2f;
        public float dtheta = 0;

        [Header("Inputs")]
        [SerializeField] float steerInput;
        [SerializeField] float accelerateInput;
        [SerializeField] float brakeInput;

        public float Steer => steerInput;
        public float Accelerate => accelerateInput;
        public float Brake => brakeInput;

        Vector3 ToNextWaypoint
        {
            get
            {
                if (currentWaypointIndex < waypoints.Count - 1)
                    return waypoints[currentWaypointIndex + 1].transform.position - 
                        waypoints[currentWaypointIndex].transform.position;
                return transform.forward;
            }
        }

        private void Awake()
        {
            vehicle = GetComponent<Vehicle2>();
            waypoints = waypointHolder.GetComponentsInChildren<Waypoint>().ToList();
        }

        public void Pilot()
        {
            GetInput();
            CheckWaypoint();
        }

        public void GetInput()
        {
            dtheta = Vector3.SignedAngle(transform.forward, ToNextWaypoint, Vector3.up);

            dtheta = Mathf.Clamp(dtheta, -steerTurnLimit, steerTurnLimit);
            if (Mathf.Abs(dtheta) <= Mathf.Abs(forwardDeadZone)) steerInput = 0;
            else steerInput = Mathf.Clamp(steerCoefficient * dtheta / 
                (vehicle.frontLeftWheel.steeringSpeed * Time.deltaTime), -1, 1);

            var speed = vehicle.speed;

            if (speed < speedLimit)
            {
                accelerateInput = 1;
                brakeInput = 0;
            }
            else if (speed > speedLimit)
            {
                accelerateInput = 0;
                brakeInput = 1;
            }
            else
            {
                accelerateInput = 0;
                brakeInput = 0;
            }
        }

        void CheckWaypoint()
        {
            Waypoint point = null;

            Collider[] hits = Physics.OverlapSphere(transform.position, maxDeviation, 
                LayerMask.GetMask("Waypoint"));
            if (hits.Length > 0) point = hits[0].GetComponent<Waypoint>();

            if (waypoints.Contains(point)) currentWaypointIndex = waypoints.IndexOf(point);
        }
    }
}