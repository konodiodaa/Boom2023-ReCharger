using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Devices{
    /// <summary>
    /// 所有需要用到电量的设施都需要实现这个接口！
    /// 用 <code> IDevice.Connect(a, b) </code>来联通两个设施。如果两个设施的供电情况不同则会自动传到电
    /// </summary>
    public interface IDevice{
        public bool HasElectric{ get; set; }
        public List<IDevice> ConnectedDevices{ get; }

    
        public static void ConductElectricity(IDevice startDevice){
            HashSet<IDevice> seenDevices = new();
            void Inner(IDevice device, System.Collections.Generic.ISet<IDevice> seen){
                device.HasElectric = true;
                seen.Add(device);
                foreach (var nextConnectedDevice in device.ConnectedDevices){
                    if(seen.Contains(nextConnectedDevice) || nextConnectedDevice.HasElectric) continue;
                    Inner(nextConnectedDevice, seen);
                }
            }
            Inner(startDevice, seenDevices);
        }

        public static void Connect(IDevice a, IDevice b){
            if(!a.ConnectedDevices.Contains(b)) a.ConnectedDevices.Add(b);
            if(!b.ConnectedDevices.Contains(a)) b.ConnectedDevices.Add(a);
            if (a.HasElectric == b.HasElectric) return;
            ConductElectricity(a.HasElectric ? b : a);
        }

        public static void UpdateElectricityState(IDevice dev){
            bool FindSupply(IDevice d, System.Collections.Generic.ISet<IDevice> seen){
                if (d is IPowerSupply {HasElectric: true}) return true;
                seen.Add(d);
                foreach (var next in d.ConnectedDevices){
                    if (seen.Contains(next)) continue;
                    if (FindSupply(next, seen)) return true;
                }
                return false;
            }

            void SetElecState(IDevice d, bool val, System.Collections.Generic.ISet<IDevice> seen){
                seen.Add(d);
                d.HasElectric = val;
                foreach (var next in d.ConnectedDevices){
                    if (seen.Contains(next)) continue;
                    SetElecState(next, val, seen);
                }
            }

            HashSet<IDevice> seen = new();
            var val = FindSupply(dev, seen);
            seen.Clear();
            SetElecState(dev, val, seen);
        }

        public static void Disconnect(IDevice a, IDevice b){
            a.ConnectedDevices.Remove(b);
            b.ConnectedDevices.Remove(a);
            UpdateElectricityState(a);
            UpdateElectricityState(b);
        }
    }

    public interface IPowerSupply : IDevice{
    }
}