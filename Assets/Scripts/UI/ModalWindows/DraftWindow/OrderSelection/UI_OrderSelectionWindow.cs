using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class UI_OrderSelectionWindow : UI_WindowBase
{
    public static UI_OrderSelectionWindow Instance;

    private ResourceCollection RemainingResources;

    [Header("Elements")]
    public UI_Draft Draft;
    public TextMeshProUGUI RemainingResourcesText;

    [Header("Prefabs")]
    public UI_OrderSelectionOption OrderDraftOptionPrefab;

    /// <summary>
    /// The function that gets executed when closing / confirming this window. The IDraftables passed represent the chosen options.
    /// </summary>
    private System.Action<List<IDraftable>> Callback;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    public void ShowOrderSelection(string title, string subtitle, List<Order> orders, System.Action<List<IDraftable>> callback)
    {
        List<IDraftable> draftOptions = orders.Select(x => (IDraftable)x).ToList();

        Init(title, Confirm);
        Callback = callback;
        Draft.Init(subtitle, draftOptions, OrderDraftOptionPrefab, minSelectableOptions: 0, maxSelectableOptions: -1, maxOptionsDisplayedPerRow: 1, initiallyAllSelected: true, OnOrderSelectionChanged);
        gameObject.SetActive(true);
    }

    private void OnOrderSelectionChanged()
    {
        RecalculateAndDisplayRemainingResources();
    }

    private void RecalculateAndDisplayRemainingResources()
    {
        // Calculate
        RemainingResources = new ResourceCollection(allowNegativeValues: true);
        RemainingResources.AddResources(Game.Instance.Resources);

        List<Order> ordersToDeliver = Draft.SelectedOptions.Select(x => (Order)x).ToList();
        foreach(Order order in ordersToDeliver)
        {
            RemainingResources.RemoveResources(order.OrderedResources);
            RemainingResources.AddResources(order.Reward);
        }

        // Display
        RemainingResourcesText.text = RemainingResources.GetAsSingleLinkedString();

        // Enable / Disable confirm
        if (RemainingResources.HasNegativeValues()) ConfirmButton.interactable = false;
        else ConfirmButton.interactable = true;
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
