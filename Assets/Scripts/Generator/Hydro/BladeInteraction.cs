using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wires;

public class BladeInteraction : MonoBehaviour, IInteractable
{
    public PropellerBlade plug;

    public string GetInstruction(PowerVolume volume)
    {
        return plug.state switch
        {
            PropellerBlade.State.Free => volume.GetCarried() == null ? "Press E to Pick Up" : null,
            PropellerBlade.State.Carried => null,
            PropellerBlade.State.Plugged => null
        };
    }

    public void Interact(PowerVolume volume)
    {
        switch (plug.state)
        {
            case PropellerBlade.State.Free:
                if (volume.GetCarried() != null) return;
                Debug.Log("Player pick up Plug!");
                volume.PickUp(plug);
                break;
            case PropellerBlade.State.Carried:
                break;
            case PropellerBlade.State.Plugged:
                break;
        }
    }

    bool IInteractable.IsActive(PowerVolume volume) => !volume.IsCarrying();

    private void OnDisable()
    {
        GetComponent<Collider2D>().enabled = false;
    }

    private void OnEnable()
    {
        GetComponent<Collider2D>().enabled = true;
    }
}
