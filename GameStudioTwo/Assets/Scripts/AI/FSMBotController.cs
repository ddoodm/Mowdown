﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BotVehicleController))]
public class FSMBotController : MonoBehaviour
{
    /// <summary>
    /// The duration for which the bot may ram the player
    /// </summary>
    private float contactTimer, reverseTimer;
    public float maxContactTime = 0.8f, maxReverseTime = 1.0f;

    public float
        patrolDistance = 20.0f,     // The bot will stop following the player at this distance
        wanderDistance = 10.0f;     // The distance that the bot will wander

    public Transform playerTransform;

    private BotVehicleController controller;
    private PlayerHealth botHealth, playerHealth;
    private Rigidbody botRigidbody;
    private SocketEquipment playerEquip;

    private NavMeshPath path;

    public enum FSMState
    {
        PATROL,
        ARRIVE,
        ARRIVE_SIDE,
        EVADE,
        REVERSE,
    }
    public FSMState state { get; protected set; }

    void Start()
    {
        controller = GetComponent<BotVehicleController>();
        botRigidbody = GetComponent<Rigidbody>();
        botHealth = GetComponent<PlayerHealth>();

        // Start bot in patrol mode:
        state = FSMState.PATROL;

        if (playerTransform == null)
            playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();

        playerHealth = playerTransform.GetComponent<PlayerHealth>();
        playerEquip = playerTransform.GetComponent<SocketEquipment>();

        path = new NavMeshPath();

        goToRandomPosition();
    }

    void Update()
    {
        if (!controller.canPathfind)
            goToRandomPosition();

        switch (state)
        {
            case FSMState.PATROL:
                execute_patrol();
                state = transition_patrol();
                return;
            case FSMState.ARRIVE:
                execute_arrive();
                state = transition_arrive();
                return;
            case FSMState.ARRIVE_SIDE:
                execute_arriveSide();
                state = transition_arriveSide();
                return;
            case FSMState.EVADE:
                execute_evade();
                state = transition_evade();
                return;
            case FSMState.REVERSE:
                execute_reverse();
                state = transition_reverse();
                return;
        }
    }

    private void execute_patrol()
    {
        controller.targetSpeed = 1.0f;
        if (controller.atTarget || botRigidbody.velocity.magnitude < 0.1f)
            goToRandomPosition();
    }

    private FSMState transition_patrol()
    {
        float distanceToPlayer =
            (this.transform.position - playerTransform.position).magnitude;

        // Stay away if we have less health
        /*if (botHealth.health < playerHealth.health)
            return FSMState.PATROL;*/

        if (distanceToPlayer < patrolDistance)
            return FSMState.ARRIVE;

        return FSMState.PATROL;
    }

    private void execute_arrive()
    {
        controller.targetSpeed = 1.0f;
        controller.setTargetWaypoint(playerTransform.position);
    }

    private FSMState transition_arrive()
    {
        float distanceToPlayer =
            (this.transform.position - playerTransform.position).magnitude;

        // Check whether the player has a hazard on their front.
        // If they do, attack from the side.
        Equipment[] dangerItems = new Equipment[]
            { Equipment.Item_Flipper, Equipment.Item_CircularSaw, Equipment.Item_Spike };
        if(distanceToPlayer > 5.0f)
            if (playerEquip.SocketContainsAnyOf(SocketLocation.FRONT, dangerItems))
                return FSMState.ARRIVE_SIDE;

        if (distanceToPlayer > patrolDistance)
            return FSMState.PATROL;

        // Run away if we have less health than the player
        /*
        if (botHealth.health < playerHealth.health)
            return FSMState.EVADE;*/

        //if (distanceToPlayer < 2.5f)
        //    return FSMState.EVADE;

        return FSMState.ARRIVE;
    }

    private SocketLocation targetSocket = SocketLocation.NONE;
    private void execute_arriveSide()
    {
        Vector3 sideOffset = Vector3.zero;

        switch(targetSocket)
        {
            case SocketLocation.LEFT: sideOffset = -playerTransform.right * 4.0f; break;
            case SocketLocation.RIGHT: sideOffset = playerTransform.right * 4.0f; break;
            case SocketLocation.FRONT: sideOffset = playerTransform.forward * 4.0f; break;
            case SocketLocation.BACK: sideOffset = -playerTransform.forward * 4.0f; break;
        }

        controller.targetSpeed = 1.0f;
        controller.setTargetWaypoint(playerTransform.position + sideOffset);
    }

    private FSMState transition_arriveSide()
    {
        float distanceToPlayer =
            (this.transform.position - playerTransform.position).magnitude;

        // Determine the best socket for the first time only
        targetSocket = findBestAttackSocket();

        if (distanceToPlayer > patrolDistance)
        {
            targetSocket = SocketLocation.NONE;
            return FSMState.PATROL;
        }

        // If we're at the player's side, arrive at them as normal
        if (distanceToPlayer < 4.5f)
        {
            targetSocket = SocketLocation.NONE;
            return FSMState.ARRIVE;
        }

        return FSMState.ARRIVE_SIDE;
    }

    private void execute_evade()
    {
        controller.setTargetWaypoint(
            (transform.position - playerTransform.position).normalized * 10.0f);
    }

    private FSMState transition_evade()
    {
        float distanceToPlayer =
            (this.transform.position - playerTransform.position).magnitude;

        if (distanceToPlayer > patrolDistance)
            return FSMState.PATROL;

        if (botRigidbody.velocity.magnitude < 0.1f)
            return FSMState.PATROL;

        // If we have more health than the player, chase them
        if (botHealth.health >= playerHealth.health)
            return FSMState.ARRIVE;

        return FSMState.EVADE;
    }

    private void execute_reverse()
    {
        
    }

    private FSMState transition_reverse()
    {
        return FSMState.ARRIVE;
    }

    private void goToRandomPosition()
    {
        Vector3 finalTarget =
            this.transform.position + Random.insideUnitSphere * wanderDistance;
        finalTarget.y = 0;

        controller.setTargetWaypoint(finalTarget);
    }

    private SocketLocation findBestAttackSocket()
    {
        /*
        // Find an empty socket that would be suitable for attack (if possible)
        for(int i = 0; i <= (int)SocketLocation.BACK; i++)
            if (playerEquip.equipmentTypes[i] == Equipment.EMPTY)
                return (SocketLocation)i;

        // No empty sockets were located. Use a random socket instead
        SocketLocation bestSock = (SocketLocation)Random.Range((int)SocketLocation.LEFT, (int)SocketLocation.BACK);
        return bestSock;
        */

        return SocketLocation.LEFT;
    }
}
