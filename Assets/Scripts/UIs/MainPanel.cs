using System.Collections;
using System.Collections.Generic;
using Levels;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainPanel : MonoBehaviour
{

    private Transform titlePanel;
    private Transform levelSelectPanel;
    private Transform helpePanel;

    public LevelLoader levelLoader;

    private void Awake()
    {
        titlePanel = transform.Find("TitlePanel");
        levelSelectPanel = transform.Find("LevelSelectPanel");
        helpePanel = transform.Find("HelpPanel");

        EventCenter.AddListener(EventDefine.StartClicked, StartGame);
        EventCenter.AddListener(EventDefine.QuitClicked, QuitGame);
        EventCenter.AddListener(EventDefine.LevelSelectClicked, OpenLevelSelectPanel);
        EventCenter.AddListener(EventDefine.Back2Title, BackToTitlePanel);
        EventCenter.AddListener(EventDefine.SettingClicked, OpenSettingPanel);
        EventCenter.AddListener(EventDefine.HelpClicked, OpenHelpPanel);
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.StartClicked, StartGame);
        EventCenter.RemoveListener(EventDefine.QuitClicked, QuitGame);
        EventCenter.RemoveListener(EventDefine.LevelSelectClicked, OpenLevelSelectPanel);
        EventCenter.RemoveListener(EventDefine.Back2Title, BackToTitlePanel);
        EventCenter.RemoveListener(EventDefine.SettingClicked, OpenSettingPanel);
        EventCenter.RemoveListener(EventDefine.HelpClicked, OpenHelpPanel);

    }

    private void StartGame()
    {
        levelLoader.LoadLevel(1);
    }

    private void QuitGame()
    {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
                Application.Quit();
    #endif
    }

    private void OpenLevelSelectPanel()
    {
        titlePanel.gameObject.SetActive(false);
        helpePanel.gameObject.SetActive(false);
        levelSelectPanel.gameObject.SetActive(true);
    }

    private void BackToTitlePanel()
    {
        levelSelectPanel.gameObject.SetActive(false);
        helpePanel.gameObject.SetActive(false);
        titlePanel.gameObject.SetActive(true);
    }

    private void OpenSettingPanel()
    {
        titlePanel.gameObject.SetActive(false);
        levelSelectPanel.gameObject.SetActive(false);
        helpePanel.gameObject.SetActive(true);
    }

    private void OpenHelpPanel()
    {
        titlePanel.gameObject.SetActive(false);
        levelSelectPanel.gameObject.SetActive(false);
        helpePanel.gameObject.SetActive(true);
    }
}
