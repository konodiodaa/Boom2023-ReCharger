using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wires;


namespace Enemies{
    public class Spiky: MonoBehaviour, IEnemy, IBreakWire{
        private void FixedUpdate(){
            var prev = transform.position;
            transform.position = prev + Vector3.left * 0.02f;
        }

        public void BeingAttacked(PowerVolume volume){
            
        }

        public void Die(){
            Destroy(gameObject);
        }

        public void OnBreakConnectedWire(){
            Die();
        }

        private void OnCollisionEnter2D(Collision2D other){
            Debug.Log("Collide!");
        }

        private void OnTriggerEnter2D(Collider2D other){
            Debug.Log("Trigger!");
        }
    }
}