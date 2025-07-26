using System.Collections.Generic;
using UnityEngine;

public class ModifierDef : Def, INestedTooltipTarget
{
    new public Sprite Sprite => ResourceManager.LoadSprite("Sprites/ObjectModifiers/" + DefName);

    /// <summary>
    /// The effect that may get applied to the object holding the modifier.
    /// </summary>
    public ObjectEffect Effect { get; init; }

    /// <summary>
    /// Flag if the same object can get this modifier multiple times.
    /// </summary>
    public bool IsStackable { get; init; }

    public override void ResolveReferences()
    {
        Effect.EffectSource = this;
    }

    public override bool Validate()
    {
        if (!Effect.Validate(out string invalidReason)) ThrowValidationError($"ObjectModifierDef {DefName} has an invalid Effect. Reason: {invalidReason}");

        return base.Validate();
    }


    #region INestedTooltipTarget

    public string GetTooltipTitle() => LabelCapWord;
    public string GetToolTipBodyText(out List<INestedTooltipTarget> references)
    {
        references = new List<INestedTooltipTarget>();

        string desc = Description;
        desc += $"\n\n{Effect.GetDescription()}";

        return desc;
    }

    public string NestedTooltipLinkId => $"ObjectModifierDef_{DefName}";
    public string NestedTooltipLinkText => LabelCapWord;
    public Color NestedTooltipLinkColor => NestedTooltipManager.DEFAULT_NESTED_LINK_COLOR;

    #endregion
}
