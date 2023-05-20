using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mech : MonoBehaviour
{

    [Header("Mech")]
    [SerializeField]
    protected bool isActivate;
    [SerializeField]
    protected int powerRequired;


    public void ReCharge()
    {
        gameObject.layer = LayerMask.NameToLayer("ActivatedInteractionLayer"); // set object to layer ActivatedInteractionLayer if it is activated
        isActivate = true;
    }

    public int GetRequired()
    {
        return powerRequired;
    }

    public bool getActivate()
    {
        
        return isActivate;
    }
}
