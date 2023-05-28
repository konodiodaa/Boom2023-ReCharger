using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;
using Wires;


namespace Enemies{
    public class Spiky: MonoBehaviour, IEnemy, IBreakWire, IChargeable{

        public SpriteRenderer renderer;
        public bool IsDead{ private set; get; } = false;

        private Collider2D _col;

        private void Awake(){
            _col = GetComponent<Collider2D>();
        }

        private void FixedUpdate(){
            if (IsDead) return;
            var prev = transform.position;
           // transform.position = prev + Vector3.left * 0.02f;
        }

        public void Die(){
            if (IsDead) return;
            _col.enabled = false;
            IsDead = true;
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
    }
}