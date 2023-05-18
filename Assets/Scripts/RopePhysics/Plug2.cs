using System;
using System.Collections.Generic;
using UnityEngine;
using Chargers;
using Devices;
using Player;
using UnityEngine;

namespace RopePhysics{
    public class Plug2: MonoBehaviour, IDevice, IInteractable, ICarriable, IRopeSegment{

        public enum State{
            Free,
            Plugged,
            Carried
        }

        public State state = State.Free;

        public Rigidbody2D body;

        private PowerVolume _powerVolume;

        private Socket _pluggedSock;

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

        public string GetInstruction(PowerVolume volume){
            return state switch{
                State.Free => volume.GetCarried() == null ? "Press E to Pick Up" : null,
                State.Carried => null,
                State.Plugged => null
            };
        }

        public void Interact(PowerVolume volume){
            switch (state){
                case State.Free:
                    if (volume.GetCarried() != null) return;
                    Debug.Log("Player pick up Plug!");
                    volume.PickUp(this);
                    break;
                case State.Carried:
                    break;
                case State.Plugged:
                    break;
            }
        }

        public bool IsCarried => state == State.Carried;

        private float _prevMass;

        public void OnPickUp(PowerVolume powerVolume){
            state = State.Carried;
            // body.isKinematic = true;
            _powerVolume = powerVolume;
            _prevMass = body.mass;
            body.mass = 0.01f;
            body.SetRotation(0);
            body.freezeRotation = true;
            gameObject.layer = LayerMask.NameToLayer("Carried"); //LayerMask.GetMask("Carried");
            
        }

        public void OnDropDown(){
            state = State.Free;
            // body.isKinematic = false;
            body.mass = _prevMass;
            _powerVolume = null;
            body.freezeRotation = false;
            gameObject.layer =  LayerMask.NameToLayer("InteractionLayer");//LayerMask.GetMask("InteractionLayer");
        }

        public void OnPlugIn(Socket socket){
            state = State.Plugged;
            body.isKinematic = true;
            _pluggedSock = socket;
            gameObject.layer = LayerMask.NameToLayer("Plugged");
        }

        public void OnPullOut(Socket socket){
            state = State.Free;
            body.isKinematic = false;
            _pluggedSock = null;
            gameObject.layer = LayerMask.NameToLayer("InteractionLayer");
        }

        public float Mass => state == State.Carried ? _powerVolume.movement.rb.mass : body.mass;

        public Vector2 Velocity{
            set{
                if (state is State.Plugged)
                    return;
                body.velocity = value;
            }
            get{
                if (state is State.Carried) return _powerVolume.movement.rb.velocity;
                return body.velocity;
            }
        }

        public Vector2 Position{
            set{
                if (state is State.Plugged) return;
                body.MovePosition(value);
            }
            get => body.position;
        }
        public Vector2 Acceleration => Force * (1/Mass);
        public Vector2 Force{ get; set; } = Vector2.zero;
        public void UpdateState(float deltaTIme){
            if(state is State.Free) body.AddForce(Force);
            else if(state is State.Carried && !_powerVolume.movement.IsOnRise) body.AddForce(Force);
        }
        
        public IRopeSegment PrevSeg{ set; get; } = null;
        public IRopeSegment NextSeg{ set; get; } = null;

        public void AvoidCollision(){ }

        private void FixedUpdate(){
            if (state == State.Plugged){
                body.MovePosition(_pluggedSock.transform.position);
            }
        }

        public Vector2 DistanceDirection => (PrevSeg == null ? (Position - NextSeg.Position) : (PrevSeg.Position - Position) ).normalized;

        public Rigidbody2D Body => body;
    }
}