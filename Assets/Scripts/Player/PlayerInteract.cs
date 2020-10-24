using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Interactable"))
        {
            if (Input.GetButtonDown("Interact"))
            {
                collision.gameObject.GetComponent<Interactable>().Interact();
            }
        }
    }
}
