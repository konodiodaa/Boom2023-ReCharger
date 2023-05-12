using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Collision coll;
    private PowerVolume powVol;

    [HideInInspector]
    public Rigidbody2D rb;

    [Space]
    [Header("Stats")]
    public float speed;
    public float jumpForce;


    void Awake()
    {
        coll = GetComponent<Collision>();
        powVol = GetComponent<PowerVolume>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        AbilityUpdateAlongPower();
        Movement();
    }

    private void Movement()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector2 dir = new Vector2(x, y);

        Walk(dir);

        if (Input.GetButtonDown("Jump"))
        {
            if (coll.onGround)
            {
                Jump(Vector2.up);
            }
        }
    }

    private void AbilityUpdateAlongPower()
    {
        if(powVol.GetPower() > 1)
        {
            jumpForce = 18;
        }
        else
        {
            jumpForce = 12;
        }
    }

    private void Walk(Vector2 dir)
    {
        rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);

    }

    private void Jump(Vector2 dir)
    {

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;
    }
}
