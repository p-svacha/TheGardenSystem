using System.Collections.Generic;
using UnityEngine;

public static class ResourceDefs
{
    public static List<ResourceDef> Defs => new List<ResourceDef>()
    {
        new ResourceDef()
        {
            DefName = "Food",
            Label = "food",
            Description = "Basic edible output from plants and animals. Most customers request food regularly.",
            Sprite = ResourceManager.LoadSprite("Sprites/Resources/Food")
        }
    };
}
