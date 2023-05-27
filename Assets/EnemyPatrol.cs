using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField]
    private float leftOffset;
    [SerializeField]
    private float rightOffset;

    [SerializeField]
    private float patrolSpeed;
    [SerializeField]
    private bool isPatrolling = true;

    private bool isReverse;

    private Vector3 left;
    private Vector3 right;
    private void Awake()
    {
        left = transform.position - new Vector3(leftOffset,0,0);
        right = transform.position + new Vector3(rightOffset,0,0);
    }

    private void Update()
    {
        if (isPatrolling)
        {
            Vector3 direction;
            if (isReverse)
            {
                if((transform.position-left).magnitude < 0.1f)
                {
                    isReverse = !isReverse;
                    return;
                }

                direction = (left - transform.position).normalized;
            }
            else
            {
                if ((transform.position - right).magnitude < 0.1f)
                {
                    isReverse = !isReverse;
                    return;
                }
                direction = (right - transform.position).normalized;
            }

            transform.Translate(direction * patrolSpeed * Time.deltaTime);
       
        }
    }
}
