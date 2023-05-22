using System.Collections;
using UnityEngine;

namespace Utility{
    public static class SpriteRendererExtensions{
        public static void SetAlpha(this SpriteRenderer renderer, float a){
            var c = renderer.color;
            c.a = a;
            renderer.color = c;
        }

        public static IEnumerator Fade(this SpriteRenderer renderer, float time){
            var start = renderer.color.a;
            yield return AnimUtility.Tween(time, i => renderer.SetAlpha(Mathf.Lerp(start, 0, i)))();
        }

        public static IEnumerator Emerge(this SpriteRenderer renderer, float time, float targetAlpha = 1){
            yield return AnimUtility.Tween(time, i => renderer.SetAlpha(Mathf.Lerp(0, targetAlpha, i)))();
        }
    }
}