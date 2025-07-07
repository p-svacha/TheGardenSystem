using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object storing information of how much of a specific resource is produced by an obect or a collection of objects.
/// </summary>
public class ResourceProduction
{
    /// <summary>
    /// The label/title of what this production is for / who is producing this.
    /// </summary>
    public string Label { get; private set; }

    /// <summary>
    /// The resource produced.
    /// </summary>
    public ResourceDef Resource { get; private set; }

    /// <summary>
    /// The base production value.
    /// </summary>
    public int BaseValue { get; private set; }

    /// <summary>
    /// The set of modifiers that transform the base value, resulting in the final production value.
    /// </summary>
    public List<ProductionModifier> Modifiers { get; private set; }

    public ResourceProduction(string label, ResourceDef resourceDef, int baseValue)
    {
        Label = label;
        Resource = resourceDef;
        BaseValue = baseValue;
        Modifiers = new List<ProductionModifier>();
    }

    public void AddModifier(ProductionModifier modifier)
    {
        Modifiers.Add(modifier);
    }

    /// <summary>
    /// Returns the final value of this production.
    /// </summary>
    public int GetValue()
    {
        float value = BaseValue;

        // Apply stat parts
        foreach (ProductionModifier modifier in GetOrderedModifiers())
        {
            modifier.TransformValue(ref value);
        }

        return (int)value;
    }

    public string GetBreakdownString()
    {
        // Label
        string text = Resource.LabelCap;

        // Description
        if (Resource.Description != "")
        {
            text += "\n\n" + Resource.Description;
        }

        // Base value
        text += $"\n\nBase Value: {BaseValue}";

        // Modifiers
        if (Modifiers.Count > 0) text += "\n";
        foreach (ProductionModifier modifier in GetOrderedModifiers())
        {
            text += $"\n{modifier.GetExplanationString()}";
        }

        // Final value
        text += $"\n\nFinal Value: {GetValue()}";

        return text;
    }

    /// <summary>
    /// Returns the modifiers to be ordered in a way, so all additive ones come first, and all multiplicative ones after.
    /// </summary>
    /// <returns></returns>
    private List<ProductionModifier> GetOrderedModifiers()
    {
        // First all additive modifiers (in original order), then all multiplicative (also in original order)
        var ordered = new List<ProductionModifier>(Modifiers.Count);

        // Add additive modifiers
        ordered.AddRange(Modifiers.FindAll(m => m.Type == ProductionModifierType.Additive));

        // Add multiplicative modifiers
        ordered.AddRange(Modifiers.FindAll(m => m.Type == ProductionModifierType.Multiplicative));

        return ordered;
    }
}
