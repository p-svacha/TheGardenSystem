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

    /// <summary>
    /// New resource collection with an empty entry for each resource of the given category.
    /// </summary>
    public ResourceCollection(ResourceType type)
    {
        Resources = new Dictionary<ResourceDef, int>();
        foreach (ResourceDef res in DefDatabase<ResourceDef>.AllDefs.Where(r => r.Type == type)) Resources.Add(res, 0);
    }

    public ResourceCollection(ResourceCollection orig)
    {
        Resources = new Dictionary<ResourceDef, int>();
        foreach (var kvp in orig.Resources) Resources.Increment(kvp.Key, kvp.Value);
    }

    public ResourceCollection GetResourcesOfType(ResourceType type)
    {
        return new ResourceCollection(Resources.Where(r => r.Key.Type == type).ToDictionary(x => x.Key, x => x.Value));
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
    public string GetAsSingleLinkedString(int numSpacesBetweenResources = 2, string textColorHex = "")
    {
        string s = "";
        foreach (var kvp in Resources)
        {
            string valueTextColor = ResourceManager.WhiteTextColorHex;
            if (textColorHex != "") valueTextColor = textColorHex;
            else if (kvp.Value < 0) valueTextColor = ResourceManager.RedTextColorHex;

            string valueText = $"<color={valueTextColor}>{kvp.Value}</color>";
            s += $"{kvp.Key.GetTooltipLink()} {valueText}";
            for (int i = 0; i < numSpacesBetweenResources; i++) s += " ";
        }
        s = s.TrimEnd(' ');
        return s;
    }
}
