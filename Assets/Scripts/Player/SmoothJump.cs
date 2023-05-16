using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothJump : MonoBehaviour
{
    private PlayerMovement pm;
    private Rigidbody2D rb;
    public float fall_coe = 2.5f;
    public float lowJump_coe = 2.0f;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pm = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rb.velocity.y < 0 && !pm.isDashing)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fall_coe - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump") && !pm.isDashing)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJump_coe - 1) * Time.fixedDeltaTime;
        }
    }
}
