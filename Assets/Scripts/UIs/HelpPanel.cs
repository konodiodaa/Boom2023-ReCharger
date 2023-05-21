using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpPanel : MonoBehaviour
{
    [SerializeField]
    List<Sprite> TutorialImg;

    private Button leftBTN;
    private Button rightBTN;

    private Image img;
    [SerializeField]
    private int index;

    private void Awake()
    {
        img = transform.Find("TutorialImage").GetComponent<Image>();
        leftBTN = transform.Find("LeftBTN").GetComponent<Button>();
        rightBTN = transform.Find("RightBTN").GetComponent<Button>();

        leftBTN.onClick.AddListener(LastImg);
        rightBTN.onClick.AddListener(NextImg);
    }

    private void OnDisable()
    {
        if (TutorialImg.Count != 0)
        {
            index = 0;
            img.sprite = TutorialImg[0];
        }
    }

    private void Start()
    {
        if(TutorialImg.Count != 0)
            img.sprite = TutorialImg[0];
    }

    public void NextImg()
    {
        if (index >= TutorialImg.Count - 1)
            return;
        else
            index++;

        img.sprite = TutorialImg[index];
    }

    public void LastImg()
    {
        if (index == 0) return;
        else
            index--;
        img.sprite = TutorialImg[index];
    }
}
