using System.Collections.Generic;
using UnityEngine;

namespace Devices{
    public class TestDevice: MonoBehaviour, IDevice{
        public bool HasElectric{ get; set; } = false;
        public List<IDevice> ConnectedDevices{ get; set; } = new();
    }
}