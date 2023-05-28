using System;
using System.Collections;
using Devices;
//using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

namespace Wires{
    public class Wire2 : MonoBehaviour{

        public GameObject plugPref;
        
        private readonly Plug2[] _plugs = { null, null };
        
        public WireIntegration2 _integration2;

        public Transform endPosition;

        public Socket[] connectedSockets;



        private IEnumerator  Start(){
            CreatePlugs();
            yield return new WaitForFixedUpdate();
            CreateRope();
            if (connectedSockets.Length > 0){
                yield return new WaitForFixedUpdate();
                connectedSockets[0].PlugIn(_plugs[0]);
            }

            if (connectedSockets.Length > 1){
                yield return new WaitForFixedUpdate();
                connectedSockets[1].PlugIn(_plugs[1]);  
            }
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

        public bool HasElectricity => _plugs[0].HasElectric;

        public void Break(){
            IDevice.Disconnect(_plugs[0], _plugs[1]);
            Destroy(gameObject);
        }

        private void OnDrawGizmos(){
            Gizmos.DrawSphere(transform.position, 0.2f);
            Gizmos.DrawSphere(endPosition.position, 0.2f);
        }
    }
}