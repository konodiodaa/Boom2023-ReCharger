using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlugHead : MonoBehaviour
{
    [HideInInspector]
    public Transform player;

    private bool isConnected;

    private Collider2D coll;

    private void Awake()
    {
        isConnected = false;
        coll = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if(player != null)
        {
            transform.position = player.position;
        }
    }

    public void SetPlayerNull()
    {
        player = null;
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
