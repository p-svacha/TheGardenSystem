using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_OrderSelectionWindow : UI_WindowBase
{
    public static UI_OrderSelectionWindow Instance;

    [Header("Elements")]
    public UI_Draft Draft;

    [Header("Prefabs")]
    public UI_OrderSelectionOption OrderDraftOptionPrefab;

    /// <summary>
    /// The function that gets executed when closing / confirming this window. The IDraftables passed represent the chosen options.
    /// </summary>
    private System.Action<List<IDraftable>> Callback;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void ShowOrderSelection(string title, string subtitle, List<Order> orders, System.Action<List<IDraftable>> callback)
    {
        List<IDraftable> draftOptions = orders.Select(x => (IDraftable)x).ToList();

        Init(title, Confirm);
        Callback = callback;
        Draft.Init(subtitle, draftOptions, OrderDraftOptionPrefab, minSelectableOptions: 0, maxSelectableOptions: -1, maxOptionsDisplayedPerRow: 1, initiallyAllSelected: true);
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
