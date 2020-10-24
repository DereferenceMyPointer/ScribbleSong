using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{

    public float animationTime;
    public AudioSource sound;

    public void Dash(Animator animator, Animator effects, float dashSpeed, Rigidbody2D rb, PlayerMovement p)
    {
        sound.Play();
        StartCoroutine(Perform(animator, effects, dashSpeed, rb, p));
    }

    public IEnumerator Perform(Animator animator, Animator effects, float speed, Rigidbody2D rb, PlayerMovement p)
    {
        rb.velocity = new Vector2(speed, 0f);
        rb.gravityScale = 0;
        p.enabled = false;
        animator.SetTrigger("Dash");
        effects.SetTrigger("Dash");
        yield return new WaitForSeconds(animationTime);
        p.enabled = true;
        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

}
