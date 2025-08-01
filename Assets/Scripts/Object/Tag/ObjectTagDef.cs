using System.Collections.Generic;
using UnityEngine;

public class ObjectTagDef : Def, ITooltipTarget
{
    public Color Color { get; init; } = new Color(1f, 0.5f, 0.5f);
    public string ColorHex => "#" + ColorUtility.ToHtmlStringRGB(Color);

    #region ITooltipTarget

    public string GetTooltipTitle() => LabelCapWord;
    public string GetTooltipBodyText(List<ITooltipTarget> dynamicReferences) => Description;

    public string NestedTooltipLinkId => $"ObjectTag_{DefName}";
    public string NestedTooltipLinkText => LabelCapWord;
    public Color NestedTooltipLinkColor => Color;

    #endregion
}
