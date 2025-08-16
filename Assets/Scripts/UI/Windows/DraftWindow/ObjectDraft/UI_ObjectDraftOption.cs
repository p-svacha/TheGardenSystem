using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ObjectDraftOption : UI_DraftOption
{
    [Header("Elements")]
    public Image Background;
    public TextMeshProUGUI TitleText;
    public Image Image;
    public TextMeshProUGUI ResourcesText;
    public TextMeshProUGUI EffectText;
    public TextMeshProUGUI TagsText;

    public override void OnInit(IDraftable option)
    {
        ObjectDef objectDef = (ObjectDef)option;

        TitleText.text = objectDef.LabelCapWord;
        TagsText.text = objectDef.GetTagsAsLinkedString();
        Image.sprite = objectDef.Sprite;
        EffectText.text = objectDef.GetEffectDescription();
        ResourcesText.text = objectDef.GetNativeProduction().GetAsSingleLinkedString();
    }

    public override void SetSelected(bool value)
    {
        Background.sprite = value ? ResourceManager.LoadSprite("Sprites/UI/UIPanel_Simple_Lighter") : ResourceManager.LoadSprite("Sprites/UI/UIPanel_Simple");
    }
}
