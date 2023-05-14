using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RopePhysics{
    public class Wire: MonoBehaviour{

        [Min(1)]
        public int segCount = 3;

        public GameObject segPref;

        public List<WireSegment> segments = new ();

        public bool ReachedMaximumLength(){
            return false;
        }

        private void Awake(){
            WireSegment prev = null;
            var cur = transform.position;
            for (var i = 0; i < segCount; i++){
                var o = Instantiate(segPref, transform, false);
                o.transform.position = cur;
                segments.Add(o.GetComponent<WireSegment>());
                if(prev != null) prev.SetNextSegment(segments[^1]);
                prev = segments[^1];
                cur.x += prev.Length;
            }
        }
        
        
    }
}