using System;
using System.Collections;
using System.Collections.Generic;
using Chargers;
using Player;
using UnityEngine;
using UnityEngine.UI;

public class PowerVolume : MonoBehaviour
{

    [SerializeField]
    [Header("Power")]
    private int PowerRequired;
    public int PowerCurrent;

    private Text Power_text;
    private Text hint_text;

    private bool interactionAva;

    private bool isCarried;
    private CarriableObject co;

    [SerializeField]
    private Collider2D targetCollider;

    private ICarriable _carriable;

    public PlayerMovement movement;
    
    private void Awake()
    {
        interactionAva = false;
        movement = GetComponent<PlayerMovement>();
        Power_text = transform.Find("Canvas").transform.Find("PowerText").GetComponent<Text>();
        hint_text = transform.Find("Canvas").transform.Find("ChargeHint").GetComponent<Text>();
    }

    private void Update()
    {
        Power_text.text = PowerCurrent.ToString();
        winCheck();
        
        // Move To Fixed Update?
        CollisionCheck(); // get interactable object
        CollisionHandler(); // handle collision
        CollisionInteraction(); // handle interaction

        if(targetCollider == null && Input.GetButtonDown("Interact") && isCarried )
        {
            isCarried = false;
            co.GetComponent<CarriableObject>().SetTargetNull();
        }
        
    }

    private void FixedUpdate(){
        _carriable?.UpdatePosition(transform);
    }

    public void PowerChange(int value)
    {
        PowerCurrent += value;
    }

    private void CollisionCheck()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, 1.0f,LayerMask.GetMask("InteractionLayer"));
        if (collider != null)
            targetCollider = collider;
        else
            targetCollider = null;

    }

    private void CollisionHandler()
    {
        if (_carriable != null){
            return;
        }
        if (targetCollider == null)
        {
            hint_text.gameObject.SetActive(false);
            return;
        }

        if (targetCollider.tag == "Charger")
        {
            interactionAva = true;
            hint_text.gameObject.SetActive(true);
            hint_text.text = "Press E to Recharge";
        }
        else if (targetCollider.tag == "Mech")
        {
            interactionAva = true;
            hint_text.gameObject.SetActive(true);
            hint_text.text = "Press E to Charge the Mech";
        }
        else if (targetCollider.tag == "PlugHead" && !isCarried)
        {
            interactionAva = true;
            hint_text.gameObject.SetActive(true);
            hint_text.text = "Press E Carry the Plug";
        }
        else if (targetCollider.tag == "Socket" && isCarried && co != null)
        {
            interactionAva = true;
            hint_text.gameObject.SetActive(true);
            hint_text.text = "Press E to Insert Plug";
        } 
        else if (targetCollider.tag == "LinkedPlug" && isCarried && co != null)
        {
            interactionAva = true;
            hint_text.gameObject.SetActive(true);
            hint_text.text = "Press E to Insert Plug";
        }
        else if (targetCollider.tag == "Switch" && isCarried && co != null)
        {
            interactionAva = true;
            hint_text.gameObject.SetActive(true);
            hint_text.text = "Press E to Insert Plug";
        }else{
            var interactable = targetCollider.GetComponent<IInteractable>();
            if (interactable is not{ }) return;
            interactionAva = true;
            hint_text.gameObject.SetActive(true);
            hint_text.text = interactable.GetInstruction();
        }
    }

    private void CollisionInteraction()
    {
        if (!interactionAva || targetCollider == null) return;

        if (Input.GetButtonUp("Interact")){
            if (targetCollider.tag == "Mech"){
                int required = targetCollider.transform.GetComponent<Mech>().GetRequired();
                if (required <= PowerCurrent && !targetCollider.transform.GetComponent<Mech>().getActivate()){
                    PowerCurrent -= required;
                    targetCollider.transform.GetComponent<Mech>().ReCharge();
                }
            } else if (targetCollider.tag == "Charger"){
                PowerCurrent += targetCollider.transform.GetComponent<Charger>().DisCharge();
                PowerCurrent = Mathf.Clamp(PowerCurrent, 0, PowerRequired);
            } else if (targetCollider.tag == "PlugHead" && !isCarried){
                co = targetCollider.transform.GetComponent<CarriableObject>();
                co.BeCarried(transform);
                isCarried = true;
            } else if (targetCollider.tag == "Socket" && isCarried && co != null){
                PlugHead ph = co.GetComponent<PlugHead>();
                if (ph != null){
                    isCarried = false;
                    targetCollider.gameObject.layer = LayerMask.NameToLayer("ActivatedInteractionLayer");
                    co.target = targetCollider.transform;
                    ph.SetConnected();
                }
            } else if (targetCollider.tag == "LinkedPlug" && isCarried && co != null)
            {
                PlugHead ph = co.GetComponent<PlugHead>();
                if (ph != null)
                {
                    isCarried = false;
                    targetCollider.gameObject.layer = LayerMask.NameToLayer("ActivatedInteractionLayer");
                    co.target = targetCollider.transform;
                    ph.SetConnected();
                    targetCollider.gameObject.GetComponent<LinkedPlug>().Power();
                }
            }
            else if (targetCollider.tag == "Switch" && isCarried && co != null)
            {
                PlugHead ph = co.GetComponent<PlugHead>();
                if (ph != null)
                {
                    isCarried = false;
                    //targetCollider.gameObject.layer = LayerMask.NameToLayer("ActivatedInteractionLayer");
                    co.target = targetCollider.transform;
                    ph.SetConnected();
                    //Debug.Log("Switch Case 1");
                    targetCollider.gameObject.GetComponent<Switch>().Power();
                }
            }
            else if (targetCollider.tag == "Switch")
            {
                //Debug.Log("Switch Case 2");
                if (targetCollider.gameObject.GetComponent<Switch>().powered)
                {
                    targetCollider.gameObject.GetComponent<Switch>().TurnSwitch();
                }
            } else {
                var interactable = targetCollider.GetComponent<IInteractable>();
                if (interactable == null){
                    if (_carriable != null){ //带着东西呢
                        DropDownCurrentCarriable();
                    }

                    return;
                }

                interactable.Interact(this);
            }
        }
    }

    public void PickUp(ICarriable carriable){
        _carriable = carriable;
        carriable.OnPickUp(this);
    }

    public void DropDownCurrentCarriable(){
        _carriable.OnDropDown();
        _carriable = null;
    }

    private void winCheck()
    {
        if (PowerCurrent >= PowerRequired)
        {
            EventCenter.Broadcast(EventDefine.Win);
        }
    }

    public int GetPower()
    {
        return PowerCurrent;
    }
}
