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

    #region Getters

    /// <summary>
    /// The exact amount of resources of each type that this object produces natively.
    /// </summary>
    public virtual ResourceCollection GetNativeResourceProduction()
    {
        return new ResourceCollection(Def.BaseResources);
    }

    /// <summary>
    /// The list of resources that this object produces natively.
    /// </summary>
    public List<ResourceDef> NativeResources => GetNativeResourceProduction().GetResourceList();

    public bool HasTag(ObjectTagDef tag) => Tags.Contains(tag);
    public bool HasAnyOfTags(List<ObjectTagDef> tags) => tags.Any(t => Tags.Contains(t));
    public bool HasAllTags(List<ObjectTagDef> tags) => tags.All(t => Tags.Contains(t));

    public virtual List<ObjectTagDef> Tags => Def.Tags;
    public virtual List<ObjectEffect> Effects => Def.Effects;
    public virtual string Label => Def.Label;
    public string LabelCap => Def.LabelCap;
    public string LabelCapWord => Def.LabelCapWord;
    public virtual string Description => Def.Description;
    public virtual Sprite Sprite => Def.Sprite;

    #endregion


    #region INestedTooltipTaget

    public string GetTooltipTitle() => LabelCapWord;
    public string GetToolTipBodyText(out List<INestedTooltipTarget> dynamicReferences)
    {
        dynamicReferences = new List<INestedTooltipTarget>();

        string tags = "";
        foreach(ObjectTagDef tag in Tags)
        {
            tags += $"{tag.GetNestedTooltipLink()} ";
        }
        tags = tags.TrimEnd(' ');

        return $"{tags}\n\n{Description}";
    }

    public string NestedTooltipLinkId => $"Object_{Def.DefName}";
    public string NestedTooltipLinkText => LabelCapWord;
    public Color NestedTooltipLinkColor => NestedTooltipManager.DEFAULT_NESTED_LINK_COLOR;

    #endregion
}
