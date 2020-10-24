using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

[CreateAssetMenu(fileName = "Health", menuName = "Scriptable Objects/Health")]
public class ScriptableHealth : ScriptableObject
{
    //doubles as progression system
    public float health;
    public float maxHealth;
    public int livesRemaining;
    public int absoluteMaxHealth;

    public bool canDash;
    public int dashesPerJump;
    public bool canWall;
    public float dmg;

    public GameObject lastBonfire;

}
