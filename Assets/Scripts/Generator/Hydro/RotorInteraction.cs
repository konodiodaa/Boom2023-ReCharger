using Devices;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wires;

public class RotorInteraction : MonoBehaviour, IInteractable
{
    //private PropellerBlade _blade = null;
    private Rotor _rotor;

    private void Awake()
    {
        _rotor = GetComponentInParent<Rotor>();
    }


    public string GetInstruction(PowerVolume volume)
    {
        if (volume.GetCarried() != null)
        {
            return "Press E to Install Propeller!";
        }

        if (volume.GetCarried() == null )
        {
            return "Press E to Unplug!";
        }
        return null;
    }

    public void Interact(PowerVolume volume)
    {
        if (volume.GetCarried() == null)
        { // Player is carrying stuff and no plug is in this socket
            volume.PickUp(PullOut());
        }
        else if (volume.GetCarried() is PropellerBlade blade && _rotor.Blades.Count <= 4)
        { // Player is not 
            volume.DropDownCurrentCarriable();
            PlugIn(blade);
        }
    }

    public bool IsActive(PowerVolume volume) => (volume.GetCarried() == null && _rotor.Blades.Count >= 4) ||
                                                (volume.GetCarried() is PropellerBlade blade && _rotor.Blades.Count <= 4);

    public void PlugIn(PropellerBlade blade)
    {
        //_plug = plug;
        //_plug.OnPlugIn(this);
        
        blade.AddToRotor(_rotor);
    }

    public Plug2 PullOut()
    {
        //var ret = _plug;
        //_plug.OnPullOut(this);
        //_plug = null;

        Debug.Log("No implementation for unplug rotor blade");
        return null;
        //return ret;
    }
}
