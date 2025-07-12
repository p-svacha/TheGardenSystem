using System.Collections.Generic;
using UnityEngine;

public class ObjectTagDef : Def, INestedTooltipTarget
{
    public Color Color { get; init; } = new Color(1f, 0.5f, 0.5f);
    public string ColorHex => "#" + ColorUtility.ToHtmlStringRGB(Color);

    // INestedTooltipTaget
    public string GetTooltipTitle() => LabelCap;
    public string GetToolTipBodyText() => Description;
    public List<INestedTooltipTarget> GetToolTipReferences() => new();

    public string NestedTooltipLinkId => $"ObjectTag_{DefName}";
    public string NestedTooltipLinkText => LabelCap;
    public Color NestedTooltipLinkColor => Color;
}
