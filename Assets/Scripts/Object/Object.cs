using System.Collections.Generic;
using UnityEngine;

public class Object
{
    public ObjectDef Def { get; private set; }

    public Object(ObjectDef def)
    {
        Def = def;
    }

    public virtual Dictionary<ResourceDef, int> GetBaseResources()
    {
        return new Dictionary<ResourceDef, int>(Def.BaseResources);
    }

    public virtual string Label => Def.Label;
    public virtual string LabelCap => Def.LabelCap;
    public virtual string Description => Def.Description;
    public virtual Sprite Sprite => Def.Sprite;
}
