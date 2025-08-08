using System.Collections.Generic;
using UnityEngine;

public class ResourceDef : Def, ITooltipTarget
{
    public override Sprite Sprite => ResourceManager.LoadSprite("Sprites/Resources/" + DefName);
    public ResourceType Type { get; init; }

    #region ITooltipTarget

    public string GetTooltipTitle() => LabelCap;
    public string GetTooltipBodyText(List<ITooltipTarget> dynamicReferences) => Description;

    public string NestedTooltipLinkId => $"Resource_{DefName}";
    public string NestedTooltipLinkText => $"<sprite name=\"{DefName}\">";
    public Color NestedTooltipLinkColor => Color.white;

    #endregion
}
