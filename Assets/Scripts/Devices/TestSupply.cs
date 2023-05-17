using System.Collections.Generic;
using UnityEngine;

namespace Devices{
    public class TestSupply: MonoBehaviour, IPowerSupply{
        public bool HasElectric{ get; set; } = true;
        public List<IDevice> ConnectedDevices{ get; set; } = new();
    }
}