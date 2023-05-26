using System;
using System.Collections.Generic;
using Devices;
using Player;
using Unity.Mathematics;
using UnityEngine;
using Utility;

namespace Switches{
    public class Switch : MonoBehaviour, IDevice, IChargeable, IInteractable{

        public enum Type{
            WithSocket,
            FixedAmount
        }

        public Type type;
        public int powerNeeded = -1;
        public Socket socket;
        // public Color noPowerColor = Color.gray;
        // public Color poweredColor = Color.green;
        // public Color onColor = Color.yellow;
        // public Color offColor = Color.blue;
        // public SpriteRenderer bg;
        // public SpriteRenderer onInd;
        public Transform circleCtn;
        
        [Header("Sprites")]
        public Sprite powerOnSpr;
        public Sprite powerOffSpr;
        public Sprite switchOnSpr;
        public Sprite switchOffSpr;

        public SpriteRenderer switchLight;
        
        [Header("Handle")]
        // public Transform handle;
        // public float onDeg = 310;
        // public float offDeg = 50;
        
        public GameObject controled;

        [Header("Outliner")] 
        public Outliner outliner;

        private ISwitchControled _controled = null;
        private Animator _animator;
    

        private int _curPower = 0;
        private bool _isPowered = false;
    
        public bool HasElectric{
            get => _isPowered;
            set{
                _isPowered = value;
                // bg.color = value ? poweredColor : noPowerColor;
                UpdateSprites();
                CallControledMethod();
            }
        }

        private bool _isOn = false;
        public bool IsOn{
            set{
                _isOn = value;
                UpdateSprites();
                // onInd.color = value ? onColor : offColor;
                CallControledMethod();
            }
            get => _isOn;
        }

        private void Awake(){
            _controled = controled != null ? controled.GetComponent<ISwitchControled>() : null;
            _animator = GetComponent<Animator>();
            if (_controled is null){
                Debug.Log($"{controled} does not have a component of ISwitchControled!");
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            _isPowered = false;
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
            // circleCtn.GetChild(_curPower-1).GetComponent<SpriteRenderer>().color = Color.green;
            UpdateSprites();
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

        public void OnFocused(PowerVolume volume){
            outliner.enabled = true;
        }

        public void OnLoseFocus(PowerVolume volume){
            outliner.enabled = false;
        }

        private void SetCirclesColor(Color c){
            for (var i = 0; i < circleCtn.childCount; i++){
                var ch = circleCtn.GetChild(i).GetComponent<SpriteRenderer>();
                ch.color = c;
            }
        }

        private void SetPowerCircleSprite(Sprite spr){
            for (var i = 0; i < circleCtn.childCount; i++){
                var ch = circleCtn.GetChild(i).GetComponent<SpriteRenderer>();
                ch.sprite = spr;
            }
        }

        private void UpdateSprites(){
            if (IsOn){
                _animator.SetTrigger("TurnOn");
            } else{
                _animator.SetTrigger("TurnOff");
            }

            if (HasElectric && IsOn){
                switchLight.sprite = switchOnSpr;
            } else{
                switchLight.sprite = switchOffSpr;
            }

            if (type == Type.WithSocket) return;
            for (int i = 0; i < circleCtn.childCount; i++){
                var ch = circleCtn.GetChild(i);
                if (i < _curPower){
                    ch.GetComponent<SpriteRenderer>().sprite = powerOnSpr;
                } else{
                    ch.GetComponent<SpriteRenderer>().sprite = powerOffSpr;
                }
            }
        }

        private void CallControledMethod(){
            if (IsOn && HasElectric){
                _controled?.OnTurnedOn();
            } else{
                _controled?.OnTurnedOff();                
            }
        }
    }
}
