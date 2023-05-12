using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothJump : MonoBehaviour
{
    private Rigidbody2D rb;
    public float fall_coe = 2.5f;
    public float lowJump_coe = 2.0f;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fall_coe - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJump_coe - 1) * Time.deltaTime;

        }
    }
}
