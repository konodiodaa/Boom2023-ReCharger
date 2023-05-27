using System;
using UnityEngine;

namespace Utility{
    public class Outliner: MonoBehaviour{
        [SerializeField]
        private Material outline;
        private SpriteRenderer _renderer;
        private Material _prev;
        
        private void Awake(){
            _renderer = GetComponent<SpriteRenderer>();
            _prev = _renderer.material;
        }

        private void OnEnable(){
            _prev = _renderer.material;
            _renderer.material = outline;
        }

        private void OnDisable(){
            _renderer.material = _prev;
        }
    }
}