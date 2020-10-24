using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hedgehog : Health
{
    public GameObject graphic;
    public Rigidbody2D rb;
    public float speed;
    public float minTime;
    public float maxTime;
    public float flickerTimes;
    public float flickerSpeed;
    public float deathSpeed;
    public AudioSource hitSound;
    public LayerMask player;
    public SpriteRenderer p;

    public override bool Damage(float amount)
    {
        StopAllCoroutines();
        rb.velocity = Vector2.zero;
        healthRemaining -= amount;
        hitSound.Play();
        if (healthRemaining <= 0)
        {
            Die();
        } else
        {
            StartCoroutine(TakeDamage());
        }
        return true;
    }

    public IEnumerator TakeDamage()
    {
        Color w = Color.red;
        Color b = Color.white;
        w.a = 1;
        b.a = 1;
        for(int i = 0; i < flickerTimes; i++)
        {
            p.color = w;
            yield return new WaitForSeconds(flickerSpeed / 2);
            p.color = b;
            yield return new WaitForSeconds(flickerSpeed / 2);
        }
        StartCoroutine(Walk(maxTime));
    }

    public override void Restore(float amount)
    {
        throw new System.NotImplementedException();
    }

    public void Die()
    {
        Destroy(gameObject, deathSpeed);
    }

    // Start is called before the first frame update
    void Start()
    {
        p = graphic.GetComponent<SpriteRenderer>();
        StartCoroutine(Walk(maxTime));
    }

    public IEnumerator Walk(float t)
    {
        p.color = Color.white;
        if (Mathf.Abs(rb.velocity.x) != speed || rb.velocity.y != 0  )
        {
            OnTriggerEnter2D(null);
        }
        else
        {
            rb.velocity = -rb.velocity;
            graphic.transform.localScale = new Vector3(-graphic.transform.localScale.x, graphic.transform.localScale.y, graphic.transform.localScale.z);
        }
        yield return new WaitForSeconds(t);
        StartCoroutine(Walk(Random.Range(minTime, maxTime)));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StopAllCoroutines();
        if(graphic.transform.localScale.x < 0)
        {
            rb.velocity = new Vector2(-speed, 0);
            graphic.transform.localScale = new Vector3(1f, 1f, 0);
        } else
        {
            rb.velocity = new Vector2(speed, 0);
            graphic.transform.localScale = new Vector3(-1f, 1f, 0);
        }
        StartCoroutine(Walk(maxTime));
        if(collision != null && collision.gameObject.layer == player)
        {
            collision.GetComponent<Health>().Damage(1f);
        }
    }

}
