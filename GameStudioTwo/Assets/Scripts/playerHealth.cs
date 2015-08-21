﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class playerHealth : MonoBehaviour {

    public Slider healthSlider;
    public Text finish;

    public float
        maxHealth = 50.0f,
        mass = 1,
        damageFactor = 1.0f;

    // Never set this directly! Anywhere! Yes, this means you!
    private float _health;

    public float health
    {
        get { return _health; }
        set
        {
            _health = value<0? 0 : value;
            healthSlider.value = _health;
        }
    }

    void Start()
    {
        healthSlider.maxValue = maxHealth;
        health = maxHealth;

        foreach (Transform child in transform)
        {
            if (child.CompareTag("Weapon"))
            {
                mass += child.GetComponent<weaponStats>().mass;
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (health <= 0)
        {
            if (gameObject.CompareTag("Player"))
                finish.text = "You Lose";
            else if (gameObject.CompareTag("Enemy"))
                finish.text = "You win";
        }
	}

    public void issueDamage(Collision collision)
    {
        // Rob's:
        //float damage = Random.Range(collision.relativeVelocity.magnitude - 2, collision.relativeVelocity.magnitude + 2);

        //Determine damage multiplier
        float damageMultiplier = 1.0f;
        foreach (ContactPoint contact in collision.contacts)
        {
            weaponStats weapon = contact.otherCollider.gameObject.GetComponent<weaponStats>();
            if (weapon != null)
            {
                if (weapon.damageMultiplier > damageMultiplier)
                {
                    damageMultiplier = weapon.damageMultiplier;
                }
            }
        }

        playerHealth enemy = collision.gameObject.GetComponent<playerHealth>();

        float damage = collision.relativeVelocity.magnitude;


        //Deinyon's code to make damage only happen on T-Bones
        //float damageAngleMag = Mathf.Abs(Vector3.Dot(collision.contacts[0].normal, -collision.collider.transform.right));
        //float otherDamage = damage * (1.0f - damageAngleMag);


        float thisDamage = damage * damageMultiplier * enemy.mass;
        Debug.Log("Damage Multiplier: " + damageMultiplier);
        Debug.Log(collision.gameObject.tag + " Mass: " + enemy.mass);

        this.issueDamage(thisDamage);
    }

    public void issueDamage(float damage)
    {
        Debug.Log(gameObject.tag + " hit for " + damage);
        health -= damage;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Player"))
        {
            issueDamage(collision);
        }
    }
}
