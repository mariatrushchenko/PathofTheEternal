using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    PlayerStats stats;
    Animator animator;
    PlayerCombat pc;
    Player player;
    SpriteRenderer sr;

    // readonly strings for slight optimization
    readonly string Horizontal = "Horizontal";
    readonly string Vertical = "Vertical";
    readonly string LastHorizontal = "LastHorizontal";
    readonly string LastVertical = "LastVertical";
    readonly string Speed = "Speed";

    public bool canMove;    // Used in PlayerCombat.cs
    [HideInInspector] public Vector2 movement;
    [SerializeField] Transform interactor;

    [Header("Knockback")]
    public float knockBackTimerCountdown;
    public float knockBackTimer;
    public float knockBackSpeed;
    public Vector3 enemyFromKnockback;

    [Header("Dashing")]
    [SerializeField] float dashSpeed;
    [SerializeField] float dashTimerC;
    float dashTimer;
    [HideInInspector] public bool isDashing;  // bool is used in PlayerCombat.cs to avoid attacking while dashing
    int direction;

    // this is to prevent constant dash spam
    [SerializeField] float dashIntervalTimerC;
    float dashIntervalTimer;

    private void Start()
    {
        stats = PlayerStats.instance;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        pc = GetComponent<PlayerCombat>();
        player = GetComponent<Player>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (knockBackTimer <= 0)
        {
            // turn invulnerability off (from knockback)
            if (player.isInvulnerable == true)
                player.isInvulnerable = false;

            // some attacks and opening shop menus will stop movement
            if (canMove == true)
            {
                // normalize vector so that moving diagonally doesn't result in faster speeds
                if (isDashing == false)
                {
                    Vector2 movementNormalized = movement.normalized * stats.runSpeed;
                    rb.velocity = new Vector2(movementNormalized.x, movementNormalized.y);
                }

                // -------------------------------
                // dash velocity
                if (dashTimer > 0)
                {
                    // D, U, R, L
                    if (direction == 1)
                        rb.velocity = Vector2.down * dashSpeed;
                    else if (direction == 2)
                        rb.velocity = Vector2.up * dashSpeed;
                    else if (direction == 3)
                        rb.velocity = Vector2.right * dashSpeed;
                    else
                        rb.velocity = Vector2.left * dashSpeed;
                }
                // ------------------------------
            }
        }
        else
            Knockback();
    }

    private void Update()
    {
        if (PauseMenu.GameIsPaused == false)
        {
            if (knockBackTimer <= 0 && canMove == true)
            {
                // avoid getting input when dashing
                if (isDashing == false)
                {
                    // get input from player (values range from -1 to 1)
                    movement.x = Input.GetAxisRaw(Horizontal);
                    movement.y = Input.GetAxisRaw(Vertical);

                    // ----------------------------------------------------------------------------------------
                    // Animation for RUNNING
                    // cache in the values in the animator parameters so animations can be played accordingly
                    animator.SetFloat(Horizontal, movement.x);
                    animator.SetFloat(Vertical, movement.y);

                    // use sqrMagnitude so we don't have to do square root operation per frame (taxing on PC). Results in same effect
                    animator.SetFloat(Speed, movement.sqrMagnitude);

                    // ----------------------------------------------------------------------------------------
                    // do not move facing direction when attacking (feels clunky)
                    if (pc.isAttacking == false)
                    {
                        FaceCorrectPosition();
                        InteractorRotation();
                    }
                }

                DashInput();
            }
        }
    }

    void DashInput()
    {
        // 0 is when player is not dashing and any other # is when they are
        if (direction == 0)
        {
            if (stats.canDash == true && Input.GetKeyDown(KeyCode.E) && dashIntervalTimer <= 0)
            {
                // initialize timer and set bool to true;
                dashTimer = dashTimerC;
                isDashing = true;

                // dash effects
                AudioManager.instance.Play("Player Dash");

                // T -> D: Down, Up, Right, Left
                // Key Thing To Note: The interactor's origin point is down. Positive degrees starts going right
                if (interactor.eulerAngles.z == 0)
                    direction = 1;
                else if (interactor.eulerAngles.z == 180)
                    direction = 2;
                else if (interactor.eulerAngles.z == 90)
                    direction = 3;
                else if (interactor.eulerAngles.z == 270)
                    direction = 4;
            }
            else if (dashIntervalTimer > 0)
            {
                // countdown dash interval timer if above 0.
                dashIntervalTimer -= Time.deltaTime;
            }
        }
        else // countdown dash timer. Movement code in FixedUpdate
        {
            // when timer hits 0, reset values
            if (dashTimer <= 0)
            {
                direction = 0;
                isDashing = false;
                rb.velocity = Vector2.zero;

                // start interval timer
                dashIntervalTimer = dashIntervalTimerC;
            }
            else
                dashTimer -= Time.deltaTime;
        }
    }

    // function is called in FixedUpdate;
    void Knockback()
    {
        // when player dashes into enemy and gets knocked back, cancel dash velocity and perform knock back
        if (isDashing == true)
        {
            dashTimer = 0;
            direction = 0;
            isDashing = false;
            rb.velocity = Vector2.zero;
        }

        knockBackTimer -= Time.fixedDeltaTime;

        Vector2 dir = (transform.position - enemyFromKnockback).normalized * knockBackSpeed;
        rb.velocity = new Vector2(dir.x, dir.y);
    }

    void FaceCorrectPosition()
    {
        // player faces correctly when in idle. This happens because the last movement getaxisraw value is stored when moving which is used
        // to determine idle direction. DONT check when the movement vector is 0 because it will just play the front looking idle anim 
        // (always returns 0)
        if (movement.x != 0 || movement.y != 0)
        {
            animator.SetFloat(LastHorizontal, movement.x);
            animator.SetFloat(LastVertical, movement.y);
        }
    }

    // Note that each input OVERRIDES the previous rotation, meaning it only works for player and not enemies 
    // See BanditMercenary.cs to see interactor code for enemies using rb.velocities
    void InteractorRotation()
    {   
        // Y axis
        if (Input.GetAxisRaw(Vertical) > 0)
            interactor.localRotation = Quaternion.Euler(0, 0, 180);
        else if (Input.GetAxisRaw(Vertical) < 0)
            interactor.localRotation = Quaternion.Euler(0, 0, 0);

        // X Axis
        if (Input.GetAxisRaw(Horizontal) > 0)
            interactor.localRotation = Quaternion.Euler(0, 0, 90);
        else if (Input.GetAxisRaw(Horizontal) < 0)
            interactor.localRotation = Quaternion.Euler(0, 0, -90);
    }
}
