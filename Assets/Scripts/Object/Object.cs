using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Object
{
    public ObjectDef Def { get; private set; }

    public Object(ObjectDef def)
    {
        Def = def;
    }

    public virtual Dictionary<ResourceDef, int> GetBaseResourceProduction()
    {
        return new Dictionary<ResourceDef, int>(Def.BaseResources);
    }

    public bool HasTag(ObjectTagDef tag) => Tags.Contains(tag);
    public bool HasAnyOfTags(List<ObjectTagDef> tags) => tags.Any(t => Tags.Contains(t));
    public bool HasAllTags(List<ObjectTagDef> tags) => tags.All(t => Tags.Contains(t));

    public virtual List<ObjectTagDef> Tags => Def.Tags;
    public virtual List<ObjectEffect> Effects => Def.Effects;
    public virtual string Label => Def.Label;
    public virtual string LabelCap => Def.LabelCap;
    public virtual string Description => Def.Description;
    public virtual Sprite Sprite => Def.Sprite;

    public string GetTooltipDescription()
    {
        string tags = "";
        foreach(ObjectTagDef tag in Tags)
        {
            tags += $"<link=tag_{tag.DefName}><color={tag.ColorHex}>{tag.LabelCap}</color></link> ";
        }
        tags = tags.TrimEnd(' ');

        return $"{tags}\n\n{Description}";
    }
}
