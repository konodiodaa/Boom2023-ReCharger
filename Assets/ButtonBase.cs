using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBase : MonoBehaviour
{
    [SerializeField]
    [Header("Button event")]
    private EventDefine buttonEvent;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(ButtonBaseClicked);
    }

    private void ButtonBaseClicked()
    {
        EventCenter.Broadcast(buttonEvent);
    }

}
