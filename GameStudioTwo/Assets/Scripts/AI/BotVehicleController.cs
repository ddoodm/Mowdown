﻿using UnityEngine;
using System.Collections;

public class BotVehicleController : MonoBehaviour
{
    /// <summary>
    /// Wheel colliders at vehicle edges
    /// TODO: Create axle class
    /// </summary>
    public WheelCollider[] wheels;

    /// <summary>
    /// Motor, braking and steering constraints
    /// </summary>
    public float
        maxTorque = 50.0f,
        maxBrakeTorque = 50.0f,
        maxSteeringAngle = 20.0f;

    /// <summary>
    /// Controller inputs obtained at frame update
    /// </summary>
    private float
        inputLinearForce,
        inputSteering;

    /// <summary>
    /// The duration for which the bot may ram the player
    /// </summary>
    private float contactTimer, reverseTimer;
    public float maxContactTime = 0.8f, maxReverseTime = 1.0f;

    /// <summary>
    /// The transform that the robot should follow
    /// </summary>
    public Transform target;
    public Rigidbody targetRigidbody;

    public enum SteeringMethod
    {
        ARRIVE,
        PURSUE,
    }
    public SteeringMethod steeringMethod = SteeringMethod.ARRIVE;

    private NavMeshPath path;

    void Start()
    {
        path = new NavMeshPath();
        targetRigidbody = target.GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 targetPosition;

        switch (steeringMethod)
        {
            case SteeringMethod.PURSUE:
                // Add target velocity to persue
                targetPosition = target.position + targetRigidbody.velocity;
                break;
            case SteeringMethod.ARRIVE: default:
                targetPosition = target.position;
                break;
        }        

        // Use the NavMesh to generate an array of waypoints
        NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path);

        // The target is the first waypoint, or the position of the target
        Vector3 targetWaypt = path.corners.Length>1? path.corners[1] : targetPosition;
        Debug.DrawLine(targetWaypt, targetWaypt + Vector3.up * 10.0f, Color.green);

        // Compute steering angle
        Vector3 targDir = targetWaypt - this.transform.position;                        // Direction to the target
        float targetAngle = Mathf.Atan2(targDir.z, targDir.x) * Mathf.Rad2Deg - 90.0f;  // Atan2 returns the angle to targDir from (0,0) in radians
        Debug.DrawRay(transform.position, targDir);
        inputSteering = -targetAngle - this.transform.rotation.eulerAngles.y;           // Steer toward the target angle, and subtract the local rotation

        // Constant driving force
        inputLinearForce = 1.0f;

        // Debug path display
        for (int i = 0; i < path.corners.Length - 1; i++)
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);

        // Prevent constant ramming
        controlRamming();

        // Robert's modifications, modified
        if (path.corners.Length == 2 && Vector3.Dot(targDir, transform.forward) < 0)
        {
            inputLinearForce = 1.1f;
            inputSteering = 40.0f;
        }
    }

    void FixedUpdate()
    {
        // Front-wheel steering
        float steerAngle = inputSteering;
        wheels[0].steerAngle = wheels[1].steerAngle = steerAngle;

        // Front-wheel drive
        float torque = inputLinearForce * maxTorque;
        for (int i = 0; i < 2; i++)
            wheels[i].motorTorque = torque;
    }

    /// <summary>
    /// Disallows constant ramming
    /// </summary>
    void controlRamming ()
    {
        // If the AI has been in contact for > maxContactTime
        if (contactTimer > maxContactTime)
        {
            // Set the reverse timer
            contactTimer = 0;
            reverseTimer = maxReverseTime;
        }

        // If the AI still needs to reverse
        if (reverseTimer > 0)
        {
            // Invert force and steering (reverse)
            inputLinearForce *= -1.0f;
            inputSteering *= -0.8f;
            reverseTimer -= Time.deltaTime;
        }
    }

    void OnCollisionStay(Collision col)
    {
        // Increate 'contact' timer to avoid annoying ramming
        if(col.transform.root.gameObject.tag == "Player")
            contactTimer += Time.deltaTime;
    }

    void OnCollisionExit(Collision col)
    {
        if (col.transform.root.gameObject.tag == "Player")
            contactTimer = 0;
    }
}