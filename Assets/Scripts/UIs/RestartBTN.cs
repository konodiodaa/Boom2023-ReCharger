using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIs{
    public class RestartBTN: MonoBehaviour{
        private void Awake(){
            GetComponent<Button>().onClick.AddListener(ButtonBaseClicked);
        }

        private void ButtonBaseClicked(){
            EventCenter.Broadcast(EventDefine.Restart);
        }
    }
}