using System;
using System.Collections.Generic;
using Devices;
using Player;
using Unity.Mathematics;
using UnityEngine;

namespace Switches{
    public class Switch : MonoBehaviour, IDevice, IChargeable, IInteractable{

        public enum Type{
            WithSocket,
            FixedAmount
        }

        public Type type;
        public int powerNeeded = -1;
        public Socket socket;
        public Color noPowerColor = Color.gray;
        public Color poweredColor = Color.green;
        public Color onColor = Color.yellow;
        public Color offColor = Color.blue;
        public SpriteRenderer bg;
        public SpriteRenderer onInd;
        public Transform circleCtn;
        [Header("Handle")]
        public Transform handle;
        public float onDeg = 310;
        public float offDeg = 50;
        
        public GameObject controled;

        private ISwitchControled _controled = null;
    

        private int _curPower = 0;
        private bool _isPowered = false;
    
        public bool HasElectric{
            get => _isPowered;
            set{
                _isPowered = value;
                bg.color = value ? poweredColor : noPowerColor;
            }
        }

        private bool _isOn = false;
        public bool IsOn{
            set{
                _isOn = value;
                onInd.color = value ? onColor : offColor;
                if(value) _controled?.OnTurnedOn();
                else _controled?.OnTurnedOff();
                
            }
            get => _isOn;
        }

        private void Awake(){
            _controled = controled != null ? controled.GetComponent<ISwitchControled>() : null;
            if (_controled is null){
                Debug.Log($"{controled} does not have a component of ISwitchControled!");
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            HasElectric = false;
            switch (type){
                case Type.WithSocket:
                    socket.SetDevice(this);
                    circleCtn.gameObject.SetActive(false);
                    break;
                case Type.FixedAmount:
                    socket.gameObject.SetActive(false);
                    for (var i = 0; i < circleCtn.childCount; i++){
                        var ch = circleCtn.GetChild(i).GetComponent<SpriteRenderer>();
                        ch.gameObject.SetActive(i < powerNeeded);
                    }
                    SetCirclesColor(Color.black);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    
        public List<IDevice> ConnectedDevices{ get; } = new();
        public void GetCharge(){
            if (type == Type.WithSocket) return;
            _curPower++;
            if (_curPower == powerNeeded){
                HasElectric = true;
            }
            circleCtn.GetChild(_curPower-1).GetComponent<SpriteRenderer>().color = Color.green;
        }

        public string GetInstruction(PowerVolume volume){
            return "Press E to toggle the switch!";
        }

        private float _timer = 0f;

        public void Interact(PowerVolume volume){
            IsOn = !IsOn;
            _timer = 0f;
        }

        public bool IsActive(PowerVolume volume){
            return !volume.IsCarrying() && HasElectric;
        }

        private void SetCirclesColor(Color c){
            for (var i = 0; i < circleCtn.childCount; i++){
                var ch = circleCtn.GetChild(i).GetComponent<SpriteRenderer>();
                ch.color = c;
            }
        }

        public float switchTime = 0.2f;

        private void Update(){
            if (IsOn){
                handle.rotation =
                    Quaternion.Lerp(Quaternion.Euler(0, 0, offDeg), Quaternion.Euler(0, 0, onDeg),
                        _timer / switchTime);
            } else{
                handle.rotation =
                    Quaternion.Lerp(Quaternion.Euler(0, 0, onDeg), Quaternion.Euler(0, 0, offDeg),
                        _timer / switchTime);
            }

            _timer += Time.deltaTime;
        }
    }
}
