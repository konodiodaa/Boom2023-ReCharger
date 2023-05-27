using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public int powerUpLimit = 3;

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

    private ICarriable _carriable;

    public PlayerMovement movement;

    public FixedJoint2D joint;
    
    private void Awake(){
        PowerCurrent = initPower;
        // interactionAva = false;
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
        CollisionCheck(); 
        UpdateInstruction();
        if(Input.GetButtonUp("Interact")) DoInteraction();

    }

    public void PowerChange(int value)
    {
        PowerCurrent += value;
    }

    private readonly List<Collider2D> _collider2Ds = new();
    private IInteractable _targetInteractable = null;
    
    private void CollisionCheck()
    {
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
        }
    }

    private void UpdateInstruction()
    {

        if (_targetInteractable is null){
            if (_carriable is not null){
                hint_text.gameObject.SetActive(true);
                hint_text.text = "Press E to Drop";
                return;
            }
            hint_text.gameObject.SetActive(false);
            return;
        }
        if (_targetInteractable.GetInstruction(this) is { } instStr){
            hint_text.gameObject.SetActive(true);
            hint_text.text = instStr;
            return;
        }
        if (_carriable is not null){
            hint_text.gameObject.SetActive(true);
            hint_text.text = "Press E to Drop";
            return;
        }
        hint_text.gameObject.SetActive(false);
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
    
    
    /// <summary>
    /// Charge the main character for powerNum power with the uplimit
    /// </summary>
    /// <param name="powerNum"></param>
    /// <returns>Actual powerNum that has been charged</returns>
    public int Charge(int powerNum){
        var cur = PowerCurrent;
        cur = Math.Clamp(cur + powerNum, 0, powerUpLimit);
        var ret = cur - PowerCurrent;
        PowerCurrent = cur;
        return ret;
    }

    public bool IsCarrying() => _carriable != null;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            EventCenter.Broadcast(EventDefine.Lose);
        }
    }

}
