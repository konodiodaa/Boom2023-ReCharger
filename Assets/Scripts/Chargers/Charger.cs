using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Charger : MonoBehaviour
{
    [SerializeField]
    private int CurrentPower;
    [SerializeField]
    private int FullPower  = 3;

    private Text Power_text;
    private PlugHead ph;
    private void Awake()
    {
        Power_text = transform.Find("Canvas").transform.GetComponentInChildren<Text>();
        ph = transform.Find("plug").GetComponent<PlugHead>();
    }

    private void Update()
    {
        Power_text.text = CurrentPower.ToString();
        if(ph.getConnectStatus())
        {
            CurrentPower = FullPower;
        }
    }

    public int DisCharge()
    {
        int tmp = Mathf.Clamp(CurrentPower,0, FullPower);
        CurrentPower = 0;
        return tmp;
    }

    public void ReCharge(int value)
    {
        CurrentPower = value;
    }

    public void FullCharge()
    {
        CurrentPower = FullPower;
    }
}
