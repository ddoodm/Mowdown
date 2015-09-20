﻿using UnityEngine;
using System.Collections;

public class PlayerSocketController : MonoBehaviour
{
    public KeyCode control;
    public string button;

    private Weapon childWeapon;

    // Update is called once per frame
    void Update()
    {
        try
        {
            if ((Input.GetButtonDown(button) || Input.GetKeyDown(control)))
                UseChildWeapon();
            if ((Input.GetButtonUp(button) || Input.GetKeyUp(control)))
                EndUseChildWeapon();
        }
        catch (System.Exception e)
        {
            throw new System.Exception(this.name + " (socket) exception: " + e.Message);
        }
    }

    private Weapon GetChildWeapon()
    {
        if (transform.childCount < 1)
            return null;
        if (transform.childCount > 1)
            throw new System.Exception(this.name + " must have ONE child that implements the Weapon interface.");

        childWeapon = transform.GetChild(0).GetComponent<Weapon>();
        if (childWeapon == null)
            throw new System.Exception(this.name + "'s child must contain a component that implements the Weapon interface.");

        return childWeapon;
    }

    private void UseChildWeapon()
    {
        childWeapon = GetChildWeapon();

        if(childWeapon != null)
            childWeapon.Use();
    }

    private void EndUseChildWeapon()
    {
        childWeapon = GetChildWeapon();

        if(childWeapon != null)
            childWeapon.EndUse();
    }
}