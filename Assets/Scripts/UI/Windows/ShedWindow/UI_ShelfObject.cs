using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ShelfObject : MonoBehaviour
{
    [Header("Elements")]
    public GameObject OuterContainer;
    public RectTransform InnerContainer;
    public Image ObjectImage;

    public TooltipTarget_Reference TooltipTarget;

    public void Init(Object obj)
    {
        ObjectImage.sprite = obj.Sprite;
        TooltipTarget.Init(obj);

        RectTransform rt = InnerContainer.GetComponent<RectTransform>();
        int yOffset = -(2 * obj.Def.SpriteBottomY) + 13;
        if (yOffset > 0) yOffset = 0;
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, yOffset);

        ObjectImage.enabled = obj.IsInShed;
    }
}
