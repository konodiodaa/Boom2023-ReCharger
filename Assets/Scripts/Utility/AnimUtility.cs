using System;
using System.Collections;
using UnityEngine;

namespace Utility{
    public static class AnimUtility{
        public static Func<IEnumerator> Tween(float totalTime, Action<float> update, Action start = null,
            Action end = null){
            IEnumerator Inner(){
                start?.Invoke();
                for (var cur = 0f; cur < totalTime; cur += Time.deltaTime){
                    update(cur / totalTime);
                    yield return null;
                }
                update(1);
                end?.Invoke();
            }
            return Inner;
        }
    }
}