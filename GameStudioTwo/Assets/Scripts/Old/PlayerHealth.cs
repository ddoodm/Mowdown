﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    //public Slider healthSlider;
    public HPBar healthBar;
    public Text finish;
    public Button restartButton;

    public float
        maxHealth = 50.0f,
        mass = 1.0f,
        damageFactor = 1.0f;


    // Never set this directly! Anywhere! Yes, this means you!
    private float _health;
    public float health
    {
        get { return _health; }
        set
        {
            _health = value<0? 0 : value;
            healthBar.value = _health;
        }
    }

    void Start()
    {
        healthBar.maxValue = maxHealth;
        health = maxHealth;
        calculateMass();
    }
	
	// Update is called once per frame
	void Update ()
    {

        if (finish == null)
            return;

        calculateMass();
        if (health <= 0)
            gameOver();
        if (Vector3.Dot(transform.up,Vector3.up) < 0)
        {
            StartCoroutine(checkFlipped());
        }
	}

    public void issueDamage(Collision collision)
    {

        //Determine damage multiplier
        float damageMultiplier = 1.0f;

        PlayerHealth enemy = collision.gameObject.GetComponent<PlayerHealth>();
        float damage = collision.relativeVelocity.magnitude;
        float thisDamage = 0;

        //check if hit by weapon
        foreach (ContactPoint contact in collision.contacts)
        {
            weaponStats weapon = contact.otherCollider.gameObject.GetComponent<weaponStats>();
            weaponStats armor = contact.thisCollider.gameObject.GetComponent<weaponStats>();
            if (weapon != null)
            {           
                if(weapon.damageMultiplier > 1)
                    damageMultiplier = weapon.damageMultiplier;
            }
            if (armor != null)
            {
                if (armor.damageMultiplier < 1)
                    damageMultiplier *= armor.damageMultiplier;
            }

            thisDamage = damage * damageMultiplier * enemy.mass;

            if (armor != null)
                armor.issueDamageAttachment(thisDamage);

        }

        //Deinyon's code to make damage only happen on T-Bones
        //float damageAngleMag = Mathf.Abs(Vector3.Dot(collision.contacts[0].normal, -collision.collider.transform.right));
        //float otherDamage = damage * (1.0f - damageAngleMag);


        

        Debug.Log("Damage Multiplier: " + damageMultiplier);
        Debug.Log(collision.gameObject.tag + " Mass: " + enemy.mass);

        this.issueDamage(thisDamage);

        Debug.Log(gameObject.name + " has " + health + " remaining");
    }

    public void issueDamage(float damage)
    {
        Debug.Log(gameObject.tag + " hit for " + damage);
        health -= damage;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Player"))
            issueDamage(collision);
    }

    private void calculateMass()
    {
        mass = 1;
        weaponStats[] allChildren = GetComponentsInChildren<weaponStats>();
        foreach (weaponStats child in allChildren)
        {
            mass += child.mass;
        }
    }

    void gameOver()
    {
        if (gameObject.CompareTag("Player"))
            finish.text = "You Lose";

        else if (gameObject.CompareTag("Enemy"))
            finish.text = "You win";


        Time.timeScale = 0.0f;

        restartButton.gameObject.SetActive(true);
        if (Input.GetButtonDown("Boost"))
        {
            Application.LoadLevel(1);
        }
    }

    IEnumerator checkFlipped()
    {
        yield return new WaitForSeconds(5);
        if (Vector3.Dot(transform.up, Vector3.up) < 0)
            gameOver();
        else
            StopCoroutine("checkFlipped");
    }

}
