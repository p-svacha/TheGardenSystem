using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ResourceRow : MonoBehaviour
{
    [Header("Elements")]
    public Image Icon;
    public TextMeshProUGUI ValueText;

    public void Init(ResourceDef def, int amount)
    {
        Icon.sprite = def.Sprite;
        ValueText.text = amount.ToString();
    }
}
