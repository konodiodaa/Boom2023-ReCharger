using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using Utility;
using Wires;

public class PlayerMovement : MonoBehaviour
{
    private Collision coll;
    private PowerVolume powVol; // power volume of player
    private PlayerCharge aab;

    [HideInInspector]
    public Rigidbody2D rb;

    [Space]
    [Header("Jump properties")]
    public float speed;
    public float jumpForce;

    private bool doubleJump; // if allow doulbe jump

    [Space]
    [Header("Dash properties")]
    [SerializeField]
    private float dashDistance;
    [SerializeField]
    private float dashSpeed;
    [SerializeField]
    private float dashTime;
    [SerializeField]
    private float dashAfterImageDistance;
    [SerializeField]
    private float dashCooldown; // set as same as dashTime

   // [HideInInspector]
    public bool isDashing;

    [SerializeField]
    private float dashTimeLeft;
    private float lastImageXpos;
    [SerializeField]
    private float lastDash = -100f;


    private float moveInput;
    private int faceDir;

    private Vector2 _counterForce = Vector2.zero;
    public float counterForceFactor = 1f;

    private Vector2 _totalForce = Vector2.zero;


    void Awake()
    {
        doubleJump = true;
        coll = GetComponent<Collision>();
        powVol = GetComponent<PowerVolume>();
        aab = GetComponent<PlayerCharge>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(!isDashing)
        {
            Movement();
            Dash();
        }
        if(isDashing)
            CheckDash();
    }

    private void Movement()
    {
        moveInput = Input.GetAxis("Horizontal");
        if(moveInput != 0)
        {
            faceDir = moveInput > 0 ? 1 : -1;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (coll.onGround)
            {
                Jump(Vector2.up);
            }
            else if(doubleJump && powVol.GetCurrentPower() > 0)
            {
                powVol.PowerChange(-1);
                doubleJump = false;
                Jump(Vector2.up);
            }
        }

        if(Input.GetButtonDown("Attack"))
        {
            aab.Charge();
        }

        if (coll.onGround)
        {
            doubleJump = true;
        }
    }
    
    private void FixedUpdate()
    {
        // Move the character
        if (!isDashing)
            rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        if (_onRise){
            if (rb.velocity.y < 0) _onRise = false;
        }
    }

    private void Dash()
    {
        if (Input.GetButtonDown("Dash") && powVol.GetCarried() != null) 
        {
            if (Time.time >= (lastDash + dashCooldown) && powVol.GetCurrentPower() > 1)
                Attempt2Dash();
        }
    }
    
    private Coroutine _coroutine;
    private float _prevMass;
    private bool _onRise = false;
    private void Jump(Vector2 dir){
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;
        _onRise = true;
    }
    
    private void Attempt2Dash()
    {
        powVol.PowerChange(-1);
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;

        PlayerAfterImagePool.Instance.GetFromPool();
    }

    private void CheckDash()
    {
        if (isDashing)
        {
            if (dashTimeLeft > 0)
            {
                rb.gravityScale = 0;
                rb.velocity = new Vector2(dashSpeed * faceDir, rb.velocity.y);
                dashTimeLeft -= Time.deltaTime;
            }

            if (Mathf.Abs(transform.position.x - lastImageXpos) > dashAfterImageDistance)
            {
                PlayerAfterImagePool.Instance.GetFromPool();
                lastImageXpos = transform.position.x;
            }

            if (dashTimeLeft < 0)
            {
                rb.gravityScale = 3;
                isDashing = false;
            }
        }
    }

    public bool IsOnRise => _onRise;
}
