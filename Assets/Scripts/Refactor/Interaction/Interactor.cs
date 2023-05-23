using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Refactor.Interaction
{
    public class Interactor : MonoBehaviour
    {
        [SerializeField] private KeyCode _interactionKey = KeyCode.E;
        [SerializeField] private float _interactRadius = 1.0f;
        

        private Text _interactionTextUI;
        private Interactable _interactable;
        private Carrier _carrier;
        private void Awake()
        {
            _interactionTextUI = transform.Find("Canvas").transform.Find("ChargeHint").GetComponent<Text>();
            _carrier = GetComponent<Carrier>();
        }
        private void Update()
        {
            CheckInteraction();
            UpdateInteractionText();
            if (Input.GetKeyDown(_interactionKey))
            {
                Interact();
            }
        }

        private void CheckInteraction()
        {
            List<Collider2D> possibleObjects= new List<Collider2D>();
            int num = Physics2D.OverlapCircle(transform.position, _interactRadius, new ContactFilter2D(){
                useTriggers = true,
                useLayerMask = true,
                layerMask = LayerMask.GetMask("InteractionLayer")
            }, possibleObjects);
            
            _interactable = null;
            float nearestDist = float.PositiveInfinity;

            for(int i = 0; i < num; i++)
            {
                Interactable interactable = possibleObjects[i].gameObject.GetComponent<Interactable>();
                float dist = Vector3.Distance(transform.position, possibleObjects[i].gameObject.transform.position);
                if (interactable)
                {
                    interactable.EnterInteractZone(this);
                    if (interactable.IsInteractable && dist < nearestDist)
                    {
                        _interactable = interactable;
                        nearestDist = dist;
                    }
                }
            }
        }

        private void UpdateInteractionText()
        {
            if (_interactable)
            {
                _interactionTextUI.gameObject.SetActive(true);
                _interactionTextUI.text = _interactable.InteractionText;
            }
            else if(_carrier.CarriedObject != null)
            {
                _interactionTextUI.gameObject.SetActive(true);
                _interactionTextUI.text = "Press E to Drop";
            }
            else
            {
                _interactionTextUI.gameObject.SetActive(false);
            }

        }

        private void Interact()
        {
            if(_interactable != null)
            {
                _interactable.Interact(this);
                _interactable = null;
            }
            else if(_carrier.CarriedObject != null)
            {
                _carrier.DropObject();
            }
        }

        private void OnDrawGizmos()
        {
            
        }


    }
}
