using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UI_Draft : MonoBehaviour
{
    [Header("Elements")]
    public TextMeshProUGUI Title;
    public GameObject OptionsContainer;

    [Header("Prefabs")]
    public GameObject DraftRowPrefab;

    public int MinSelectableOptions { get; private set; }
    public int MaxSelectableOptions { get; private set; }


    public List<IDraftable> Options { get; private set; }
    public List<IDraftable> SelectedOptions { get; private set; }
    private Dictionary<IDraftable, UI_DraftOption> OptionDisplays;

    /// <summary>
    /// Initializes the draft with the given options and attributes.
    /// </summary>
    public void Init(string title, List<IDraftable> options, UI_DraftOption draftOptionPrefab, int minSelectableOptions = 0, int maxSelectableOptions = 1, int maxOptionsDisplayedPerRow = 5, bool initiallyAllSelected = false)
    {
        if (initiallyAllSelected && (maxSelectableOptions != -1 && maxSelectableOptions < options.Count))
            throw new System.Exception($"Can't initially select all options because maxSelectableOptions is set to {maxSelectableOptions}, which is smaller than the amount of options ({options.Count}).");
        if (minSelectableOptions > options.Count)
            throw new System.Exception($"minSelectableOptions ({minSelectableOptions}) may not be higher than the amount of options provided ({options.Count}).");

        OptionDisplays = new Dictionary<IDraftable, UI_DraftOption>();
        SelectedOptions = new List<IDraftable>();
        Options = options;

        MinSelectableOptions = minSelectableOptions;
        MaxSelectableOptions = maxSelectableOptions;

        Title.text = title;

        HelperFunctions.DestroyAllChildredImmediately(OptionsContainer);

        if (options == null || options.Count == 0) return;

        int counter = 0;
        GameObject currentRow = null;
        foreach (IDraftable option in options)
        {
            if (counter % maxOptionsDisplayedPerRow == 0) // Create new row
            {
                currentRow = GameObject.Instantiate(DraftRowPrefab, OptionsContainer.transform);
                HelperFunctions.DestroyAllChildredImmediately(currentRow);
            }
            UI_DraftOption optionDisplay = GameObject.Instantiate(draftOptionPrefab, currentRow.transform);
            optionDisplay.Init(this, option);
            OptionDisplays.Add(option, optionDisplay);
            counter++;
        }

        foreach(IDraftable draftOption in OptionDisplays.Keys)
        {
            if (initiallyAllSelected) SelectOption(draftOption);
            else DeselectOption(draftOption);
        }
    }

    public void ToggleOption(IDraftable option)
    {
        if (SelectedOptions.Contains(option)) DeselectOption(option);
        else SelectOption(option);
    }

    public void SelectOption(IDraftable option)
    {
        // Unselect currently selected options if only one pick possible
        if (MaxSelectableOptions == 1)
        {
            while (SelectedOptions.Count > 0) DeselectOption(SelectedOptions.First());
        }

        // Select new option
        SelectedOptions.Add(option);
        OptionDisplays[option].SetSelected(true);
    }

    public void DeselectOption(IDraftable option)
    {
        SelectedOptions.Remove(option);
        OptionDisplays[option].SetSelected(false);
    }

    /// <summary>
    /// Returns if the currently selected options match the draft constraints.
    /// </summary>
    /// <returns></returns>
    public bool IsValidDraft()
    {
        if (SelectedOptions.Count < MinSelectableOptions) return false;
        if (MaxSelectableOptions != -1 && SelectedOptions.Count > MaxSelectableOptions) return false;
        return true;
    }
}
