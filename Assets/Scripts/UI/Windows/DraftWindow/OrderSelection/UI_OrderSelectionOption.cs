using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_OrderSelectionOption : UI_DraftOption
{
    [Header("Elements")]
    public TextMeshProUGUI CustomerNameText;
    public TextMeshProUGUI ResourcesText;
    public TextMeshProUGUI RewardText;
    public Image CheckmarkImage;

    public override void OnInit(IDraftable option)
    {
        Order order = (Order)option;

        CustomerNameText.text = order.Customer.LabelCapWord;
        ResourcesText.text = order.OrderedResources.GetAsSingleLinkedString();
        RewardText.text = order.Reward.GetAsSingleLinkedString();
    }

    public override void SetSelected(bool value)
    {
        CheckmarkImage.sprite = value ? ResourceManager.LoadSprite("Sprites/MiscIcons/Checkmark") : ResourceManager.LoadSprite("Sprites/MiscIcons/Cross");
    }
}
