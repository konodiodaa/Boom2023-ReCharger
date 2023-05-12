using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private GameObject WinPanel;

    private void Awake()
    {
        EventCenter.AddListener(EventDefine.Win, OpenWinPanel);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.Win, OpenWinPanel);
    }

    void OpenWinPanel()
    {
        WinPanel.SetActive(true);
    }
}
