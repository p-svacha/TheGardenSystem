using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
using System.Collections.Generic;

public class UI_ShopWindow : UI_WindowBase
{
    public static UI_ShopWindow Instance;

    [Header("Elements")]
    public TextMeshProUGUI RestockText;
    public Image HelpButton;
    public UI_ShopContainer YourGoods;
    public UI_ShopContainer Selling;
    public UI_ShopContainer ShopGoods;
    public UI_ShopContainer Buying;

    public TextMeshProUGUI SummaryText;
    public Button CloseButton;

    /// <summary>
    /// Positive means player gets gold, negative player pays gold.
    /// </summary>
    public int CurrentFinalBalance { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        CloseButton.onClick.AddListener(Close);
        ConfirmButton.SetOnClick(Confirm);
        InitHelpButton();
    }

    private void InitHelpButton()
    {
        HelpButton.GetComponent<UI_TooltipTarget_Simple>().SpawnsInstantTooltip = true;
        HelpButton.GetComponent<UI_TooltipTarget_Simple>().Text = "Welcome to the shop." +
            "\n\nHere you can sell and buy resources, as well as buy a selection of objects." +
            "\n\nThe shops stock will refill and change at the start of each month. Each month one object will be on sale." +
            "\n\nClick on any resource or object to add or remove it from the current trade. Hold shift to move 10 resources at a time." +
            "\n\nSold resources will stay in the shops inventory until the end of the month and can be bought back at a loss." +
            "\n\nComplete the trade by clicking 'Confirm'. You see the amount of gold will pay or receive next to the button.";
    }

    public void Show()
    {
        // Header
        RestockText.text = $"Next restock in {Game.Instance.DaysUntilNextMonth()} days";

        // Sell
        string sellText = $"sell for {Game.RESOURCE_SELL_VALUE} {ResourceDefOf.Gold.GetTooltipLink()} / resource";
        YourGoods.Init(this, showEmptyResources: true, sellText, Game.Instance.Resources.GetResourcesOfType(ResourceType.MarketResource), showObjects: false, new(), Selling);
        ResourceCollection sellResources = new ResourceCollection(ResourceType.MarketResource);
        Selling.Init(this, showEmptyResources: false, "", sellResources, showObjects: false, new(), YourGoods);

        // Buy
        string buyText = $"buy for {Game.RESOURCE_BUY_PRICE} {ResourceDefOf.Gold.GetTooltipLink()} / resource";
        ShopGoods.Init(this, showEmptyResources: true, buyText, Game.Instance.ShopResources, showObjects: true, new Dictionary<ObjectDef, int>(Game.Instance.ShopObjects), Buying);
        ResourceCollection buyResources = new ResourceCollection(ResourceType.MarketResource);
        Buying.Init(this, showEmptyResources: false, "", buyResources, showObjects: true, new(), ShopGoods);

        RecalculateFinalBalance();

        gameObject.SetActive(true);
    }

    public void MoveResource(ResourceDef resource, UI_ShopContainer source, UI_ShopContainer target)
    {
        int amount = 1;
        if (Input.GetKey(KeyCode.LeftShift)) amount = 10;
        if (source.Resources.Resources[resource] <= amount) amount = source.Resources.Resources[resource];
        source.RemoveResource(resource, amount);
        target.AddResource(resource, amount);
        RecalculateFinalBalance();
    }

    public void MoveObject(ObjectDef objectDef, int price, UI_ShopContainer source, UI_ShopContainer target)
    {
        source.RemoveObject(objectDef);
        target.AddObject(objectDef, price);
        RecalculateFinalBalance();
    }

    private void RecalculateFinalBalance()
    {
        // Balance calculation
        float exactBalance = 0f;

        foreach (var res in Buying.Resources.Resources) exactBalance -= res.Value * Game.RESOURCE_BUY_PRICE;
        foreach (var obj in Buying.Objects) exactBalance -= obj.Value;

        foreach (var res in Selling.Resources.Resources) exactBalance += res.Value * Game.RESOURCE_SELL_VALUE;

        CurrentFinalBalance = (int)exactBalance;

        // Summary text
        if (CurrentFinalBalance < 0) SummaryText.text = $"You will pay {Mathf.Abs(CurrentFinalBalance)} {ResourceDefOf.Gold.GetTooltipLink()}";
        else SummaryText.text = $"You will receive {CurrentFinalBalance} {ResourceDefOf.Gold.GetTooltipLink()}";
        SummaryText.color = CanCompleteTrade() ? ResourceManager.UiTextDefault : ResourceManager.UiTextRed;

        // Enable / Disable confirm
        ConfirmButton.SetInteractable(CanCompleteTrade());
    }

    public bool CanCompleteTrade()
    {
        if (CurrentFinalBalance < 0 && Mathf.Abs(CurrentFinalBalance) > Game.Instance.Resources.Resources[ResourceDefOf.Gold]) return false;
        return true;
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

    private void Confirm()
    {
        if (!CanCompleteTrade()) return;

        Game.Instance.CompleteTrade();

        gameObject.SetActive(false);
    }
}
