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
