using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public Collider2D hit;
    public float strength;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //hit.enabled = false;
        Debug.Log("Contact");
        if (collision.gameObject.TryGetComponent<Health>(out Health health))
        {
            Debug.Log("Health contact");
            bool hitSuccess = health.Damage(strength);
            if (hitSuccess)
            {
                //move combo to next
            } else
            {
                playerMovement.lastMove = PlayerMovement.Combo.PARRY;
            }
        }
    }

}
