using Devices;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using Devices;
using Player;
using UnityEngine.Serialization;
using Utility;
using Socket = Devices.Socket;


public class Charger : MonoBehaviour, IDevice, IInteractable
{
    [FormerlySerializedAs("CurrentPower")] [SerializeField]
    private int initPower;

    private int _curPower = 0;
    public int CurPower{
        set{
            _curPower = value;
            UpdatePowerBars();
        }
        get => _curPower;
    }

    public Transform barCtn;
    public Socket socket;

    public SpriteRenderer onSupplySign;

    public Outliner outliner;

    private void Awake(){
        CurPower = initPower;
        socket.SetDevice(this);
    }

    public void UpdatePowerBars(){
        for (var j = 0; j < barCtn.childCount; j++){
            barCtn.GetChild(j).gameObject.SetActive(j < CurPower);
        }
    }

    private bool _hasPower = false;
    public bool HasElectric{
        get => _hasPower;
        set{
            _hasPower = value;
            onSupplySign.color = value ? Color.green : Color.clear;
        } 
    }
    
    public List<IDevice> ConnectedDevices{ get; } = new();
    public string GetInstruction(PowerVolume volume){
        if (volume.IsCarrying()) return null;
        if (HasElectric || CurPower > 0) return "Press E to Charge self!";
        return null;
    }

    public void Interact(PowerVolume volume){
        if (volume.IsCarrying()) return;
        if (!HasElectric){
            var charged = volume.Charge(CurPower);
            CurPower -= charged;
        } else{
            volume.Charge(volume.powerUpLimit);
        }
    }

    public bool IsActive(PowerVolume volume) => !volume.IsCarrying() && ( HasElectric || CurPower > 0);
    public void OnFocused(PowerVolume volume){
        outliner.enabled = true;
    }

    public void OnLoseFocus(PowerVolume volume){
        outliner.enabled = false;
    }
}
    