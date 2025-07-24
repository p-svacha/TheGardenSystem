using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class UI_DraftOption : MonoBehaviour
{
    [Header("Base Elements")]
    public Button OptionSelectionButton;

    /// <summary>
    /// Initializes the given draft option.
    /// </summary>
    public void Init(UI_Draft draft, IDraftable option)
    {
        OptionSelectionButton.onClick.AddListener(() => draft.ToggleOption(option));

        OnInit(option);
    }

    /// <summary>
    /// Initializes the display of the given draft option.
    /// </summary>
    public abstract void OnInit(IDraftable option);

    /// <summary>
    /// Draws this draft option as being selected or unselected.
    /// </summary>
    public abstract void SetSelected(bool value);
}
