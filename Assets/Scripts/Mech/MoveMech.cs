using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMech : Mech
{
    [Header("MovePlane")]
    public float distance;
    public float speed;

    private float cur_dist;
    private bool reverse;

    private void Awake()
    {
        cur_dist = 0;
        reverse = false;
    }

    private void Update()
    {
        if(isActivate)
        {
            float x = Time.deltaTime * speed;
            cur_dist += x;
            if(!reverse)
                transform.Translate(x, 0f, 0f);
            else
               transform.Translate(-x, 0f, 0f);

            if(cur_dist >= distance)
            {
                cur_dist = 0;
                reverse = !reverse;
            }
        }
    }

}
