using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UI_ObjectDraftWindow : UI_WindowBase
{
    public static UI_ObjectDraftWindow Instance;

    [Header("Elements")]
    public UI_Draft Draft;

    [Header("Prefabs")]
    public UI_ObjectDraftOption ObjectDraftOptionPrefab;

    /// <summary>
    /// The function that gets executed when closing / confirming this window. The IDraftables passed represent the chosen options.
    /// </summary>
    private System.Action<List<IDraftable>> Callback;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    public void ShowObjectDraft(string title, string subtitle, List<ObjectDef> objects, System.Action<List<IDraftable>> callback)
    {
        List<IDraftable> draftOptions = objects.Select(x => (IDraftable)x).ToList();

        Init(title, Confirm);
        Callback = callback;
        Draft.Init(subtitle, draftOptions, ObjectDraftOptionPrefab, minSelectableOptions: 0, maxSelectableOptions: 1, maxOptionsDisplayedPerRow: 5, initiallyAllSelected: false);
        gameObject.SetActive(true);
    }

    private void Confirm()
    {
        // Validate draft
        if (!Draft.IsValidDraft()) return;

        // Hide
        gameObject.SetActive(false);

        // Callback
        Callback?.Invoke(new List<IDraftable>(Draft.SelectedOptions));
    }
}
