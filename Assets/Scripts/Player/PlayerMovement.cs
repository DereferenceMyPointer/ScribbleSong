using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Progression")]
    public ScriptableHealth status;
    public PlayerDash dash;

    [Header("Horizontal Movement")]
    public float moveSpeed;
    public Vector2 direction;
    public float turnTime;
    public float dashSpeed;
    public float wallDistance;
    public float slideSpeed;

    [Header("Jumping")]
    public float jumpSpeed;
    public float fallMultiplier;
    public float defGravity;
    public float terminalVelocity;

    [Header("Abilities")]
    public float moveForgiveness;
    public float attackCooldown;
    public float lightAttackRadius;
    public float lightAttackDistance;

    public enum Combo
    {
        NO_MOVE,
        LIGHT_ATTACK,
        LIGHT_ATTACK2,
        TRIPLE_SLASH,
        PARRY,
        HEAVY_SLASH,
    };
    public Dictionary<Combo, float> attackCooldowns;
    public Combo lastMove = Combo.NO_MOVE;
    public bool isAttacking;
    private float attack;
    private float sinceAttack;
    private bool canAttack;
    public float dashCooldown;
    private int canDash;
    private float sinceDash;
    public bool onWall = false;

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    public Animator slash;
    public Transform graphicsHolder;
    public GameObject slashSprite;
    public AudioSource aPlayer;
    public LayerMask groundLayer;
    public LayerMask damageable;

    [Header("Sound")]
    public AudioSource lightAttackSound;
    public AudioSource dashSound;
    public AudioSource footstep;

    [Header("Collision")]
    public bool onGround = false;
    public float groundDistance;

    private float movementDirection;
    private bool orientation = true;
    private float sinceJump;
    private float sinceFall;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        aPlayer = GetComponent<AudioSource>();
        attackCooldowns = new Dictionary<Combo, float>();
        attackCooldowns.Add(Combo.LIGHT_ATTACK, attack);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        PlayerHealth.Instance.Reset();
        animator.SetTrigger("Reset");

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * Mathf.Sign(movementDirection) * wallDistance);
        Gizmos.DrawSphere(transform.position + new Vector3(Mathf.Round(movementDirection) * lightAttackDistance, 0, 0), lightAttackRadius);
    }

    private void Update()
    {
        if (status.canWall)
        {
            onWall = Physics2D.Raycast(transform.position, transform.position + Vector3.right * Mathf.Sign(movementDirection), wallDistance * -Mathf.Sign(movementDirection), groundLayer);
        }
        bool wasOnGround = onGround;
        onGround = Physics2D.Raycast(transform.position, Vector2.down, groundDistance, groundLayer);
        if (onGround != wasOnGround)
        {
            sinceFall = moveForgiveness;
        }
        HandleJump();
        if (onGround)
        {
            animator.SetBool("JumpUp", false);
            animator.SetBool("JumpDown", false);
            canDash = status.dashesPerJump;
        }
        else if (!animator.GetBool("JumpUp"))
        {
            animator.SetBool("JumpUp", true);
            animator.SetBool("JumpDown", true);
        }
        if (Input.GetButton("Horizontal") && direction.x != 0)
        {
            movementDirection = direction.x;
        }
        if ((Input.GetButtonDown("Horizontal") && onGround) ||
            (onGround == true && wasOnGround == false))
        {
            footstep.Play();
        } else if (rb.velocity.x == 0)
        {
            footstep.Stop();
        }
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        MoveCharacter(direction.x);
        WallSlide();

        HandleAttacks();
        HandleDash();
    }

    private void FixedUpdate()
    {
        
    }

    void WallSlide()
    {
        if (onWall && rb.velocity.y <= 0 && Input.GetAxisRaw("Horizontal") != 0)
        {
            rb.gravityScale = 0;
            rb.velocity = new Vector2(rb.velocity.x, -slideSpeed);
        }
    }

    void HandleDash()
    {
        if (sinceDash > 0)
        {
            sinceDash -= Time.deltaTime;
        }
        if (Input.GetButtonDown("Fire2") && !isAttacking && sinceDash <= 0 && !onGround && canDash > 0)
        {
            dash.Dash(animator, slash, dashSpeed * Mathf.Round(movementDirection), rb, this);
            sinceDash = dashCooldown;
            canDash--;
            dashSound.Play();
        }
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        animator.SetBool("JumpUp", true);
        footstep.Stop();
        if (onWall)
        {
            //something???
        }
    }

    void HandleJump()
    {
        if (sinceJump > 0)
        {
            sinceJump -= Time.deltaTime;
        }
        if (sinceFall > 0)
        {
            sinceFall -= Time.deltaTime;
        }
        if (Input.GetButtonDown("Jump"))
        {
            sinceJump = moveForgiveness;
        }
        if (!onGround)
        {
            rb.gravityScale = defGravity;
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = fallMultiplier;
                animator.SetBool("JumpDown", true);
            } else if(rb.velocity.y > 0)
            {
                if (!Input.GetButton("Jump"))
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    rb.gravityScale = defGravity * fallMultiplier / 2;
                    animator.SetBool("JumpDown", true);
                }
            }
        }
        if (sinceJump > 0 && (onGround || sinceFall > 0 || onWall))
        {
            Jump();
        }
    }

    void MoveCharacter(float horizontal)
    {
        rb.velocity = new Vector2(Vector2.right.x * horizontal * moveSpeed, rb.velocity.y);
        if (rb.velocity.x != 0)
        {
            animator.SetBool("Running", true);
        } else
        {
            animator.SetBool("Running", false);
        }
        if ((horizontal > 0 && !orientation) || (horizontal < 0 && orientation))
        {
            Flip();
        }
        if(rb.velocity.y < -terminalVelocity)
        {
            rb.gravityScale = 0;
        }
    }

    void Flip()
    {
        orientation = !orientation;
        //animator.SetBool("isTurning", true);
        StartCoroutine("FlipTimer");
    }

    public IEnumerator FlipTimer()
    {
        yield return new WaitForSeconds(turnTime);
        transform.rotation = Quaternion.Euler(0, orientation ? 0 : 180, 0);
        //animator.SetBool("isTurning", false);

    }

    void HandleAttacks()
    {
        animator.SetBool("Attack", false);
        if (sinceAttack <= 0)
        {
            canAttack = true;
        }
        else
        {
            sinceAttack -= Time.deltaTime;
            if (attackCooldown - moveForgiveness > sinceAttack)
            {
                isAttacking = false;
            }
        }
        if (Input.GetButtonDown("Fire1"))
        {
            attack = moveForgiveness;
        }
        if (attack > 0 && canAttack)
        {
            PerformAttack();
        } else if(attack > -moveForgiveness)
        {
            attack -= Time.deltaTime;
        } else
        {
            lastMove = Combo.NO_MOVE;
        }
    }

    void PerformAttack()
    {
        canAttack = false;
        isAttacking = true;
        sinceAttack = attackCooldown;
        slash.SetTrigger("Slash");
        animator.SetTrigger("Attack");
        lightAttackSound.Play();
        if (lastMove == Combo.NO_MOVE)
        {
            lastMove = Combo.LIGHT_ATTACK;
        }
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position + new Vector3(Mathf.Round(movementDirection) * lightAttackDistance, 0, 0), lightAttackRadius, damageable);
        foreach(Collider2D hit in hits)
        {
            if(hit.TryGetComponent<Health>(out Health health))
            {
                health.Damage(status.dmg);
            }
        }
    }

}
