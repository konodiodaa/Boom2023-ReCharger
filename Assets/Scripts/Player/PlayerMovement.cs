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

    private bool canDoubleJump; // if allow doulbe jump

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
        canDoubleJump = true;
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
            DashMovement();
        }
        CheckDash();
    }

    private void Movement()
    {
        moveInput = Input.GetAxis("Horizontal");
        if(moveInput != 0)
        {
            Debug.Log("Walk");
            GetComponent<Animator>().SetTrigger("Walk");
            faceDir = moveInput > 0 ? 1 : -1;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (coll.onGround)
            {
                Jump(Vector2.up);
                Debug.Log("Jump");
                GetComponent<Animator>().SetTrigger("Jump");
            }
            else if(canDoubleJump && powVol.GetCurrentPower() > 0)
            {
                powVol.PowerChange(-1);
                canDoubleJump = false;
                Jump(Vector2.up);
                Debug.Log("Double Jump");
                GetComponent<Animator>().SetTrigger("Jump");
            }
        }

        if(Input.GetButtonDown("Attack"))
        {
            aab.Charge();
        }

        if (coll.onGround)
        {
            canDoubleJump = true;
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

    private void DashMovement()
    {
        if (Input.GetButtonDown("Dash"))// && powVol.GetCarried() != null) 
        {
            if (Time.time >= (lastDash + dashCooldown) && powVol.GetCurrentPower() > 0)
            {
                StartDash();
                Debug.Log("Dash");
                GetComponent<Animator>().SetTrigger("Dash");
            }
        }
    }
    
    private Coroutine _coroutine;
    private float _prevMass;
    private bool _onRise = false;
    private void Jump(Vector2 dir){
        rb.velocity = new Vector2(rb.velocity.x, 0) + dir * jumpForce;
        _onRise = true;
    }

    private float _prevGravityScale;
    
    private void StartDash(){
        powVol.PowerChange(-1);
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;
        _prevGravityScale = rb.gravityScale;
        rb.gravityScale = 0;
        PlayerAfterImagePool.Instance.GetFromPool();
    }

    private void EndDash(){
        isDashing = false;
        rb.gravityScale = _prevGravityScale;
    }
    
    
    private void CheckDash(){
        if (!isDashing) return;
        if (dashTimeLeft > 0){
            rb.velocity = new Vector2(dashSpeed * faceDir, rb.velocity.y);
            dashTimeLeft -= Time.deltaTime;
        } else{
            EndDash();
        }

        if (!(Mathf.Abs(transform.position.x - lastImageXpos) > dashAfterImageDistance)) return;
        PlayerAfterImagePool.Instance.GetFromPool();
        lastImageXpos = transform.position.x;

    }

    public bool IsOnRise => _onRise;
}
