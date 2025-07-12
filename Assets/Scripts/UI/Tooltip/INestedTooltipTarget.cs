using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Every class that supports showing a tooltip within the nested tooltip framework must implement this.
/// </summary>
public interface INestedTooltipTarget
{
    /// <summary>
    /// The title of the tooltip that gets shown when hovering this.
    /// </summary>
    public string GetTooltipTitle();

    /// <summary>
    /// The body text of the tooltip that gets shown when hovering this.
    /// </summary>
    public string GetToolTipBodyText();

    /// <summary>
    /// Returns all references to other tooltips that can be opened from this targets tooltip.
    /// </summary>
    public List<INestedTooltipTarget> GetToolTipReferences();

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
