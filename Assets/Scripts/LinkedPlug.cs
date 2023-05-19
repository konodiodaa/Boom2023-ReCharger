using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LinkedPlug : MonoBehaviour
{
    public static Dictionary<int, bool> LinkPlugStates;

    public int LinkID;
    public bool powered;
    public bool on;
    public Color offColor;
    public Color onColor;
    public GameObject onIndicatorGO;

    // Start is called before the first frame update
    void Start()
    {
        if (LinkedPlug.LinkPlugStates == null)
        {
            LinkedPlug.LinkPlugStates = new Dictionary<int, bool>();
        }
        powered = false;
        on = false;
        if (!LinkedPlug.LinkPlugStates.ContainsKey(LinkID))
        {
            LinkedPlug.LinkPlugStates.Add(LinkID, false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Correlate power states
        if (LinkedPlug.LinkPlugStates[LinkID] != powered)
        {
            powered = LinkedPlug.LinkPlugStates[LinkID];
            // Update Visual Accordingly
        }

        if (powered)
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
            LinkedPlug.LinkPlugStates[LinkID] = powered;
        }
    }

    public void Switch()
    {
        if (powered)
        {
            on = !on;
            Debug.Log("Switched!");
        } else
        {
            Debug.Log("No Power!");
        }
    }
}
