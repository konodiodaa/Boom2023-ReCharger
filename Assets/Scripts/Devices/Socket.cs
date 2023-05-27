using System;
using Player;
using UnityEngine;
using Utility;
using Wires;

namespace Devices{
    public class Socket: MonoBehaviour, IInteractable{

        public Sprite sprPlugged;
        
        private Plug2 _plug = null;
        private IDevice _connectedDevice = null;
        private SpriteRenderer _renderer;

        private void Awake(){
            _renderer = GetComponent<SpriteRenderer>();
        }


        public void SetDevice(IDevice device){
            _connectedDevice = device;
        }

        public IDevice GetDevice() => _connectedDevice;

        public string GetInstruction(PowerVolume volume){
            if (volume.GetCarried() != null && _plug == null){
                return "Press E to Plug!";
            }

            if (volume.GetCarried() == null && _plug != null){
                return "Press E to Unplug!";
            }
            return null;
        }

        public void Interact(PowerVolume volume){
            if (volume.GetCarried() == null && _plug != null){ // Player is carrying stuff and no plug is in this socket
                volume.PickUp(PullOut());
            }
            else if(volume.GetCarried() is Plug2 plug && _plug == null){ // Player is not 
                volume.DropDownCurrentCarriable();
                PlugIn(plug);
            }
        }

        public bool IsActive(PowerVolume volume) => (volume.GetCarried() == null && _plug != null) ||
                                                    (volume.GetCarried() is Plug2 plug && _plug == null);


        private Sprite _prev;
        public void PlugIn(Plug2 plug){
            _plug = plug;
            _plug.OnPlugIn(this);
            if (_connectedDevice != null){
                IDevice.Connect(_connectedDevice, _plug);
            }

            _prev = _renderer.sprite;
            _renderer.sprite = sprPlugged;
        }

        public Plug2 PullOut(){
            var ret = _plug;
            _plug.OnPullOut(this);
            _plug = null;
            
            if (_connectedDevice != null){
                IDevice.Disconnect(_connectedDevice, ret);
            }

            _renderer.sprite = _prev;
            return ret;
        }
        
        public Outliner outliner;
        public void OnFocused(PowerVolume volume){
            outliner.enabled = true;
        }

        public void OnLoseFocus(PowerVolume volume){
            outliner.enabled = false;
        }
    }
}