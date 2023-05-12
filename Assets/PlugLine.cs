using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlugLine : MonoBehaviour
{
    [HideInInspector]
    public LineRenderer lr;

    private Transform plug;

    [SerializeField]
    private Vector3 offset;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        plug = transform.Find("plug");
    }

    private void Update()
    {
        lr.SetPosition(0,transform.position + offset);
        lr.SetPosition(1, plug.transform.position);
    }
}
