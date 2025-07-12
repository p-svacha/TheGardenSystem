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

    [HideInInspector] public bool IsFocussed;
    private float Delay = 0.5f;
    [HideInInspector] public float CurrentDelay;

    /// <summary>
    /// Should be called once.
    /// </summary>
    public void Init(INestedTooltipTarget target)
    {
        TooltipObject = target;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsFocussed = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip();
        CurrentDelay = 0;
    }

    private void Update()
    {
        if (Disabled) return;
        if (IsFocussed)
        {
            if (CurrentDelay < Delay) CurrentDelay += UnityEngine.Time.deltaTime;
            else ShowTooltip();
        }
    }

    private void ShowTooltip()
    {
        NestedTooltipManager.Instance.NotifyObjectHovered(TooltipObject);
    }

    public void HideTooltip()
    {
        NestedTooltipManager.Instance.NotifyObjectUnhovered(TooltipObject);
    }
}

