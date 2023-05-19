using UnityEngine;

namespace Wires{
    public interface IRopeSegment{
        float Mass{ get; }
        Vector2 Velocity{ get; set; }
        Vector2 Position{ get; set; }
        Vector2 Acceleration{ get; }
        Vector2 Force{ set; get; }
        void UpdateState(float deltaTIme);
        
        IRopeSegment PrevSeg{ set; get; }
        IRopeSegment NextSeg{ set; get; }
        void AvoidCollision();
    }
}