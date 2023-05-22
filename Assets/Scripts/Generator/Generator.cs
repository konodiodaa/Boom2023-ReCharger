using Devices;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class Generator : MonoBehaviour, IPowerSupply
{
    public Socket socket;
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private bool _hasElectricity;
    private void Awake()
    {
        //GetComponent<SpriteRenderer>().color = Color.white;
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        socket = GetComponentInChildren<Socket>();
        if (socket)
            socket.SetDevice(this);
        else
            Debug.LogWarning("Socket cannot be found. ");
    }

    public bool HasElectric { 
        get {
            return _hasElectricity;
        } 
        set{ 
            _hasElectricity = value;
            if (_hasElectricity && _spriteRenderer)
                _spriteRenderer.color = Color.green;
            else if (_spriteRenderer)
                _spriteRenderer.color = Color.white;
        }
    }
    public List<IDevice> ConnectedDevices { get; set; } = new();
}
