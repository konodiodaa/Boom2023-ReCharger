using System.Collections;
using System.Collections.Generic;
using Levels;
using UnityEngine;

public class LevelSelection : MonoBehaviour
{

    [Header("Level count")]
    [SerializeField]
    private int count;

    private Transform grid;

    [SerializeField]
    private GameObject levelBTNPrefab;

    public LevelLoader loader;

    private void Awake()
    {
        grid = transform.Find("Levels");
        if(grid != null)
        InitLevelBTNs();
    }

    private void InitLevelBTNs()
    {
        for(int i = 0;i<LevelManager.LevelMax;i++)
        {
            GameObject go = Instantiate(levelBTNPrefab,grid.transform);
            go.GetComponent<LevelBTN>().levelId = i + 1;
            go.GetComponent<LevelBTN>().loader = loader;
        }
    }
}
