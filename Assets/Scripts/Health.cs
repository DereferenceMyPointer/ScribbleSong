using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Health : MonoBehaviour
{
    public float maxHealth;
    public float healthRemaining;

    public abstract bool Damage(float amount);
    public abstract void Restore(float amount);
}
