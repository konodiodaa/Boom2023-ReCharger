using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class LevelBTN : MonoBehaviour
{
    private TextMeshProUGUI text;

    [HideInInspector]
    public int levelId = -1;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(ButtonBaseClicked);
        text = transform.Find("Text").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (levelId != -1)
        {
            text.text = levelId.ToString();
        }
    }

    private void ButtonBaseClicked()
    {
        if(levelId != -1)
        {
            Debug.Log("level: " + levelId);
            // TODO: change to level id
            // SceneManager.LoadScene(levelId);
        }
    }
}
