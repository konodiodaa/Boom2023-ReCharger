using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utility{
    public static class CanvasGroupExtensions{
        public static IEnumerator Fade(this CanvasGroup renderer, float time){
            var start = renderer.alpha;
            yield return AnimUtility.Tween(time, i => renderer.alpha = Mathf.Lerp(start, 0, i))();
        }

        public static IEnumerator Emerge(this CanvasGroup renderer, float time, float targetAlpha = 1){
            yield return AnimUtility.Tween(time, i => renderer.alpha = (Mathf.Lerp(0, targetAlpha, i)))();
        }
    }
}