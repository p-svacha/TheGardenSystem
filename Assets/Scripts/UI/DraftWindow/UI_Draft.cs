using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Draft : MonoBehaviour
{
    private static int MAX_OPTIONS_PER_ROW = 5;

    [Header("Elements")]
    public TextMeshProUGUI Title;
    public GameObject OptionsContainer;

    [Header("Prefabs")]
    public GameObject DraftRowPrefab;
    public UI_DraftOption DraftOptionPrefab;

    
    public List<IDraftable> Options { get; private set; }
    public IDraftable SelectedOption { get; private set; }
    private Dictionary<IDraftable, UI_DraftOption> OptionDisplays;

    /// <summary>
    /// Flag if this represents a draft where the player has to choose one of the available options.
    /// </summary>
    public bool IsDraft { get; private set; }

    public void Init(string title, List<IDraftable> options, bool isDraft)
    {
        Options = options;
        IsDraft = isDraft;

        Title.text = title;
        SelectedOption = null;
        OptionDisplays = new Dictionary<IDraftable, UI_DraftOption>();

        HelperFunctions.DestroyAllChildredImmediately(OptionsContainer);

        if (options == null || options.Count == 0) return;

        int counter = 0;
        GameObject currentRow = null;
        foreach (IDraftable option in options)
        {
            if (counter % MAX_OPTIONS_PER_ROW == 0) currentRow = GameObject.Instantiate(DraftRowPrefab, OptionsContainer.transform);
            UI_DraftOption optionDisplay = GameObject.Instantiate(DraftOptionPrefab, currentRow.transform);
            optionDisplay.Init(this, option);
            OptionDisplays.Add(option, optionDisplay);
            counter++;
        }
    }

    public void SetSelectedOption(IDraftable option)
    {
        if (!IsDraft) return; // Can't change selection when it's not a draft

        if (SelectedOption != null) OptionDisplays[SelectedOption].SetSelected(false);

        if (option.Equals(SelectedOption)) SelectedOption = null;
        else SelectedOption = option;

        if (SelectedOption != null) OptionDisplays[SelectedOption].SetSelected(true);
    }
}
