using System.Collections.Generic;
using UnityEngine;

public class ResourceDef : Def, INestedTooltipTarget
{
    new public Sprite Sprite => ResourceManager.LoadSprite("Sprites/Resources/" + DefName);
    public ResourceType Type { get; init; }

    // INestedTooltipTaget
    public string GetTooltipTitle() => LabelCap;
    public string GetToolTipBodyText(out List<INestedTooltipTarget> references)
    {
        references = new List<INestedTooltipTarget>();
        return Description;
    }

    public string NestedTooltipLinkId => $"Resource_{DefName}";
    public string NestedTooltipLinkText => $"<sprite name=\"{DefName}\">";
    public Color NestedTooltipLinkColor => NestedTooltipManager.DEFAULT_NESTED_LINK_COLOR;
}
