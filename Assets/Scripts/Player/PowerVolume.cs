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
    
    public int initPower;

    private int _power;
    public int PowerCurrent{
        set{
            _power = value;
            if (value >= PowerRequired){
                EventCenter.Broadcast(EventDefine.Win);
            }
        }
        get => _power;
    }

    private Text Power_text;
    private Text hint_text;

    private bool interactionAva;

    private bool isCarried;
    private CarriableObject co;

    [SerializeField]
    private Collider2D targetCollider;

    private ICarriable _carriable;

    public PlayerMovement movement;

    public FixedJoint2D joint;
    
    private void Awake(){
        PowerCurrent = initPower;
        interactionAva = false;
        movement = GetComponent<PlayerMovement>();
        joint = GetComponent<FixedJoint2D>();
        Power_text = transform.Find("Canvas").transform.Find("PowerText").GetComponent<Text>();
        hint_text = transform.Find("Canvas").transform.Find("ChargeHint").GetComponent<Text>();
    }

    private void Update()
    {
        Power_text.text = PowerCurrent.ToString();
        // winCheck();
        
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

    public void PowerChange(int value)
    {
        PowerCurrent += value;
    }

    private readonly List<Collider2D> _collider2Ds = new();    
    
    private void CollisionCheck()
    {
        // Collider2D collider = Physics2D.OverlapCircle(transform.position, 1.0f,LayerMask.GetMask("InteractionLayer"));
        // if (collider != null)
        //     targetCollider = collider;
        // else
        //     targetCollider = null;
        
        int num = Physics2D.OverlapCircle(transform.position, 1f, new ContactFilter2D(){
            useLayerMask = true,
            layerMask = LayerMask.GetMask("InteractionLayer")
        }, _collider2Ds);
        if (num == 0){
            targetCollider = null;
            return;
        }
        _collider2Ds.Sort((c1, c2) => {
            var p1 = c1.transform.position;
            var p2 = c2.transform.position;
            var p0 = transform.position;
            return Math.Sign((p0 - p1).sqrMagnitude - (p0 - p2).sqrMagnitude);
        });
        targetCollider = _collider2Ds[0];
    }

    private void CollisionHandler()
    {

        if (_carriable != null){
            
            hint_text.gameObject.SetActive(true);
            if (targetCollider == null){
                hint_text.text = "Press E to Drop";
                return;
            }
            var interactable = targetCollider.GetComponent<IInteractable>();
            interactionAva = interactable != null;
            if (interactable?.GetInstruction(this) is { } str ){
                hint_text.text = str;
                return;
            }
            hint_text.text = "Press E to Drop";
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
            hint_text.text = interactable.GetInstruction(this);
        }
    }

    private void CollisionInteraction()
    {
        if (!interactionAva) return;
        if (_carriable != null && Input.GetButtonUp("Interact")){
            if (targetCollider == null){
                DropDownCurrentCarriable();
                return;
            }
            var interactable = targetCollider.GetComponent<IInteractable>();
            if (interactable is{ } inter){
                inter.Interact(this);
                return;
            }
            DropDownCurrentCarriable();
            return;
        }

        if (Input.GetButtonUp("Interact") && targetCollider != null){
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
                interactable?.Interact(this);
            }
        }
    }

    public void PickUp(ICarriable carriable){
        _carriable = carriable;
        carriable.OnPickUp(this);
        var body = carriable.Body;
        joint.enabled = true;
        joint.connectedBody = body;
    }

    public void DropDownCurrentCarriable(){
        _carriable.OnDropDown();
        _carriable = null;
        joint.enabled = false;
    }

    public int GetCurrentPower()
    {
        return PowerCurrent;
    }

    public ICarriable GetCarried() => _carriable;

    public bool IsCarrying() => _carriable != null;
    
}
