using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chargers;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PowerVolume : MonoBehaviour
{

    [SerializeField]
    [Header("Power")]
    private int PowerRequired;
    
    public int initPower;

    public int powerUpLimit = 3;

    private int _power;
    public int PowerCurrent{
        set{
            _power = value;
            if (value >= PowerRequired){
                EventCenter.Broadcast(EventDefine.Win);
            }
            UpdatePower();
        }
        get => _power;
    }

    public Transform powerContainer;

    public Text Power_text;
    [FormerlySerializedAs("hint_text")] public Text hintText;

    private ICarriable _carriable;
    
    [NonSerialized]
    public PlayerMovement Movement;

    [NonSerialized]
    public FixedJoint2D Joint;
    
    private void Awake(){
        PowerCurrent = initPower;
        // interactionAva = false;
        Movement = GetComponent<PlayerMovement>();
        Joint = GetComponent<FixedJoint2D>();
    }

    private void Update()
    {
        Power_text.text = PowerCurrent.ToString();
        // winCheck();
        
        // Move To Fixed Update?
        CollisionCheck(); 
        UpdateInstruction();
        if(Input.GetButtonUp("Interact")) DoInteraction();
        if (PowerCurrent < 1)
        {
            Debug.Log("Uncharge");
            GetComponent<Animator>().SetTrigger("Uncharge");
        } else
        {
            GetComponent<Animator>().ResetTrigger("Uncharge");
        }
    }

    public void PowerChange(int value)
    {
        PowerCurrent += value;
    }

    private readonly List<Collider2D> _collider2Ds = new();
    private IInteractable _targetInteractable = null;
    
    private void CollisionCheck()
    {
        foreach (var c in _collider2Ds){
            if (c.GetComponent<IInteractable>() is not{ } interactable) continue;
            interactable.OnLoseFocus(this);
        }
        var num = Physics2D.OverlapCircle(transform.position, 1f, new ContactFilter2D(){
            useTriggers = true,
            useLayerMask = true,
            layerMask = LayerMask.GetMask("InteractionLayer")
        }, _collider2Ds);
        if (num == 0){
            _targetInteractable = null;
            return;
        }
        _collider2Ds.Sort((c1, c2) => {
            var p1 = c1.transform.position;
            var p2 = c2.transform.position;
            var p0 = transform.position;
            return Math.Sign((p0 - p1).sqrMagnitude - (p0 - p2).sqrMagnitude);
        });
        var i = 0;
        for (; i<num; i++){
            var t = _collider2Ds[i];
            if (t.GetComponent<IInteractable>() is not { } interactable) continue;
            if(!interactable.IsActive(this)) continue;
            _targetInteractable = interactable;
            break;
        }

        if (i == num){
            _targetInteractable = null;
            return;
        }
        _targetInteractable.OnFocused(this);
    }

    private void UpdateInstruction()
    {

        if (_targetInteractable is null){
            if (_carriable is not null){
                hintText.gameObject.SetActive(true);
                hintText.text = "Press E to Drop";
                return;
            }
            hintText.gameObject.SetActive(false);
            return;
        }
        if (_targetInteractable.GetInstruction(this) is { } instStr){
            hintText.gameObject.SetActive(true);
            hintText.text = instStr;
            return;
        }
        if (_carriable is not null){
            hintText.gameObject.SetActive(true);
            hintText.text = "Press E to Drop";
            return;
        }
        hintText.gameObject.SetActive(false);
    }

    private void DoInteraction(){
        if (_targetInteractable is null){
            if (_carriable is null) return;
            DropDownCurrentCarriable();
            return;
        }
        _targetInteractable.Interact(this);
    }

    public void PickUp(ICarriable carriable){
        Debug.Log("Picking Up");
        GetComponent<Animator>().SetTrigger("Hold");
        _carriable = carriable;
        carriable.OnPickUp(this);
        var body = carriable.Body;
        Joint.enabled = true;
        Joint.connectedBody = body;
    }

    public void DropDownCurrentCarriable(){
        Debug.Log("Dropping Down");
        GetComponent<Animator>().SetTrigger("Unhold");
        _carriable.OnDropDown();
        _carriable = null;
        Joint.enabled = false;
    }

    public int GetCurrentPower()
    {
        return PowerCurrent;
    }

    public ICarriable GetCarried() => _carriable;
    
    
    /// <summary>
    /// Charge the main character for powerNum power with the uplimit
    /// </summary>
    /// <param name="powerNum"></param>
    /// <returns>Actual powerNum that has been charged</returns>
    public int Charge(int powerNum){
        Debug.Log("Charge");
        foreach (var param in GetComponent<Animator>().parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                GetComponent<Animator>().ResetTrigger(param.name);
            }
        }
        GetComponent<Animator>().SetTrigger("Charge");
        var cur = PowerCurrent;
        cur = Math.Clamp(cur + powerNum, 0, powerUpLimit);
        var ret = cur - PowerCurrent;
        PowerCurrent = cur;
        return ret;
    }

    public bool IsCarrying() => _carriable != null;

    public void UpdatePower(){
        for (int i = 0; i < PowerCurrent; i++){
            if (i >= powerContainer.childCount) return;
            var child = powerContainer.GetChild(i).GetComponent<SpriteRenderer>();
            child.enabled = true;
        }

        for (int i = PowerCurrent; i < powerContainer.childCount; i++){
            powerContainer.GetChild(i).GetComponent<SpriteRenderer>().enabled = false;
        }
    }
    
}
