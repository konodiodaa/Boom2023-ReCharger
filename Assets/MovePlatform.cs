using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    private Mech mech;

    [Header("MovePlane")]
    public float distance;
    public float speed;

    private float cur_dist;
    private bool reverse;

    private void Awake()
    {
        cur_dist = 0;
        reverse = false;
        mech = transform.Find("MoveMech").GetComponent<Mech>();
    }

    private void Update()
    {
        if (mech.getActivate())
        {
            float x = Time.deltaTime * speed;
            cur_dist += x;
            if (!reverse)
                transform.Translate(x, 0f, 0f);
            else
                transform.Translate(-x, 0f, 0f);

            if (cur_dist >= distance)
            {
                cur_dist = 0;
                reverse = !reverse;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (mech.getActivate())
        {
            float x = Time.deltaTime * speed;
            if (!reverse)
                collision.transform.Translate(x, 0f, 0f);
            else
                collision.transform.Translate(-x, 0f, 0f);
        }

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(mech.getActivate())
        {
            float x = Time.deltaTime * speed;
            if (!reverse)
                collision.transform.Translate(x, 0f, 0f);
            else 
                collision.transform.Translate(-x, 0f, 0f);
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (mech.getActivate())
        {
            float x = Time.deltaTime * speed;
            if (!reverse)
                collision.transform.Translate(x, 0f, 0f);
            else
                collision.transform.Translate(-x, 0f, 0f);
        }

    }
}
