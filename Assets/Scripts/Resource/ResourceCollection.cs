using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A helper class for a collection of resources, providing some useful functions
/// </summary>
public class ResourceCollection : MonoBehaviour
{
    public Dictionary<ResourceDef, int> Resources { get; private set; }
    public bool AllowNegativeValues { get; private set; }

    public ResourceCollection(bool allowNegativeValues = false)
    {
        Resources = new Dictionary<ResourceDef, int>();
        AllowNegativeValues = allowNegativeValues;
    }

    public ResourceCollection(Dictionary<ResourceDef, int> res)
    {
        Resources = res;
    }

    public ResourceCollection(ResourceCollection orig)
    {
        Resources = new Dictionary<ResourceDef, int>();
        foreach (var kvp in orig.Resources) Resources.Increment(kvp.Key, kvp.Value);
    }

    public void AddResource(ResourceDef def, int amount)
    {
        Resources.Increment(def, amount);
    }
    public void AddResources(ResourceCollection other)
    {
        Resources.IncrementMultiple(other.Resources);
    }

    public void RemoveResource(ResourceDef def, int amount)
    {
        Resources.Decrement(def, amount, allowNegativeValues: AllowNegativeValues);
    }
    public void RemoveResources(ResourceCollection other)
    {
        Resources.DecrementMultiple(other.Resources, allowNegativeValues: AllowNegativeValues);
    }

    public bool IsEmpty => Resources == null || Resources.Count == 0 || Resources.All(x => x.Value == 0);

    /// <summary>
    /// Checks and returns if this collection has at least all the resources in the provided collection.
    /// </summary>
    public bool HasResources(ResourceCollection res)
    {
        if (res.Resources.Any(x => !Resources.ContainsKey(x.Key) || Resources[x.Key] < x.Value)) return false;
        return true;
    }

    public bool HasNegativeValues() => Resources.Values.Any(x => x < 0);

    /// <summary>
    /// The list of resources that are in this collection.
    /// </summary>
    public List<ResourceDef> GetResourceList()
    {
        return Resources.Keys.ToList();
    }

    /// <summary>
    /// Returns this collection as a single string, with TMPro link tooltip references to the resources.
    /// </summary>
    public string GetAsSingleLinkedString(int numSpacesBetweenResources = 2)
    {
        string s = "";
        foreach (var kvp in Resources)
        {
            string valueText = kvp.Value >= 0 ? kvp.Value.ToString() : $"<color=#E07568>{kvp.Value}</color>";
            s += $"{kvp.Key.GetTooltipLink()} {valueText}";
            for (int i = 0; i < numSpacesBetweenResources; i++) s += " ";
        }
        s = s.TrimEnd(' ');
        return s;
    }
}
