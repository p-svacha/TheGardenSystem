using System.Collections.Generic;
using UnityEngine;

public class ResourceDef : Def, INestedTooltipTarget
{
    // INestedTooltipTaget
    public string GetTooltipTitle() => LabelCap;
    public string GetToolTipBodyText() => Description;
    public List<INestedTooltipTarget> GetToolTipReferences() => new();

    public string NestedTooltipLinkId => $"Resource_{DefName}";
    public string NestedTooltipLinkText => LabelCap;
    public Color NestedTooltipLinkColor => NestedTooltipManager.DEFAULT_NESTED_LINK_COLOR;
}
