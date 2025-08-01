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

    public void Init(ResourceDef resource, int amount, System.Action onClick)
    {
        Init(resource.Sprite, amount.ToString(), onClick, ResourceManager.UiBackgroundLighter2);
    }

    public void Init(ObjectDef objectDef, int price, System.Action onClick)
    {
        Init(objectDef.Sprite, price.ToString(), onClick, objectDef.Tier.Color);
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
