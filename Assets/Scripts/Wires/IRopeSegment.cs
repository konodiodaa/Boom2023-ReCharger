using UnityEngine;

namespace Wires{
    public interface IRopeSegment{
        float Mass{ get; }
        Vector2 Velocity{ get; set; }
        Vector2 Position{ get; set; }
        Vector2 NextPosition{ get; set; }
        Vector2 Force{ set; get; }
        IRopeSegment PrevSeg{ set; get; }
        IRopeSegment NextSeg{ set; get; }

        /// <summary>
        /// Put testPosition into test, output the final position
        /// </summary>
        /// <param name="testPosition"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        Collider2D AvoidCollision(Vector2 testPosition, out Vector2 ret);
        bool IsFree();
    }
}