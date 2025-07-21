using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

/// <summary>
/// Modular window for drafting or displaying any amount of IDraftables.
/// </summary>
public class UI_DraftWindow : MonoBehaviour
{
    public static UI_DraftWindow Instance;

    [Header("Elements")]
    public TextMeshProUGUI Title;
    public UI_Draft Draft;
    public Button ConfirmButton;

    /// <summary>
    /// The function that gets executed when closing / confirming this window. The IDraftables passed represent the chosen options.
    /// </summary>
    private System.Action<List<IDraftable>> Callback;

    private void Awake()
    {
        Instance = this;
        ConfirmButton.onClick.AddListener(Confirm);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Shows the draft window with the given options.
    /// If isDraft is false, this represents information without user input.
    /// The callback gets called with a list of all selected options (or all options if isDraft is false).
    /// </summary>
    public void Show(string title, string subtitle, List<IDraftable> options, bool isDraft = true, System.Action<List<IDraftable>> callback = null)
    {
        Callback = callback;

        gameObject.SetActive(true);
        Title.text = title;
        Draft.Init(subtitle, options, isDraft);
    }

    private void Confirm()
    {
        List<IDraftable> chosenOptions = new List<IDraftable>();

        // Apply options
        if (Draft.IsDraft)
        {
            if (Draft.SelectedOption == null) chosenOptions = new List<IDraftable>();
            else chosenOptions = new List<IDraftable>() { Draft.SelectedOption };
        }
        else
        {
            if (Draft.Options != null)
            {
                foreach (IDraftable draftable in Draft.Options)
                {
                    chosenOptions.Add(draftable);
                }
            }
        }

        // Hide
        gameObject.SetActive(false);

        // Callback
        Callback?.Invoke(chosenOptions);
    }
}
