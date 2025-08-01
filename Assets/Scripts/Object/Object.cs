using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Object : ITooltipTarget
{
    public ObjectDef Def { get; private set; }

    /// <summary>
    /// The modifiers applied to this object.
    /// </summary>
    public List<Modifier> Modifiers { get; private set; }

    public Object(ObjectDef def)
    {
        Def = def;
        Modifiers = new List<Modifier>();
    }

    #region Modifiers

    public void ApplyModifier(ModifierDef def, int duration = -1)
    {
        Debug.Log($"Applying modifier {def.DefName} to {Def.DefName} with a duration of {duration}.");
        if (!def.IsStackable && HasModifier(def))
        {
            Modifier existingModifier = Modifiers.First(m => m.Def == def);
            if (existingModifier.IsInfinite) return;
            if (duration == -1) existingModifier.MakeInfinite();
            else if (duration > existingModifier.RemainingDuration) existingModifier.SetDuration(duration);
        }
        else Modifiers.Add(new Modifier(def, duration));
    }

    public void DecrementModifierDurations()
    {
        foreach (Modifier modifier in Modifiers) modifier.DecreaseDuration();
        Modifiers = Modifiers.Where(m => m.RemainingDuration == -1 || m.RemainingDuration > 0).ToList();
    }

    public bool HasModifier(ModifierDef def) => Modifiers.Any(m => m.Def == def);

    #endregion

    #region Getters

    /// <summary>
    /// The exact amount of resources of each type that this object produces natively.
    /// </summary>
    public virtual ResourceCollection GetNativeResourceProduction()
    {
        return new ResourceCollection(Def.GetNativeProduction());
    }

    /// <summary>
    /// The list of resources that this object produces natively.
    /// </summary>
    public List<ResourceDef> NativeResources => GetNativeResourceProduction().GetResourceList();

    public bool ProducesAnyOfResourcesNatively(List<ResourceDef> res) => NativeResources.Any(r => res.Contains(r));
    public bool ProducesAllOfResourcesNatively(List<ResourceDef> res) => NativeResources.All(r => res.Contains(r));

    public bool HasTag(ObjectTagDef tag) => Tags.Contains(tag);
    public bool HasAnyOfTags(List<ObjectTagDef> tags) => tags.Any(t => Tags.Contains(t));
    public bool HasAllTags(List<ObjectTagDef> tags) => tags.All(t => Tags.Contains(t));

    public virtual List<ObjectTagDef> Tags => Def.Tags;
    public virtual string Label => Def.Label;
    public string LabelCap => Def.LabelCap;
    public string LabelCapWord => Def.LabelCapWord;
    public virtual string Description => Def.Description;
    public virtual Sprite Sprite => Def.Sprite;

    public List<ObjectEffect> GetAllEffects()
    {
        List<ObjectEffect> effects = new List<ObjectEffect>();

        // Native effects from ObjectDef
        effects.AddRange(GetNativeEffects());

        // Effects from modifiers
        foreach (Modifier modifier in Modifiers)
        {
            ObjectEffect modifierEffect = modifier.Effect.GetCopy();
            modifierEffect.EffectSource = modifier.Def;
            effects.Add(modifierEffect);
        }

        return effects;
    }

    private List<ObjectEffect> GetNativeEffects()
    {
        List<ObjectEffect> effects = new List<ObjectEffect>();
        foreach (ObjectEffect effect in Def.Effects)
        {
            ObjectEffect instanceEffect = effect.GetCopy();
            instanceEffect.EffectSource = this;
            effects.Add(instanceEffect);
        }
        return effects;
    }

    #endregion


    #region ITooltipTaget

    public string GetTooltipTitle() => LabelCapWord;
    public string GetTooltipBodyText(List<ITooltipTarget> dynamicReferences)
    {
        string defDesc = Def.GetTooltipBodyText(dynamicReferences);

        // Modifiers
        string modifiersDesc = "";
        if (Modifiers.Count > 0)
        {
            modifiersDesc += "\n\nModifiers:";
            foreach (Modifier modifier in Modifiers)
            {
                string duration = modifier.IsInfinite ? "" : $" ({modifier.RemainingDuration} days remaining)";
                modifiersDesc += $"\n{modifier.Def.GetTooltipLink()}: {modifier.Effect.GetDescription()}{duration}";
            }
        }

        return $"{defDesc}{modifiersDesc}";
    }

    public string NestedTooltipLinkId => $"Object_{Def.DefName}";
    public string NestedTooltipLinkText => LabelCapWord;
    public Color NestedTooltipLinkColor => TooltipManager.DEFAULT_NESTED_LINK_COLOR;

    #endregion
}
