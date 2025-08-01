using UnityEngine;

/// <summary>
/// Extension needed to allow having a default implementation of GetTooltipLink() in the ITooltipTarget
/// interface that can be used to easily create text in tooltips, that when hovered, creates nested tooltips on top.
/// </summary>
public static class ITooltipTargetExtensions
{
    /// <summary>
    /// The complete TMPro-tagged link to use in an existing tooltip, that opens a nested tooltip when hovered.
    /// </summary>
    public static string GetTooltipLink(this ITooltipTarget target)
    {
        var hexColor = ColorUtility.ToHtmlStringRGB(target.NestedTooltipLinkColor);
        return $"<nobr><link={target.NestedTooltipLinkId}><color=#{hexColor}>{target.NestedTooltipLinkText}</color></link></nobr>";
    }
}
