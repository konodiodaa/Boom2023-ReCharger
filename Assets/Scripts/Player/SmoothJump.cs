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

    private void FixedUpdate(){
        switch (rb.velocity.y){
            case <= 0 when !pm.isDashing:
                rb.velocity += Vector2.up * (Physics2D.gravity.y * (fall_coe - 1) * Time.fixedDeltaTime);
                break;
            case > 0 when !Input.GetButton("Jump") && !pm.isDashing:
                rb.velocity += Vector2.up * (Physics2D.gravity.y * (lowJump_coe - 1) * Time.fixedDeltaTime);
                break;
        }
    }
}
