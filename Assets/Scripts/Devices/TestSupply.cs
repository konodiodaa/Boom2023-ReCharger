using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devices{
    public class TestSupply: MonoBehaviour, IPowerSupply{
        public Socket socket;

        private void Awake(){
            GetComponent<SpriteRenderer>().color = Color.green;
            socket.SetDevice(this);
        }

        public bool HasElectric{ get; set; } = true;
        public List<IDevice> ConnectedDevices{ get; set; } = new();
    }
}