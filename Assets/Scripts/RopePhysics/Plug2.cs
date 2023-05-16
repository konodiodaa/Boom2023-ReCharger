using System;
using System.Collections.Generic;
using UnityEngine;
using Chargers;
using Player;
using UnityEngine;

namespace RopePhysics{
    public class Plug2: MonoBehaviour, IDevice, IInteractable, ICarriable, IRopeSegment{

        public Rigidbody2D body;

        private PowerVolume _powerVolume;

        private bool _hasElec = false;
        public bool HasElectric{ get => _hasElec;
            set{
                _hasElec = value;
                if (Wire == null) return;
                if (value){
                    Wire.GainElectricity();
                } else{
                    Wire.LoseElectricity();
                }
            }
        }
        
        public List<IDevice> ConnectedDevices{ get; set; } = new();

        [NonSerialized]
        public Plug2 OtherEnd;
        
        [NonSerialized]
        public Wire2 Wire;


        public void SetPair(Plug2 other){
            ConnectedDevices.Add(other);
            OtherEnd = other;
        }

        public string GetInstruction(){
            return "Press E to pick up";
        }

        public void Interact(PowerVolume volume){
            Debug.Log("Player pick up Plug!");
            volume.PickUp(this);
        }

        public bool IsCarried{ get; set; } = false;

        public void OnPickUp(PowerVolume powerVolume){
            IsCarried = true;
            body.isKinematic = true;
            _powerVolume = powerVolume;
            gameObject.layer = LayerMask.NameToLayer("Carried"); //LayerMask.GetMask("Carried");
        }

        public void OnDropDown(){
            IsCarried = false;
            body.isKinematic = false;
            _powerVolume = null;
            gameObject.layer =  LayerMask.NameToLayer("InteractionLayer");//LayerMask.GetMask("InteractionLayer");
        }

        public void UpdatePosition(Transform playerTransform){
            body.MovePosition(playerTransform.position + new Vector3(0, 0.5f, 0));
        }

        public float Mass => body.mass;
        public Vector2 Velocity => body.velocity;
        public Vector2 Position => body.position;
        public Vector2 Acceleration => Force * (1/Mass);
        public Vector2 Force{ get; set; } = Vector2.zero;
        public void UpdateState(float deltaTIme){
            body.AddForce(Force);
            if (IsCarried){
                _powerVolume.movement.AddCounterForce(Force);
            }
        }
    }
}