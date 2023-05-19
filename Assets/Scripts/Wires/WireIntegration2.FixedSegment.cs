using UnityEngine;

namespace Wires{
    public partial class WireIntegration2{
        public class FixedSegment : IRopeSegment{

            public float Mass{ get; set; } = 1;
            public Vector2 Velocity{ get; set; } = Vector2.zero;
            public Vector2 Position{ get; set; } = Vector2.zero;
            public Vector2 Acceleration => Force * (1 / Mass);
            public IRopeSegment PrevSeg{ set; get; } = null;
            public IRopeSegment NextSeg{ set; get; } = null;

            public Vector2 Force{ set; get; } = Vector2.zero;
            public void UpdateState(float deltaTIme){ }
            public void AvoidCollision(){ }
        }
    }
}