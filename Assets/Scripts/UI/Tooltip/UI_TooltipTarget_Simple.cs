using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attach this to a UI element to make it show a simple tooltip target with a fixed text.
/// </summary>
public class UI_TooltipTarget_Simple : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ITooltipTarget
{
    public string Text;

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipManager.Instance.NotifyObjectHovered(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.NotifyObjectUnhovered(this);
    }

    #region ITooltipTarget

    public string GetTooltipTitle() => "";
    public string GetTooltipBodyText(List<ITooltipTarget> dynamicReferences) => Text;

    public string NestedTooltipLinkId => "";
    public string NestedTooltipLinkText => "";
    public Color NestedTooltipLinkColor => TooltipManager.DEFAULT_NESTED_LINK_COLOR;

    #endregion
}
