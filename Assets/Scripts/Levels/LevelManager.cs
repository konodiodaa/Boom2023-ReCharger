using System.Collections;
using System.Collections.Generic;
using Levels;
using UnityEngine;

public class LevelManager : MonoBehaviour{
    public const int LevelMax = 3;
    public LevelLoader loader;
    public LevelInfo info;

    public void Restart(){
        loader.LoadLevelWithTween(info.id);
    }

    public void GoNext(){
        if (info.id == LevelMax) return;
        loader.LoadLevelWithTween(info.id + 1);
    }

    public bool ReachLastLevel(){
        return info.id == LevelMax;
    }
}
