using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Object : INestedTooltipTarget
{
    public ObjectDef Def { get; private set; }

    /// <summary>
    /// The modifiers applied to this object, with the value being how many times a specific modifier has been applied.
    /// </summary>
    public Dictionary<ObjectModifierDef, int> Modifiers;

    public Object(ObjectDef def)
    {
        Def = def;
        Modifiers = new Dictionary<ObjectModifierDef, int>();
    }

    public void ApplyModifier(ObjectModifierDef def)
    {
        if (def.IsStackable) Modifiers.Increment(def);
        else if(!Modifiers.TryGetValue(def, out int value) || value < 1)
        {
            Modifiers.Increment(def);
        }
    }

    #region Getters

    /// <summary>
    /// The exact amount of resources of each type that this object produces natively.
    /// </summary>
    public virtual ResourceCollection GetNativeResourceProduction()
    {
        return new ResourceCollection(Def.NativeProduction);
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
        foreach (var kvp in Modifiers)
        {
            ObjectModifierDef modifier = kvp.Key;
            int numApplied = kvp.Value;
            for(int i = 0; i < numApplied; i++)
            {
                ObjectEffect modifierEffect = modifier.Effect.GetCopy();
                modifierEffect.EffectSource = modifier;
                effects.Add(modifierEffect);
            }
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


    #region INestedTooltipTaget

    public string GetTooltipTitle() => LabelCapWord;
    public string GetToolTipBodyText(out List<INestedTooltipTarget> dynamicReferences)
    {
        dynamicReferences = new List<INestedTooltipTarget>();

        // Tags
        string tags = "";
        foreach(ObjectTagDef tag in Tags)
        {
            tags += $"{tag.GetTooltipLink()} ";
        }
        tags = tags.TrimEnd(' ');

        // Native production
        string nativeProd = "";
        if (!GetNativeResourceProduction().IsEmpty)
        {
            nativeProd += $"\n\nNative Production: {GetNativeResourceProduction().GetAsSingleLinkedString()}";
        }

        // Effects
        string effectDescriptions = "";
        List<ObjectEffect> effects = GetNativeEffects();
        if (effects.Count > 0)
        {
            effectDescriptions += "\n";
            foreach (ObjectEffect effect in effects) effectDescriptions += "\n" + effect.GetDescription();
        }

        // Modifiers
        string modifiersDesc = "";
        if (Modifiers.Count > 0)
        {
            modifiersDesc += "\n\nModifiers:";
            foreach (var kvp in Modifiers)
            {
                string amount = kvp.Value > 1 ? $"{kvp.Value}x " : "";
                string mod = kvp.Key.LabelCapWord;
                string effect = kvp.Key.Effect.GetDescription();
                modifiersDesc += $"\n{amount}{mod}: {effect}";
            }
        }

        return $"{tags}\n\n<color=#999999>{Description}</color>{nativeProd}{effectDescriptions}{modifiersDesc}";
    }

    public string NestedTooltipLinkId => $"Object_{Def.DefName}";
    public string NestedTooltipLinkText => LabelCapWord;
    public Color NestedTooltipLinkColor => NestedTooltipManager.DEFAULT_NESTED_LINK_COLOR;

    #endregion
}
