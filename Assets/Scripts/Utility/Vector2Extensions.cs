using UnityEngine;

namespace Utility{
    public static class Vector2Extensions{
        public static Vector2 FromRotation(float dag){
            return new(Mathf.Cos(dag * Mathf.Deg2Rad), Mathf.Sin(dag * Mathf.Deg2Rad));
        }
    }
}