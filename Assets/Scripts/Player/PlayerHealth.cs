using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    public PlayerMovement playerMovement;
    public ScriptableHealth health;
    public ScriptableHealth defaults;
    public AudioSource hitSound;
    public Dialogue dash;
    public Dialogue wallJump;
    public float iTime;
    public float blinkSpeed;
    private bool isImmune;

    public static PlayerHealth Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        healthRemaining = health.health;
        maxHealth = health.maxHealth;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public override bool Damage(float amount)
    {
        if (!playerMovement.isAttacking && !isImmune)
        {
            healthRemaining -= amount;
            health.health -= amount;
            hitSound.Play();
            if (healthRemaining <= 0)
            {
                Die();
                return true;
            }
            StartCoroutine(IFrames(iTime));
            return false;
        } else
        {
            return false;
        }
    }

    public IEnumerator IFrames(float t)
    {
        isImmune = true;
        yield return new WaitForSeconds(t);
        isImmune = false;
    }

    public IEnumerator DamageBlink()
    {
        yield return new WaitForEndOfFrame();
    }

    public void Die()
    {
        StartCoroutine("Rip");
    }

    public IEnumerator Rip()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
        PlayerMovement pController = GetComponent<PlayerMovement>();
        pController.enabled = false;
        health.health = 0;
        DialogueManager.Instance.FadeToBlack(3);
        yield return new WaitForSeconds(1);
        DialogueManager.Instance.DisplayDeathText();
        yield return new WaitForSeconds(1);
        transform.position = health.lastBonfire.transform.position;
        RestoreMax();
        yield return new WaitForSeconds(1);
        pController.enabled = true;
    }

    public override void Restore(float amount)
    {
        healthRemaining += amount;
        health.health += amount;
        if (healthRemaining > maxHealth)
        {
            healthRemaining = maxHealth;
        }
    }

    public void RestoreMax()
    {
        health.health = health.maxHealth;
        healthRemaining = health.health;
    }

    public void SetSpawn(GameObject bonfire)
    {
        health.lastBonfire = bonfire;
    }

    public void IncreaseHealth()
    {
        if (health.maxHealth < health.absoluteMaxHealth)
        {
            health.maxHealth++;
            health.health = health.maxHealth;
            healthRemaining = health.health;
        }
    }

    public void EnableDash()
    {
        health.canDash = true;
        DialogueManager.Instance.StartDialogue(dash);
    }

    public void EnableWall()
    {
        health.canWall = true;
        DialogueManager.Instance.StartDialogue(wallJump);
    }

    public void Reset()
    {
        health.health = defaults.health;
        health.maxHealth = defaults.maxHealth;
        health.absoluteMaxHealth = defaults.absoluteMaxHealth;
        health.canDash = defaults.canDash;
        health.dashesPerJump = defaults.dashesPerJump;
        health.canWall = defaults.canWall;
    }

}
