using UnityEngine;
using System.Collections.Generic;

public static class ObjectDefs
{
    public static List<ObjectDef> Defs => new List<ObjectDef>()
    {
        new ObjectDef()
        {
            DefName = "Carrot",
            Label = "carrot",
            Description = "1 Food",
            Sprite = ResourceManager.LoadSprite("Sprites/Objects/Carrot"),
            BaseResources = new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Food, 1 },
            },
        }
    };
}