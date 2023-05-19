using UnityEngine;

namespace Wires{
    public partial class WireIntegration2{
        private class RopeSegment: IRopeSegment{
            public RopeSegment(Vector3 initPos){
                Position = initPos;
            }

            public float Mass => integrator.segmentMass;
            public Vector2 Velocity{ get; set; } = Vector2.zero;
            public Vector2 Position{ get; set; }
            public Vector2 Acceleration => Force * (1 / Mass);
            public IRopeSegment PrevSeg{ set; get; } = null;
            public IRopeSegment NextSeg{ set; get; } = null;

            public Vector2 Force{ set; get; } = Vector2.zero;

            public WireIntegration2 integrator;
            

            public void AvoidCollision(){
                var col = Physics2D.OverlapCircle(Position, integrator.segColRadius, LayerMask.GetMask("Ground"));
                if (col == null){
                    return;
                }
                var closestPoint = col.ClosestPoint(Position);
                var norm = (Position - closestPoint).normalized;
                Position = closestPoint + norm * integrator.segColRadius;
                Velocity -= Vector2.Dot(norm, Velocity) * norm;
                Force -= Vector2.Dot(norm, Force) * norm;
            }

            public void UpdateState(float deltaTIme){
                Velocity += Force * (deltaTIme / Mass);
                Position += deltaTIme * Velocity;
            }
        }
    }
}