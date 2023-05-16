using System.Collections.Generic;

public interface IDevice{
    public bool HasElectric{ get; set; }
    public List<IDevice> ConnectedDevices{ get; }

    
    public static void ConductElectricity(IDevice startDevice){
        HashSet<IDevice> seenDevices = new();
        void Inner(IDevice device, ISet<IDevice> seen){
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
    
}