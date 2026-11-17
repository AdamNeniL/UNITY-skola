using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // General Values
    [Header("General")]
    [HideInInspector] public float time;
    [SerializeField] private string currentAnimation;
    [SerializeField] private Collider2D col;
    [SerializeField] private Vector2 idleOffset;
    [SerializeField] private Vector2 idleSize;
    private List<Collider2D> attackHitboxes = new List<Collider2D>();
    [SerializeField] private Collider2D a1Hitbox;
    [SerializeField] private Collider2D a2Hitbox;
    [SerializeField] private Collider2D a3Hitbox;
    [SerializeField] private Collider2D aAHitbox;
    [SerializeField] private Collider2D aSHitbox;
    [SerializeField] private Vector2 aSOffset;
    [SerializeField] private Vector2 aSSize;
    private Animator anim;
    [HideInInspector] public InputSystem_Actions controls;
    private Rigidbody2D rb;
    private float defaultGravity;

    // Move Values
    [Space(5)]
    [Header("Movement")]
    [SerializeField] private float dashTime = 0.35f;
    [SerializeField] private Vector2 dashOffset;
    [SerializeField] private Vector2 dashSize;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Vector2 walkingOffset;
    [SerializeField] private float isGroundedDistance = 0.1f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private LayerMask groundLayer;
    float moveInput;
    [SerializeField] bool isDashing;
    [SerializeField] bool isGrounded;
    [SerializeField] bool isAttacking;
    [SerializeField] bool doAnotherAttack;
    bool walking = false;
    bool upgraded = false;
    Vector2 bttmOrigin;
    RaycastHit2D bttmHit;

    [SerializeField] float dashCooldown = 1.5f;
    bool canDash = true;
    bool airDashed = false;

    [Space(5)]
    [Header("Attack")]
    [SerializeField] private int combo;
    [SerializeField][Range(0, 1)] private float attackDelay;

    private void Awake()
    {
        // General Actions
        controls = new InputSystem_Actions();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        defaultGravity = rb.gravityScale;

        // Controls
        controls.Player.Dash.started += ctx => Dash();
        controls.Player.Jump.started += ctx => Jump();
        controls.Player.Attack.started += ctx => Attack();

        // Hitboxes
        if (a1Hitbox != null || a2Hitbox != null || a3Hitbox != null || aAHitbox != null || a3Hitbox != null)
        {
            attackHitboxes.Add(a1Hitbox);
            attackHitboxes.Add(a2Hitbox);
            attackHitboxes.Add(a3Hitbox);
            attackHitboxes.Add(aAHitbox);
            attackHitboxes.Add(aSHitbox);
            foreach (Collider2D col in attackHitboxes)
            {
                col.enabled = false;
                col.isTrigger = true;
            }
        }
    }

    private void Start()
    {
        dashCooldown = 1.5f;
        dashCooldown = Mathf.Clamp(dashCooldown - (Global.dashUpgrades * 0.25f), 0.36f, 1.5f);
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        time += Time.deltaTime;
        UpdateValues();
        Walk();
    }

    private void UpdateValues()
    {
        // Flip
        if (moveInput != 0) transform.eulerAngles = new Vector2(0, Mathf.Clamp(moveInput, -1, 0) * 180);
        // Grounded checks
        bttmOrigin = new Vector2(col.bounds.center.x, col.bounds.min.y - 0.05f);
        Debug.DrawRay(bttmOrigin, Vector2.down * isGroundedDistance, Color.red);
        bttmHit = Physics2D.Raycast(bttmOrigin, Vector2.down, isGroundedDistance, groundLayer);
        isGrounded = bttmHit.collider != null;
        // Gravity Fix
        if (!isDashing && rb.gravityScale == 0)
        {
            rb.gravityScale = defaultGravity;
        }
        // Attack movement lock
        if (isAttacking && isGrounded) rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        if (isGrounded) airDashed = false;
    }

    private void ChangeAnimation(string _name)
    {
        if (currentAnimation == _name) return;

        currentAnimation = _name;

        anim.Play(currentAnimation);
    }

    private void Walk()
    {
        // Dash block
        if (isDashing) return;
        // Walking
        moveInput = Mathf.Round(controls.Player.Movement.ReadValue<float>());
        if (!isGrounded && Global.jumpUpgrade && !upgraded)
        {
            moveSpeed *= 1.5f;
            upgraded = true;
        }
        if (isGrounded && Global.jumpUpgrade && upgraded)
        {
            moveSpeed /= 1.5f;
            upgraded = false;
        }
        if (!isAttacking) rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        // Collider offset
        if (moveInput != 0 && !isDashing && !isAttacking)
        {
            ChangeAnimation("Walk");
            while (!walking)
            {
                col.offset += walkingOffset;
                walking = true;
            }
        }
        else if (!isDashing && isGrounded && !isAttacking)
        {
            ChangeAnimation("Idle");
            col.offset = idleOffset;
            walking = false;
        }
    }

    private void Dash()
    {
        if (!canDash) return;
        if (!isGrounded && airDashed) return;

        if (moveInput != 0 && !isAttacking && !isDashing)
        {
            if (!isGrounded) airDashed = true;
            isDashing = true;
            canDash = false;
            StartCoroutine(DashCoroutine());
            StartCoroutine(DashReset());
        }
    }

    IEnumerator DashCoroutine()
    {
        float temp = rb.gravityScale;
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(new Vector2(dashForce * moveInput, 0));
        ChangeAnimation("Dash");
        yield return new WaitForSecondsRealtime(dashTime);
        rb.gravityScale = temp;
        isDashing = false;
    }

    IEnumerator DashReset()
    {
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void Jump()
    {
        // called once
        if (isGrounded && !isDashing && !isAttacking)
        {
            rb.AddForce(new Vector2(0, jumpForce * 100));
        }
    }

    private void WallSlide()
    {

    }

    private void WallJump()
    {

    }

    private void Attack()
    {
        // called once
        if (isAttacking && combo > 0) { doAnotherAttack = true; }

        isAttacking = true;
        if (!doAnotherAttack && isGrounded && combo == 0) { ChangeAnimation("Attack1"); combo++; }
        if (!isGrounded) { ChangeAnimation("AirAttack"); }
    }

    private void EndAttack()
    {
        isAttacking = false;

        if (doAnotherAttack)
        {
            switch (combo)
            {
                case 1:
                    ChangeAnimation("Attack2");
                    isAttacking = true;
                    doAnotherAttack = false;
                    combo++;
                    return;
                case 2:
                    ChangeAnimation("Attack3");
                    isAttacking = true;
                    doAnotherAttack = false;
                    combo = 0;
                    return;
            }
        }
        ChangeAnimation("Idle");
        combo = 0;
        doAnotherAttack = false;
    }
}
