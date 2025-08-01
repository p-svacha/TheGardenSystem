using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Everything that can spawn a tooltip when hovered needs to implement this.
/// </summary>
public interface ITooltipTarget
{
    /// <summary>
    /// The title of the tooltip that gets shown when hovering this.
    /// </summary>
    public string GetTooltipTitle();

    /// <summary>
    /// The body text of the tooltip that gets shown when hovering this.
    /// <br/>Additionally returns all references pointing to dynamic objects that can be opened from this targets tooltip.
    /// <br/>References to static objects (such as Defs) do not need to be included in the references, since these links are registered statically in the NestedTooltipManager.
    /// </summary>
    public string GetTooltipBodyText(List<ITooltipTarget> dynamicReferences);

    /// <summary>
    /// The id of the TMPro link that is used to detect a hover on the text in an existing tooltip, to allow a nested tooltip for this target. 
    /// </summary>
    public string NestedTooltipLinkId { get; }

    /// <summary>
    /// The text in an existing tooltip that - when hovered - opens a nested tooltip for this target.
    /// </summary>
    public string NestedTooltipLinkText { get; }

    /// <summary>
    /// The color of the text in an existing tooltip that - when hovered - opens a nested tooltip for this target.
    /// </summary>
    public Color NestedTooltipLinkColor { get; }
}
