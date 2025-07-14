using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_DraftOption : MonoBehaviour
{
    [Header("Elements")]
    public Image SelectionFrame;
    public Button Button;
    public TextMeshProUGUI Title;
    public Image Image;

    public void Init(UI_Draft draft, IDraftable option)
    {
        Button.onClick.AddListener(() => draft.SetSelectedOption(option));

        // Title
        if (option.DraftDisplay_Title != null && option.DraftDisplay_Title != "")
        {
            Title.gameObject.SetActive(true);
            Title.text = option.DraftDisplay_Title;
        }
        else Title.gameObject.SetActive(false);

        // Sprite
        if (option.DraftDisplay_Sprite != null)
        {
            Image.gameObject.SetActive(true);
            Image.sprite = option.DraftDisplay_Sprite;
        }
        else Image.gameObject.SetActive(false);
    }

    public void SetSelected(bool value)
    {
        
    }
}
