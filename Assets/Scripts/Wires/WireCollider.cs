using System;
using System.Collections.Generic;
using UnityEngine;

namespace Wires{
    public class WireCollider: MonoBehaviour{
        public Wire2 wire;
        public WireIntegration2 integration2;
        public EdgeCollider2D edge;
        private readonly List<Vector2> _points = new();
        private readonly List<Collider2D> _wireBreakers = new();
        private void Start(){
            integration2.GetPositions(_points);
            PositionShift();
            edge.SetPoints(_points);
        }

        private void FixedUpdate(){
            integration2.GetPositions(_points);
            PositionShift();
            edge.SetPoints(_points);
            var res = Physics2D.OverlapCollider(edge,
                new ContactFilter2D(){ useLayerMask = true, layerMask = LayerMask.GetMask("Enemy") }, _wireBreakers);
            if (res > 0){
                for (int i = 0; i < res; i++){
                    var breaker = _wireBreakers[i].GetComponent<IBreakWire>();
                    if (breaker == null) continue;
                    if(wire.HasElectricity) breaker.OnBreakConnectedWire();
                    wire.Break();
                    break;
                }
            }
        }

        private void PositionShift(){ // world to local
            for (var i = 0; i < _points.Count; i++){
                _points[i] = transform.InverseTransformPoint(_points[i]);
            };
        }
    }
}