using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using Wires;


namespace Enemies{
    public class Spiky: MonoBehaviour, IEnemy, IBreakWire, IChargeable{

        public SpriteRenderer renderer;

        public bool isDead = false;
        
        private void FixedUpdate(){
            if (isDead) return;
            var prev = transform.position;
            transform.position = prev + Vector3.left * 0.02f;
        }

        public void Die(){
            if (isDead) return;
            isDead = true;
            var c = renderer.color;
            renderer.color = c;
            var tween = AnimUtility.Tween(0.2f, i => {
                c.a = Mathf.Lerp(1, 0, i);
                renderer.color = c;
            }, null, () => {
                c.a = 0;
                renderer.color = c;
                Destroy(this);
            });
            StartCoroutine(tween());
        }

        public void OnBreakConnectedWire(){
            Die();
        }

        public void GetCharge(){
            Die();
        }

        private void OnCollisionEnter2D(Collision2D other){
            if (string.Compare(other.gameObject.tag, "Player", StringComparison.Ordinal) != 0) return;
            // Player fail!
        }
    }
}