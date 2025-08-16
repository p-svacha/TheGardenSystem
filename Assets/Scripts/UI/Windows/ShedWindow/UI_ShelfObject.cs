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
    }
}
