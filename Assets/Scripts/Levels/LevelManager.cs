using System.Collections;
using System.Collections.Generic;
using Levels;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private GameObject WinPanel;
    public LevelLoader loader;
    public LevelInfo info;

    private void Start(){
        EventCenter.AddListener(EventDefine.Win, OpenWinPanel);
        EventCenter.AddListener(EventDefine.Restart, Restart);
    }

    private void OnDestroy(){
        EventCenter.RemoveListener(EventDefine.Win, OpenWinPanel);
        EventCenter.RemoveListener(EventDefine.Restart, Restart);
    }

    void OpenWinPanel(){
        WinPanel.SetActive(true);
    }

    public void Restart(){
        loader.LoadLevelWithTween(info.id);
    }
}
