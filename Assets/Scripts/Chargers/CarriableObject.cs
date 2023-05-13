using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarriableObject : MonoBehaviour
{

    private Collision coll; // check if on ground
    private Rigidbody2D rb;

    [HideInInspector]
    public Transform target;

    private void Awake()
    {
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (target != null )
        {
            if (target.tag == "Player")
                transform.position = target.position + new Vector3(0, 0.5f, 0);
            else
                transform.position = target.position;
        }
        else if (coll.onGround)
        {
            rb.velocity = Vector3.zero;
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = 1;
        }
    }

    public void SetTargetNull()
    {
        target = null;
        gameObject.layer = LayerMask.NameToLayer("InteractionLayer");
    }

    public void BeCarried(Transform target)
    {
        this.target = target;
        gameObject.layer = LayerMask.NameToLayer("ActivatedInteractionLayer");
    }
}
