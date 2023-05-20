using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player{
    public class PlayerCharge : MonoBehaviour
    {
        private readonly Color _debugCollisionColor = Color.white;

        [FormerlySerializedAs("attackRange")] [FormerlySerializedAs("AttackRange")] [SerializeField]
        private float chargeRange;

        public PowerVolume volume;
        public SpriteRenderer spr;
        
        [SerializeField]
        private float _cd = 0.2f;
        private float _timer = 0f;

        private readonly List<Collider2D> _targetColliders = new(); 
        public void Charge(){
            if (volume.GetCurrentPower() == 0) return;
            if (_timer != 0) return;
            volume.PowerCurrent -= 1;
            StartCoroutine(ChargeAnim());
            var num = Physics2D.OverlapCircle(transform.position, chargeRange, new(), _targetColliders);
            if (num == 0) return;
            foreach (var col in _targetColliders){
                if (col.GetComponent<IChargeable>() is not { } chargeable) return;
                chargeable.GetCharge();
            }
        }

        private IEnumerator ChargeAnim(){
            _timer = 0;
            var c = spr.color;
            c.a = 0;
            spr.color = c;
            for (; _timer <= _cd/2; _timer += Time.deltaTime){
                c.a = Mathf.Lerp(0, 1, _timer / _cd * 2);
                spr.color = c;
                yield return null;
            }

            for (; _timer <= _cd/2; _timer += Time.deltaTime){
                c.a = Mathf.Lerp(1, 0, _timer / _cd * 2);
                spr.color = c;
                yield return null;
            }
            
            _timer = 0;
            c.a = 0;
            spr.color = c;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _debugCollisionColor;

            Gizmos.DrawWireSphere((Vector2)transform.position, chargeRange);
        }
    }
}
