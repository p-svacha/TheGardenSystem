using UnityEngine;
using System.Collections.Generic;

public class ObjectDef : Def
{
    public Dictionary<ResourceDef, int> BaseResources { get; init; } = new();
}
