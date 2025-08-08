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
        Init(resource.Sprite, text, onClick);
        TooltipTarget.Init(resource);
    }

    public void Init(ObjectDef objectDef, int price, System.Action onClick)
    {
        string text = $"{ResourceDefOf.Gold.GetTooltipLink()} {price}";
        ((RectTransform)Icon.transform).sizeDelta = new Vector2(64, 64);

        Init(objectDef.Sprite, text, onClick);
        TooltipTarget.Init(objectDef);

        bool isDiscount = objectDef == Game.Instance.ShopDiscountedObject;
        if (isDiscount) Text.color = ResourceManager.UiTextGreen;
    }

    private void Init(Sprite iconSprite, string text, System.Action onClick)
    {
        Icon.sprite = iconSprite;
        Text.text = text;
        Button.onClick.AddListener(onClick.Invoke);
    }

    public void SetText(string text)
    {
        Text.text = text;
    }
}
