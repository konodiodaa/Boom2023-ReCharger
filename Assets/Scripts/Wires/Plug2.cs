using System;
using System.Collections.Generic;
using Chargers;
using Devices;
using UnityEngine;

namespace Wires{
    public class Plug2: MonoBehaviour, IDevice, ICarriable, IRopeSegment{

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

        public bool IsCarried => state == State.Carried;

        private float _prevMass;

        private int _prevLayer;

        public PlugInteraction interaction;

        public void OnPickUp(PowerVolume powerVolume){
            state = State.Carried;
            _powerVolume = powerVolume;
            _prevMass = body.mass;
            body.SetRotation(0);
            body.freezeRotation = true;
            _prevLayer = gameObject.layer;
            interaction.enabled = false;
            gameObject.layer = LayerMask.NameToLayer("NoCollision"); //LayerMask.GetMask("Carried");
            
        }

        public void OnDropDown(){
            state = State.Free;
            body.mass = _prevMass;
            _powerVolume = null;
            body.freezeRotation = false;
            interaction.enabled = true;
            gameObject.layer = _prevLayer; //LayerMask.NameToLayer("");//LayerMask.GetMask("InteractionLayer");
        }

        public void OnPlugIn(Socket socket){
            state = State.Plugged;
            body.isKinematic = true;
            _pluggedSock = socket;
            _prevLayer = gameObject.layer;
            interaction.enabled = false;
            gameObject.layer = LayerMask.NameToLayer("NoCollision");
        }

        public void OnPullOut(Socket socket){
            state = State.Free;
            body.isKinematic = false;
            _pluggedSock = null;
            interaction.enabled = true;
            gameObject.layer = _prevLayer; // LayerMask.NameToLayer("InteractionLayer");
        }

        public float Mass => state == State.Carried ? (_powerVolume.Movement.rb.mass + body.mass) : body.mass;

        public Vector2 Velocity{
            set{
                if (state is State.Plugged){
                    body.velocity = Vector2.zero;
                    return;
                }
                // if (IsCarried) _powerVolume.movement.rb.velocity += value - body.velocity;
                body.velocity = value;
            }
            get => IsCarried ? _powerVolume.Movement.rb.velocity : body.velocity;
        }

        public Vector2 Position{
            set{
                if (state is State.Plugged) return;
                body.MovePosition(value);
            }
            get => body.position;
        }

        public Vector2 NextPosition{ get; set; } = Vector2.zero;
        public Vector2 Acceleration => Force * (1/Mass);
        public Vector2 Force{ get; set; } = Vector2.zero;
        public IRopeSegment PrevSeg{ set; get; } = null;
        public IRopeSegment NextSeg{ set; get; } = null;
        public Vector2 AvoidCollision(Vector2 testPosition){
            return testPosition;
        }

        public bool IsFree(){
            return state == State.Free;
        }

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