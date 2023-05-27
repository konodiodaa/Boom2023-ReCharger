using System.Collections;
using System.Collections.Generic;
using Levels;
using UnityEngine;

public class LevelManager : MonoBehaviour{
    public int levelMax = 10;
    public LevelLoader loader;
    public LevelInfo info;

    private void Start(){
        EventCenter.AddListener(EventDefine.Restart, Restart);
    }

    private void OnDestroy(){
        EventCenter.RemoveListener(EventDefine.Restart, Restart);
    }
    
    public void Restart(){
        loader.LoadLevelWithTween(info.id);
    }

    public void GoNext(){
        if (info.id == 10) return;
        loader.LoadLevelWithTween(info.id + 1);
    }

    public bool ReachLastLevel(){
        return info.id == 10;
    }
}
