using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PausePanel : MonoBehaviour
{

    private GameObject PauselPanel;
    private GameObject LosePanel;

    [SerializeField]
    private TextMeshProUGUI title;
    [SerializeField]
    private GameObject ContinueBTN;
    [SerializeField]
    private GameObject NextLevelBTN;

    private void Awake()
    {

        EventCenter.AddListener(EventDefine.Win, WinHandle);
        EventCenter.AddListener(EventDefine.Lose, LoseHandle);
        EventCenter.AddListener(EventDefine.Pause, PauseHandle);
        EventCenter.AddListener(EventDefine.Restart, RestartHandle);
        EventCenter.AddListener(EventDefine.NextLevel, NextLevelHandle);
        EventCenter.AddListener(EventDefine.Back2Title, Back2TitleHandle);
        EventCenter.AddListener(EventDefine.Continue, ContinueHandle);
    }

    private void Start()
    {
        PauselPanel = transform.Find("PausePanel").gameObject;
        LosePanel = transform.Find("LosePanel").gameObject;

        Time.timeScale = 1.0f; 
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.Pause, PauseHandle);
        EventCenter.RemoveListener(EventDefine.Restart, RestartHandle);
        EventCenter.RemoveListener(EventDefine.NextLevel, NextLevelHandle);
        EventCenter.RemoveListener(EventDefine.Back2Title, Back2TitleHandle);
        EventCenter.RemoveListener(EventDefine.Continue, ContinueHandle);
    }

    void WinHandle()
    {
        title.text = "Level Clear";

        if (NextLevelBTN == null || ContinueBTN == null || PauselPanel == null) return;
        NextLevelBTN.SetActive(true);
        ContinueBTN.SetActive(false);
        PauselPanel.SetActive(true);
        Time.timeScale = 0.0f;
    }

    void LoseHandle()
    {
        if(LosePanel!=null)
        {
            LosePanel.SetActive(true);
            Time.timeScale = 0.0f;
        }

    }

    void PauseHandle()
    {
        title.text = "Pause";
        NextLevelBTN.SetActive(false);
        ContinueBTN.SetActive(true);
        PauselPanel.SetActive(true);
        Time.timeScale = 0.0f;
    }

    void RestartHandle()
    {
        Time.timeScale = 1.0f;
        PauselPanel.SetActive(false);
        LosePanel.SetActive(false);
        // TODO: replace to SceneManager.GetActiveScene().buildIndex
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void NextLevelHandle()
    {
        Time.timeScale = 1.0f;
        PauselPanel.SetActive(false);
        LosePanel.SetActive(false);
        Debug.Log("Load Next Level");
        // TODO: use it when with level loader
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void Back2TitleHandle()
    {
        Time.timeScale = 1.0f;
        PauselPanel.SetActive(false);
        LosePanel.SetActive(false);
        Debug.Log("Load Main Title");
        // TODO: rename it with main title scene
        //SceneManager.LoadScene("UI Issues");
    }

    void ContinueHandle()
    {
        PauselPanel.SetActive(false);
        LosePanel.SetActive(false);
        Time.timeScale = 1.0f;
    }
}