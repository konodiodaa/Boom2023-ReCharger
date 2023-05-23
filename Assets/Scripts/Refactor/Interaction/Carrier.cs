using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Refactor.Interaction
{
    public class Carrier : MonoBehaviour
    {
        [SerializeField] private Transform _carryPosition;

        public Carriable CarriedObject { get; private set; }
        
        public void CarryObject(Carriable carriableObject)
        {
            if(carriableObject == null)
            {
                if (_carryPosition)
                {
                    CarriedObject = carriableObject;
                    CarriedObject.AddToCarrier(_carryPosition);
                }
                else
                {
                    CarriedObject = carriableObject;
                    CarriedObject.AddToCarrier(transform);

                }
            }
        }

        public void DropObject()
        {
            if (CarriedObject != null)
            {
                CarriedObject.DropFromCarrier();
                CarriedObject = null;
            }
        }
    }
}
