using UnityEngine;
using System.Collections.Generic;

public class ObjectDef : Def, IDraftable
{
    public List<ObjectTagDef> Tags { get; init; } = new();
    public Dictionary<ResourceDef, int> BaseResources { get; init; } = new();
    public List<ObjectEffect> Effects { get; init; } = new();
    new public Sprite Sprite => ResourceManager.LoadSprite("Sprites/Objects/" + DefName);

    // IDraftable
    public string DraftDisplay_Title => LabelCapWord;
    public string DraftDisplay_Description => Description;
    public Sprite DraftDisplay_Sprite => Sprite;
}
