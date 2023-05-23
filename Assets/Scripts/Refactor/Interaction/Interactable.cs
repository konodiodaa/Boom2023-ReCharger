using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Refactor.Interaction
{
    public class Interactable : MonoBehaviour
    {
        public string InteractionText = "Press E to interact";
        public bool IsInteractable;
                
        public Action<Interactor> OnInteractableEnter;
        public Action<Interactor> OnInteraction;

        public void EnterInteractZone(Interactor interactor)
        {
            IsInteractable = true;
            OnInteractableEnter?.Invoke(interactor);
        }

        public void Interact(Interactor interactor)
        {
            OnInteraction?.Invoke(interactor);
        }
    }
}
