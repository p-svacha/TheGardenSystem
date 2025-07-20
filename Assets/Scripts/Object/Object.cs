using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Object : INestedTooltipTarget
{
    public ObjectDef Def { get; private set; }

    public Object(ObjectDef def)
    {
        Def = def;
    }

    /// <summary>
    /// The exact amount of resources of each type that this object produces natively.
    /// </summary>
    public virtual Dictionary<ResourceDef, int> GetNativeResourceProduction()
    {
        return new Dictionary<ResourceDef, int>(Def.BaseResources);
    }

    /// <summary>
    /// The list of resources that this object produces natively.
    /// </summary>
    public List<ResourceDef> NativeResources => GetNativeResourceProduction().Keys.ToList();

    public bool HasTag(ObjectTagDef tag) => Tags.Contains(tag);
    public bool HasAnyOfTags(List<ObjectTagDef> tags) => tags.Any(t => Tags.Contains(t));
    public bool HasAllTags(List<ObjectTagDef> tags) => tags.All(t => Tags.Contains(t));

    public virtual List<ObjectTagDef> Tags => Def.Tags;
    public virtual List<ObjectEffect> Effects => Def.Effects;
    public virtual string Label => Def.Label;
    public virtual string LabelCap => Def.LabelCap;
    public virtual string Description => Def.Description;
    public virtual Sprite Sprite => Def.Sprite;


    #region INestedTooltipTaget

    public string GetTooltipTitle() => LabelCap;
    public string GetToolTipBodyText()
    {
        string tags = "";
        foreach(ObjectTagDef tag in Tags)
        {
            tags += $"{tag.GetNestedTooltipLink()} ";
        }
        tags = tags.TrimEnd(' ');

        return $"{tags}\n\n{Description}";
    }
    public List<INestedTooltipTarget> GetToolTipReferences() => Tags.Select(t => (INestedTooltipTarget)t).ToList();

    public string NestedTooltipLinkId => $"Object_{Def.DefName}";
    public string NestedTooltipLinkText => LabelCap;
    public Color NestedTooltipLinkColor => NestedTooltipManager.DEFAULT_NESTED_LINK_COLOR;

    #endregion
}
