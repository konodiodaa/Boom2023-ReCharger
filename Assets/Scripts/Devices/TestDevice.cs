using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devices{
    public class TestDevice: MonoBehaviour, IDevice{
        public Socket socket;
        public SpriteRenderer renderer;
        public TestDevice connected;

        private void Awake(){
            socket.SetDevice(this);
            if (connected != null){
                IDevice.Connect(this, connected);
            }
        }

        private void Update(){
            if (connected == null) return;
            Debug.DrawLine(transform.position, connected.transform.position);
        }

        private bool _hasElectricity = false;

        public bool HasElectric{
            set{
                _hasElectricity = value;
                if(_hasElectricity) renderer.color = Color.green;
                else renderer.color = Color.black;
            }
            get => _hasElectricity;
        }
        public List<IDevice> ConnectedDevices{ get; set; } = new();
    }
}