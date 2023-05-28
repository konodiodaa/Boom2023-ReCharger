using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemies{
    public class Patrol: MonoBehaviour{
        public Transform endTrans;
        [Min(0.01f)]
        public float speed;
        
        
        private Vector2 _dir;
        private Vector2 _start;
        private Vector2 _end;
        public bool isReverse;

        private float _totalTime;
        private float _curTime = 0;

        private void Awake(){
            _start = transform.position;
            _end = endTrans.position;
            var dist = _end - _start; 
            _dir = dist.normalized;
            _totalTime = dist.magnitude / speed;
        }

        private void FixedUpdate(){
            var i = _curTime / _totalTime;
            i = Mathf.Clamp(i, 0, 1);

            var cur = Vector2.Lerp(_start, _end, i);
            transform.position = new(cur.x, cur.y, transform.position.z);

            _curTime += Time.fixedDeltaTime * (isReverse ? -1 : 1);
            if (_curTime > _totalTime){
                _curTime = _totalTime;
                isReverse = true;
            } else if (_curTime < 0){
                _curTime = 0;
                isReverse = false;
            }
        }
    }
}