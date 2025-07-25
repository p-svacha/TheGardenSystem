using System.Collections.Generic;
using UnityEngine;

public class ObjectTagDef : Def, INestedTooltipTarget
{
    public Color Color { get; init; } = new Color(1f, 0.5f, 0.5f);
    public string ColorHex => "#" + ColorUtility.ToHtmlStringRGB(Color);

    #region INestedTooltipTarget

    public string GetTooltipTitle() => LabelCapWord;
    public string GetToolTipBodyText(out List<INestedTooltipTarget> references)
    {
        references = new List<INestedTooltipTarget>();
        return Description;
    }

    public string NestedTooltipLinkId => $"ObjectTag_{DefName}";
    public string NestedTooltipLinkText => LabelCapWord;
    public Color NestedTooltipLinkColor => Color;

    #endregion
}
