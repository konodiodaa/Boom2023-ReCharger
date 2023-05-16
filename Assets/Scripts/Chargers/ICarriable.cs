using UnityEngine;

namespace Chargers{
    public interface ICarriable{
        bool IsCarried{ get; }
        void OnPickUp(PowerVolume powerVolume);
        void OnDropDown();
        void UpdatePosition(Transform playerTransform);
    }
}