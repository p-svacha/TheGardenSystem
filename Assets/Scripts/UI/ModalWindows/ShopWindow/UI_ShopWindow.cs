using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UI_ShopWindow : UI_WindowBase
{
    public static UI_ShopWindow Instance;

    [Header("Elements")]
    public TextMeshProUGUI RestockText;
    public UI_ShopContainer YourGoods;
    public UI_ShopContainer Selling;
    public UI_ShopContainer ShopGoods;
    public UI_ShopContainer Buying;

    public TextMeshProUGUI SummaryText;
    public Button CloseButton;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        CloseButton.onClick.AddListener(Close);
        ConfirmButton.onClick.AddListener(Confirm);
    }

    public void Show()
    {
        string sellText = $"sell for {Game.RESOURCE_SELL_VALUE} {ResourceDefOf.Gold.GetTooltipLink()} / resource";
        YourGoods.Init(this, sellText, Game.Instance.Resources.GetResourcesOfType(ResourceType.MarketResource), showObjects: false, new(), Selling);
        ResourceCollection sellResources = new ResourceCollection(ResourceType.MarketResource);
        Selling.Init(this, "", sellResources.Resources, showObjects: false, new(), YourGoods);

        string buyText = $"buy for {Game.RESOURCE_BUY_PRICE} {ResourceDefOf.Gold.GetTooltipLink()} / resource";
        ShopGoods.Init(this, buyText, Game.Instance.ShopResources.Resources, showObjects: true, Game.Instance.ShopObjects, Buying);
        ResourceCollection buyResources = new ResourceCollection(ResourceType.MarketResource);
        Buying.Init(this, "", buyResources.Resources, showObjects: true, new(), ShopGoods);

        RefreshSummaryText();

        gameObject.SetActive(true);
    }

    public void MoveResource(ResourceDef resource, int amount, UI_ShopContainer source, UI_ShopContainer target)
    {
        if (source.Resources[resource] <= amount) amount = source.Resources[resource];
        source.RemoveResource(resource, amount);
        target.AddResource(resource, amount);
        RefreshSummaryText();
    }

    public void MoveObject(ObjectDef objectDef, int price, UI_ShopContainer source, UI_ShopContainer target)
    {
        source.RemoveObject(objectDef);
        target.AddObject(objectDef, price);
        RefreshSummaryText();
    }

    private void RefreshSummaryText()
    {

    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

    private void Confirm()
    {
        gameObject.SetActive(false);
    }
}
