using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAbility : MonoBehaviour
{
    private Color debugCollisionColor = Color.white;

    [SerializeField]
    private float AttackRange;

    public void Attack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, AttackRange,LayerMask.GetMask("AttackLayer"));
        
        foreach (Collider2D collider in colliders)
        {
            if(collider.tag == "Enemy")
            {
                collider.gameObject.SetActive(false);
            }
        }

    }


    private void OnDrawGizmos()
    {
        Gizmos.color = debugCollisionColor;

        Gizmos.DrawWireSphere((Vector2)transform.position, AttackRange);
    }
}
