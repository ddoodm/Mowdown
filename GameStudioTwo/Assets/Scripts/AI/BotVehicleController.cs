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
        maxSteeringAngle = 20.0f,
        targetSpeed = 1.0f;

    public float
        targetThreshRadius = 1.0f;

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

    private Vector3 targetWaypoint;

    public bool canPathfind { get; private set; }

    public enum SteeringMethod
    {
        ARRIVE,
        PURSUE,
    }
    public SteeringMethod steeringMethod = SteeringMethod.ARRIVE;

    private NavMeshPath path;

	/// <summary>
	/// The bot items.
	/// </summary>
	public Equipment[] botItems;

    void Start()
    {
        path = new NavMeshPath();
        //targetRigidbody = target.GetComponent<Rigidbody>();

		// Jesse's code to add attachments to the bot
		AddAttachments ();
    }

    void Update()
    {
        // Use the NavMesh to generate an array of waypoints
        NavMesh.CalculatePath(transform.position, targetWaypoint, NavMesh.AllAreas, path);
        Debug.DrawLine(targetWaypoint, targetWaypoint + Vector3.up * 10.0f, Color.magenta);
        canPathfind = (path.status == NavMeshPathStatus.PathComplete);
        if (!canPathfind)
            Debug.Log("Cannot pathfind!");

        // The target is the first waypoint, or the position of the target
        Vector3 cWaypt = path.corners.Length > 1 ? path.corners[1] : targetWaypoint;
        Debug.DrawLine(cWaypt, cWaypt + Vector3.up * 10.0f, Color.green);

        // Compute steering angle
        Vector3 targDir = cWaypt - this.transform.position;                        // Direction to the target
        float targetAngle = Mathf.Atan2(targDir.z, targDir.x) * Mathf.Rad2Deg - 90.0f;  // Atan2 returns the angle to targDir from (0,0) in radians
        Debug.DrawRay(transform.position, targDir);
        inputSteering = -targetAngle - this.transform.rotation.eulerAngles.y;           // Steer toward the target angle, and subtract the local rotation

        // Constant driving force
        inputLinearForce = targetSpeed;

        // Debug path display
        for (int i = 0; i < path.corners.Length - 1; i++)
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);

        // Prevent constant ramming
        controlRamming();

        // Robert's modifications, modified
        // (If the waypoint is behind the bot, reverse [don't try to ram walls!])
        if (/*path.corners.Length == 2 &&*/ Vector3.Dot(targDir, transform.forward) < 0)
        {
            inputLinearForce = -1.1f;
            inputSteering = 40.0f;
        }
    }

    public void setTargetWaypoint(Vector3 targetWaypoint)
    {
        this.targetWaypoint = targetWaypoint;
    }

    public bool atTarget
    {
        get { return (targetWaypoint - transform.position).magnitude <= targetThreshRadius; }
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
            inputSteering = -8.0f;
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




	public void AddAttachments()
	{
		botItems = new Equipment[5];
		for (int i = 0; i < 5; i++)
		{
			botItems[i] = Equipment.EMPTY;
		}


		int rand;
		for (int i = 0; i < 3; i++) 
		{
			rand = Random.Range (0, 3);

			switch (rand)
			{
				case 0:
					botItems[i] = Equipment.Item_Spike;
					break;

				case 1:
					botItems[i] = Equipment.Item_Flipper;
					break;

				case 2:
					if (i == 2)
						i--;
					else
						botItems[i] = Equipment.Item_Booster;
					break;
			
				default:
					break;
			}
		}

		
		botItems[3] = Equipment.Item_Handle;



		
		GetComponent<SocketEquipment>().SocketItems(botItems, true);

		//this.transform.Rotate (0.0f, 180.0f, 0.0f, Space.World);
	}



}






