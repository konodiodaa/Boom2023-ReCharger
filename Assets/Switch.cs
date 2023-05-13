using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Switch : MonoBehaviour
{
    public bool powered;
    public bool on;

    public Color onColor;
    public Color offColor;
    public GameObject onIndicatorGO;
    
    public Color PowerColor;
    public Color noPowerColor;
    public GameObject poweredIndicatorGO;

    // Start is called before the first frame update
    void Start()
    {
        powered = false;
        on = false;
        onIndicatorGO.GetComponent<RawImage>().color = offColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (powered)
        {
            poweredIndicatorGO.GetComponent<RawImage>().color = PowerColor;
        } else
        {
            poweredIndicatorGO.GetComponent<RawImage>().color = noPowerColor;
        }
        if (on)
        {
            onIndicatorGO.GetComponent<RawImage>().color = onColor;
        }
        else
        {
            onIndicatorGO.GetComponent<RawImage>().color = offColor;
        }
    }

    public void Power()
    {
        if (!powered)
        {
            powered = true;
        }
    }

    public void UnPower()
    {
        if (powered)
        {
            powered = false;
        }
    }

    public void TurnSwitch()
    {
        if (powered)
        {
            on = !on;
            Debug.Log("Switched!");
        }
        else
        {
            Debug.Log("No Power!");
        }
    }
}
