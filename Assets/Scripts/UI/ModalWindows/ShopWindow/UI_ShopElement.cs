using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ShopElement : MonoBehaviour
{
    [Header("Elements")]
    public Button Button;
    public Image Background;
    public Image Icon;
    public TextMeshProUGUI Text;
    public UI_TooltipTarget_Reference TooltipTarget;

    public void Init(ResourceDef resource, int amount, System.Action onClick)
    {
        string text = $"x{amount}";
        Init(resource.Sprite, text, onClick, ResourceManager.UiBackgroundLighter2);
        TooltipTarget.Init(resource);
    }

    public void Init(ObjectDef objectDef, int price, System.Action onClick)
    {
        string text = $"{ResourceDefOf.Gold.GetTooltipLink()} {price}";
        Init(objectDef.Sprite, text, onClick, objectDef.Tier.Color);
        TooltipTarget.Init(objectDef);

        bool isDiscount = objectDef == Game.Instance.ShopDiscountedObject;
        if (isDiscount) Text.color = ResourceManager.UiTextGreen;
    }

    private void Init(Sprite iconSprite, string text, System.Action onClick, Color backgroundColor)
    {
        Background.color = backgroundColor;
        Icon.sprite = iconSprite;
        Text.text = text;
        Button.onClick.AddListener(onClick.Invoke);
    }

    public void SetText(string text)
    {
        Text.text = text;
    }
}
