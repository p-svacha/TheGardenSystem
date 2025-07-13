using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


/// <summary>
/// Attach this to a UI element that should show a tooltip when hovered
/// </summary>
public class TooltipTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// The object the tooltip is shown for.
    /// </summary>
    public INestedTooltipTarget TooltipObject { get; private set; }

    public bool Disabled;

    /// <summary>
    /// Should be called once.
    /// </summary>
    public void Init(INestedTooltipTarget target)
    {
        TooltipObject = target;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Disabled) return;
        NestedTooltipManager.Instance.NotifyObjectHovered(TooltipObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Disabled) return;
        NestedTooltipManager.Instance.NotifyObjectUnhovered(TooltipObject);
    }
}

