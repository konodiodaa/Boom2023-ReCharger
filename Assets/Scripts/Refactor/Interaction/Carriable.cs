using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Refactor.Interaction
{
    public class Carriable : MonoBehaviour
    {
        [SerializeField] private Vector3 _carryPositionOffset;
        private Transform _carrier;
        private Vector3 _carrierLastFramePosition;

        private void LateUpdate()
        {
            if (_carrier)
            {
                transform.position += _carrier.position - _carrierLastFramePosition;
                _carrierLastFramePosition = _carrier.position;
            }
        }

        public void AddToCarrier(Transform carrier)
        {
            transform.SetParent(carrier);
            transform.localPosition += _carryPositionOffset;
            _carrier = carrier;
        }

        public void DropFromCarrier()
        {
            transform.parent = null;
            _carrier = null;
        }
    }
}
