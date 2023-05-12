using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerVolume : MonoBehaviour
{

    [SerializeField]
    [Header("Power")]
    private int PowerRequired;
    public int PowerCurrent;

    private Text Power_text;

    private bool isCarried;
    private PlugHead PH;
    private void Awake()
    {
        Power_text = transform.Find("Canvas").transform.GetComponentInChildren<Text>();
    }

    private void Update()
    {
        Power_text.text = PowerCurrent.ToString();
        winCheck();
    }

    public void PowerChange(int value)
    {
        PowerCurrent += value;


    }

    private void winCheck()
    {
        if (PowerCurrent >= PowerRequired)
        {
            EventCenter.Broadcast(EventDefine.Win);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Charger")
        {
            PowerCurrent += collision.transform.GetComponent<Charger>().DisCharge();
            PowerCurrent = Mathf.Clamp(PowerCurrent,0,PowerRequired); 
        }
        else if(collision.tag == "Mech")
        {
            int required = collision.transform.GetComponent<Mech>().GetRequired();
            if (required <= PowerCurrent && !collision.transform.GetComponent<Mech>().getActivate())
            {
                PowerCurrent -= required;
                collision.transform.GetComponent<Mech>().ReCharge();
            }
        }
        else if(collision.tag == "PlugHead" && !isCarried)
        {
            PH = collision.transform.GetComponent<PlugHead>();
            PH.player = transform;
            isCarried = true;
        }
        else if(collision.tag == "Socket" && isCarried && PH != null)
        {
            isCarried = false;
            PH.player = collision.transform;
            PH.SetConnected();
        }
    }

    public int GetPower()
    {
        return PowerCurrent;
    }
}
