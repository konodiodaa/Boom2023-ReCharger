using Chargers;
using Devices;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using Wires;

public class PropellerBlade : MonoBehaviour, ICarriable
{
    [SerializeField] private float _length;
    [SerializeField] private float _width;
    [SerializeField] private float _offsetFromRotorCenter;
    
    private Rotor _pluggedSock;
    private Rotor _rotor;
    private int _bladeIndex; 

    public bool UnderHydroForce{ get; private set; }
    public bool OnRightSide { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        transform.localScale = new Vector3(_length, _width, 1);
        body = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        //AddToRotor(_testRotor);
    }

    private void LateUpdate()
    {


        if (_rotor)
        {
            Quaternion rotation = Quaternion.identity;
            rotation = rotation * _rotor.RotorSprite.rotation;
            for (int i = 0; i < _bladeIndex; i++)
            {
                rotation = rotation * Quaternion.Euler(0, 0, 90);
            }

            Vector3 dirVec = Vector3.right;
            dirVec = rotation * dirVec;
            //Debug.Log(dirVec);

            transform.rotation = rotation;
            transform.localPosition = Vector3.zero;
            //Debug.Log(dirVec * (_offsetFromRotorCenter + _length / 2));
            transform.localPosition += dirVec * (_offsetFromRotorCenter + _length / 2);
            //Debug.Log(transform.position);

            //Calcualte the blade on right side or on the left side.
            if(dirVec.x >= 0)
            {
                OnRightSide = true;
            }
            else
            {
                OnRightSide = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("WaterPowerSource"))
        {
            //Debug.Log("Into water");
            UnderHydroForce = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("Collision");
        Debug.Log(collision.gameObject.tag);

        if (collision.gameObject.CompareTag("WaterPowerSource"))
        {
            //Debug.Log("In water");
            UnderHydroForce = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("WaterPowerSource"))
        {
            //Debug.Log("Out water");
            UnderHydroForce = false ;
        }
    }
    public void AddToRotor(Rotor rotor)
    {
        

        if (rotor.Blades.Count >= 4)
        {
            return;
        }
        _rotor = rotor;
        transform.SetParent(_rotor.transform);
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;
        _rotor.Blades.Add(this);
        _bladeIndex = _rotor.Blades.Count - 1;





        //transform.Translate(transform.position + new Vector3(_offsetFromRotor, 0, 0));




        state = State.Plugged;
        body.isKinematic = true;
        _pluggedSock = rotor;
        _prevLayer = gameObject.layer;
        interaction.enabled = false;
        gameObject.layer = LayerMask.NameToLayer("Default");
        gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
    }


    public enum State
    {
        Free,
        Plugged,
        Carried
    }

    public State state = State.Free;

    public Rigidbody2D body;

    private PowerVolume _powerVolume;

    
    public bool IsCarried => state == State.Carried;

    private float _prevMass;

    private int _prevLayer;

    public BladeInteraction interaction;

    public void OnPickUp(PowerVolume powerVolume)
    {
        state = State.Carried;
        _powerVolume = powerVolume;
        _prevMass = body.mass;
        body.SetRotation(0);
        body.freezeRotation = true;
        _prevLayer = gameObject.layer;
        interaction.enabled = false;
        gameObject.layer = LayerMask.NameToLayer("NoCollision"); //LayerMask.GetMask("Carried");

    }

    public void OnDropDown()
    {
        state = State.Free;
        body.mass = _prevMass;
        _powerVolume = null;
        body.freezeRotation = false;
        interaction.enabled = true;
        gameObject.layer = _prevLayer; //LayerMask.NameToLayer("");//LayerMask.GetMask("InteractionLayer");
    }

  
    public float Mass => state == State.Carried ? (_powerVolume.Movement.rb.mass + body.mass) : body.mass;

    public Vector2 Velocity
    {
        set
        {
            if (state is State.Plugged)
            {
                body.velocity = Vector2.zero;
                return;
            }
            // if (IsCarried) _powerVolume.movement.rb.velocity += value - body.velocity;
            body.velocity = value;
        }
        get => IsCarried ? _powerVolume.Movement.rb.velocity : body.velocity;
    }

    public Vector2 Position
    {
        set
        {
            if (state is State.Plugged) return;
            body.MovePosition(value);
        }
        get => body.position;
    }

    public Vector2 NextPosition { get; set; } = Vector2.zero;
    public Vector2 Acceleration => Force * (1 / Mass);
    public Vector2 Force { get; set; } = Vector2.zero;
    public IRopeSegment PrevSeg { set; get; } = null;
    public IRopeSegment NextSeg { set; get; } = null;
    public Vector2 AvoidCollision(Vector2 testPosition)
    {
        return testPosition;
    }

    public bool IsFree()
    {
        return state == State.Free;
    }

    public void AvoidCollision() { }

    private void FixedUpdate()
    {
        
    }

    public Vector2 DistanceDirection => (PrevSeg == null ? (Position - NextSeg.Position) : (PrevSeg.Position - Position)).normalized;

    public Rigidbody2D Body => body;
}
