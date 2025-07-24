using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attach this to a UI element to make it show a simple tooltip target with a fixed text.
/// </summary>
public class SimpleTooltipTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, INestedTooltipTarget
{
    public string Text;

    public void OnPointerEnter(PointerEventData eventData)
    {
        NestedTooltipManager.Instance.NotifyObjectHovered(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        NestedTooltipManager.Instance.NotifyObjectUnhovered(this);
    }

    #region INestedTooltipTarget

    public string GetTooltipTitle() => "";
    public string GetToolTipBodyText(out List<INestedTooltipTarget> references)
    {
        references = new List<INestedTooltipTarget>();
        return Text;
    }

    public string NestedTooltipLinkId => "";
    public string NestedTooltipLinkText => "";
    public Color NestedTooltipLinkColor => NestedTooltipManager.DEFAULT_NESTED_LINK_COLOR;

    #endregion
}
