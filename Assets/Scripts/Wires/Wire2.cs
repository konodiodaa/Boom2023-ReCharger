using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wires;

namespace RopePhysics{
    public class Wire2 : MonoBehaviour{

        public GameObject plugPref;
        
        private readonly Plug2[] _plugs = { null, null };
        
        public WireIntegration2 _integration2;

        public Transform endPosition;

        public float maxLengthFactor = 3f;


        private IEnumerator  Start(){
            CreatePlugs();
            yield return new WaitForFixedUpdate();
            CreateRope();
        }

        public Vector2 GetStartPosition(){
            return _plugs[0].Position;
        }

        public Vector2 GetEndPosition(){
            return _plugs[1].Position;
        }

        private void CreateRope(){
            _integration2.CreateSegments(_plugs[0], _plugs[1]);
        }

        private void CreatePlugs(){
            var inst = Instantiate(plugPref, transform, false);
            inst.transform.localPosition = Vector3.zero;
            _plugs[0] = inst.GetComponent<Plug2>();

            inst = Instantiate(plugPref, transform, false);
            inst.transform.position = endPosition.position; 
            if ((endPosition.position - transform.position).magnitude > _integration2.GetMaxLength() * maxLengthFactor){
                inst.transform.localPosition = (endPosition.position - transform.position).normalized *
                                               (_integration2.GetMaxLength() * maxLengthFactor);
            }
            _plugs[1] = inst.GetComponent<Plug2>();
            
            _plugs[0].SetPair(_plugs[1]);
            _plugs[1].SetPair(_plugs[0]);
            _plugs[0].Wire = this;
            _plugs[1].Wire = this;
        }

        public void GainElectricity(){
            _integration2.SetColor(Color.yellow);
        }

        public void LoseElectricity(){
            _integration2.SetColor(Color.black);
        }

        public float CurrentLength => _integration2.CurrentLength;

        public float MaxLength => _integration2.GetMaxLength();

        public bool ReachingMaxLength => CurrentLength >= MaxLength;
    }
}