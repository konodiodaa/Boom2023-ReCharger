using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotor : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed;

    private Transform _rotorSprite;
    private bool _isRotating;
    private bool _clockwise;
    private Generator _generator;

    public List<PropellerBlade> Blades = new List<PropellerBlade>();


    public Transform RotorSprite
    {
        get { return _rotorSprite; }
    }
    private void Awake()
    {
        _rotorSprite = GetComponentsInChildren<Transform>()[1];
      
        _generator = GetComponentInParent<Generator>();
        _clockwise = false;
        _isRotating = false;
    }

    private void Update()
    {
        _isRotating = false;
        foreach(PropellerBlade blade in Blades)
        {
            if (blade.UnderHydroForce)
            {
                _isRotating = true;
                if (blade.OnRightSide)
                {
                    _clockwise = true;
                }
                else
                {
                    _clockwise = false;
                }
            }
        }


        if (_isRotating && _clockwise)
        {
            // _generator.HasElectric = true;
            _generator.TurnOn();
            _rotorSprite.Rotate(0, 0, -_rotationSpeed * Time.deltaTime);
        }
        else if (_isRotating)
        {
            // _generator.HasElectric = true;
            _generator.TurnOn();
            _rotorSprite.Rotate(0, 0, _rotationSpeed * Time.deltaTime);
        }
        else
        {
            // _generator.HasElectric = false;
            _generator.TurnOff();
        }
    }
}
