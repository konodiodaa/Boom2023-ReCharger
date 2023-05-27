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
            public Vector2 NextPosition{ get; set; } = Vector2.zero;
            public Vector2 Acceleration => Force * (1 / Mass);
            public IRopeSegment PrevSeg{ set; get; } = null;
            public IRopeSegment NextSeg{ set; get; } = null;
            public Vector2 AvoidCollision(Vector2 testPosition){
                var col = Physics2D.OverlapCircle(testPosition, integrator.segColRadius, LayerMask.GetMask("Ground"));
                if (col == null){
                    return testPosition;
                }
                var closestPoint = col.ClosestPoint(testPosition);
                var norm = (testPosition - closestPoint).normalized; 
                return closestPoint + norm * integrator.segColRadius;
            }

            public Collider2D AvoidCollision(Vector2 testPosition, out Vector2 ret){
                var col = Physics2D.OverlapCircle(testPosition, integrator.segColRadius, LayerMask.GetMask("Ground"));
                if (col == null){
                    ret = testPosition;
                    return null;
                }
                var closestPoint = col.ClosestPoint(testPosition);
                var norm = (testPosition - closestPoint).normalized; 
                ret = closestPoint + norm * integrator.segColRadius;
                return col;
            }

            public bool IsFree() => true;

            public Vector2 Force{ set; get; } = Vector2.zero;

            public WireIntegration2 integrator;
            
        }
    }
}