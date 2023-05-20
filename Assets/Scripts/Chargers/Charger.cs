using Devices;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using Devices;
using Player;
using UnityEngine.Serialization;
using Socket = Devices.Socket;


public class Charger : MonoBehaviour, IDevice, IInteractable
{
    [FormerlySerializedAs("CurrentPower")] [SerializeField]
    private int initPower;

    private int _curPower = 0;
    public int CurPower{
        set{
            _curPower = value;
            Power_text.text = value.ToString();
            UpdatePowerBars();
        }
        get => _curPower;
    }

    private Text Power_text;

    public Transform barCtn;
    public Socket socket;

    private void Awake(){
        Power_text = transform.Find("Canvas").transform.GetComponentInChildren<Text>();
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

    public bool IsActive(PowerVolume volume) => HasElectric || CurPower > 0;
}
    