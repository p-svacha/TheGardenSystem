using UnityEngine;

/// <summary>
/// A modifier that affects how much of a resource is produced in whatever context.
/// </summary>
public class ProductionModifier
{
    /// <summary>
    /// The string that is displayed as the source of where this modifier comes from.
    /// </summary>
    public INestedTooltipTarget Source { get; private set; }

    /// <summary>
    /// Type of how this modifier affects the production value (additive [+] or multiplicative [*%])
    /// </summary>
    public ProductionModifierType Type { get; private set; }

    /// <summary>
    /// The amount of how much the production value gets affected.
    /// </summary>
    public float TransformationValue { get; private set; }

    public ProductionModifier(INestedTooltipTarget source, ProductionModifierType type, float transformationValue)
    {
        Source = source;
        Type = type;
        TransformationValue = transformationValue;
    }

    /// <summary>
    /// Transforms the given value based on this modifiers attributes.
    /// </summary>
    public void TransformValue(ref float value)
    {
        if (Type == ProductionModifierType.Additive) value += TransformationValue;
        if (Type == ProductionModifierType.Multiplicative) value *= TransformationValue;
    }

    public string GetExplanationString()
    {
        string sign = "";
        string numberFormat = "";
        if (Type == ProductionModifierType.Additive)
        {
            if (TransformationValue > 0) sign = "+";
            else sign = "";
        }
        if (Type == ProductionModifierType.Multiplicative)
        {
            sign = "x";
            numberFormat = "P0";
        }
        return $"{Source.GetTooltipTitle()}: {sign}{TransformationValue.ToString(numberFormat)}";
    }

}
