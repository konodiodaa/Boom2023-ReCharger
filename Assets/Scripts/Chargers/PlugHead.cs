using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlugHead : MonoBehaviour
{
    private bool isConnected;

    private Collider2D coll;

    private void Awake()
    {
        isConnected = false;
        coll = GetComponent<Collider2D>();
    }


    public void SetConnected()
    {
        isConnected = true;
        coll.enabled = false;
    }

    public bool getConnectStatus()
    {
        return isConnected;
    }
}
