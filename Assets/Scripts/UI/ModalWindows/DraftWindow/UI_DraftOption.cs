using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_DraftOption : MonoBehaviour
{
    [Header("Elements")]
    public Image SelectionFrame;
    public Image InnerFrame;
    public Button Button;
    public TextMeshProUGUI TitleText;
    public Image Image;
    public TextMeshProUGUI DescriptionPreText;
    public TextMeshProUGUI DescriptionMainText;
    public TextMeshProUGUI DescriptionPostText;

    public void Init(UI_Draft draft, IDraftable option)
    {
        Button.onClick.AddListener(() => draft.SetSelectedOption(option));

        // Title
        if (option.DraftDisplay_Title != null && option.DraftDisplay_Title != "")
        {
            TitleText.gameObject.SetActive(true);
            TitleText.text = option.DraftDisplay_Title;
        }
        else TitleText.gameObject.SetActive(false);

        // Sprite
        if (option.DraftDisplay_Sprite != null)
        {
            Image.gameObject.SetActive(true);
            Image.sprite = option.DraftDisplay_Sprite;
        }
        else Image.gameObject.SetActive(false);

        // Description Pre
        if (option.DraftDisplay_DescriptionPre != null)
        {
            DescriptionPreText.gameObject.SetActive(true);
            DescriptionPreText.text = option.DraftDisplay_DescriptionPre;
        }
        else DescriptionPreText.gameObject.SetActive(false);

        // Description Main
        if (option.DraftDisplay_DescriptionMain != null)
        {
            DescriptionMainText.gameObject.SetActive(true);
            DescriptionMainText.text = option.DraftDisplay_DescriptionMain;
        }
        else DescriptionMainText.gameObject.SetActive(false);

        // Description Post
        if (option.DraftDisplay_DescriptionPost != null)
        {
            DescriptionPostText.gameObject.SetActive(true);
            DescriptionPostText.text = option.DraftDisplay_DescriptionPost;
        }
        else DescriptionPostText.gameObject.SetActive(false);
    }

    public void SetSelected(bool value)
    {
        InnerFrame.color = value ? ResourceManager.UiBackgroundLighter2 : ResourceManager.UiBackgroundLighter1;
    }
}
