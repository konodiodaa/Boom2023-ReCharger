using System;
using UnityEngine;

namespace RopePhysics{
    public class WireSegment: MonoBehaviour{
        private BoxCollider2D _col;
        private Rigidbody2D _body;

        private HingeJoint2D _joint;

        public WireSegment prev = null;
        public WireSegment next = null;

        public float Length => _col.size.x * transform.localScale.x;

        private void Awake(){
            _col = GetComponent<BoxCollider2D>();
            _body = GetComponent<Rigidbody2D>();
            _joint = GetComponent<HingeJoint2D>();
        }

        public void SetPrevSegment(WireSegment newPrev){
            prev = newPrev;
            _joint.connectedBody = prev._body;
        }

        public void SetNextSegment(WireSegment newNext){
            next = newNext;
        }
    }
}