using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelection : MonoBehaviour
{

    [Header("Level count")]
    [SerializeField]
    private int count;

    private Transform grid;

    [SerializeField]
    private GameObject levelBTNPrefab;

    private void Awake()
    {
        grid = transform.Find("Levels");
        if(grid != null)
        InitLevelBTNs();
    }

    private void InitLevelBTNs()
    {
        for(int i = 0;i<count;i++)
        {
            GameObject go = Instantiate(levelBTNPrefab,grid.transform);
            go.GetComponent<LevelBTN>().levelId = i + 1;
        }
    }
}
