﻿using UnityEngine;
using System.Collections;

public class flipperControls : MonoBehaviour {

    public float
        flipForce = 800.0f,
        proximityRadius = 1.0f;

    private float
        initialSpring, initialSpringTarget, initialRotation;

    public Vector3 initialRot = new Vector3(0,0,0);

    public bool canFlip { get; protected set; }


    public float flipstate = 0,
    flipSpeed = 1,
    counter = 0;


    //HingeJoint hinge;

    Rigidbody thisRigidbody, opRigidbody;
    public KeyCode control;
    public string button;

    public Transform visible;


    public float curveVar;
    public AnimationCurve curve;

	// Use this for initialization
	void Start () {

        control = transform.parent.GetComponentInParent<socketControl>().control;
        button = transform.parent.GetComponentInParent<socketControl>().button;

        visible = transform.parent.parent.parent.transform;

        //hinge = GetComponent<HingeJoint>();
        thisRigidbody = GetComponent<Rigidbody>();
        opRigidbody = GameObject.FindWithTag("Enemy").GetComponent<Rigidbody>();

        //initialSpring = hinge.spring.spring;
        //initialSpringTarget = hinge.spring.targetPosition;

        canFlip = true;
        curveVar = 1.0f;
	}
	
	// Update is called once per frame
	void Update ()
    {
        
        curveVar += Time.deltaTime;
        if ((/*Input.GetButtonUp(button) || */Input.GetKeyUp(control)) && flipstate == 0)
        {
            curveVar = 0;
            // Flip the flipper's hinge joint
            flipstate = 10f;

            // The target may not have been collided with
            if (!opRigidbody)
                return;

            // If the flipper is close enough to the bot:
            if ((this.transform.position - opRigidbody.transform.position).magnitude < proximityRadius)
            {
                Vector3 normal = (opRigidbody.transform.position - this.transform.position).normalized;

                // Apply a reliable force to the bot rigidbody
                opRigidbody.AddForceAtPosition(
                    0.5f * (normal + Vector3.up) * flipForce,
                    this.transform.position,
                    ForceMode.Impulse);
            }
            else if(Vector3.Dot(transform.up, Vector3.up) < 0)
            {
                transform.parent.GetComponentInParent<Rigidbody>().AddForceAtPosition(0.5f * Vector3.up * flipForce, this.transform.position, ForceMode.Impulse);
            }
            
        }
        flipHinge();
    }

    private void flipHinge()
    {
        counter += flipstate * flipSpeed;

        this.transform.Rotate(new Vector3(0, 0, flipstate * -flipSpeed));

        if (counter > 50)
        {
            counter = 0;
            flipstate = -1;
        }
        if (counter < -60)
        {
            this.transform.rotation = Quaternion.Euler(new Vector3(0,transform.parent.parent.parent.transform.rotation.eulerAngles.y - initialRot.y,0));
            //transform.parent.parent.parent.transform
            flipstate = 0;
            counter = 0;
        }



    }

    /*
    private void flipHinge ()
    {
        JointSpring hingeSpring = hinge.spring;
        hingeSpring.spring = initialSpring;
        hingeSpring.targetPosition = 135;
        hinge.spring = hingeSpring;

        canFlip = false;

        StartCoroutine(unflipHinge(hingeSpring));
    }

    IEnumerator unflipHinge(JointSpring spring)
    {
        yield return new WaitForSeconds(1.0f);
        spring.spring = 2.0f;
        spring.targetPosition = initialSpringTarget;
        hinge.spring = spring;

        yield return new WaitForSeconds(1.0f);
        spring.spring = initialSpring;
        hinge.spring = spring;

        // Wait for the flipper to un-flip
        float rotDelta = 0.0f;
        do
        {
            rotDelta = Mathf.Abs(initialRotation - transform.rotation.z);
            yield return new WaitForSeconds(0.1f);
        } while (rotDelta > 10.0f);

        canFlip = true;
    }
     * */

    /* Bad idea:
    void OnCollisionStay(Collision col)
    {
        if(col.collider.GetComponent<Rigidbody>())
            opRigidbody = col.collider.GetComponent<Rigidbody>();
    }

    void OnCollisionExit(Collision col)
    {
        opRigidbody = null;
    }*/
}
