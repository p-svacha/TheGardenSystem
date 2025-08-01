using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


/// <summary>
/// Attach this to a UI element that should show a tooltip of a specific referenced ITooltipTarget when hovered.
/// </summary>
public class UI_TooltipTarget_Reference : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// The object the tooltip is shown for.
    /// </summary>
    public ITooltipTarget TooltipObject { get; private set; }

    public bool Disabled;

    /// <summary>
    /// Should be called once.
    /// </summary>
    public void Init(ITooltipTarget target)
    {
        TooltipObject = target;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Disabled) return;
        TooltipManager.Instance.NotifyObjectHovered(TooltipObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Disabled) return;
        TooltipManager.Instance.NotifyObjectUnhovered(TooltipObject);
    }
}

