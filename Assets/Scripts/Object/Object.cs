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

    /// <summary>
    /// The tile this object is currently on. Can be null.
    /// </summary>
    public MapTile Tile { get; private set; }

    /// <summary>
    /// The sector this object belongs to.
    /// </summary>
    public GardenSector Sector { get; private set; }

    /// <summary>
    /// Flag if this object is currently in the shed. Used for animations and shed display.
    /// </summary>
    public bool IsInShed;

    public Object(ObjectDef def)
    {
        Def = def;
        Modifiers = new List<Modifier>();
        IsInShed = true;
    }

    public void SetTile(MapTile tile)
    {
        Tile = tile;
    }

    public void SetSector(GardenSector sector)
    {
        Sector = sector;
    }

    #region Modifiers

    public void ApplyModifier(Modifier modifier)
    {
        Debug.Log($"Applying modifier {modifier.Def.DefName} to object {LabelCapWord} on {Tile.Coordinates} with a duration of {modifier.RemainingDuration}.");

        if (!modifier.Def.IsStackable && HasModifier(modifier.Def))
        {
            Modifier existingModifier = Modifiers.First(m => m.Def == modifier.Def);
            if (existingModifier.IsInfinite) return;
            if (modifier.IsInfinite) existingModifier.MakeInfinite();
            else if (modifier.RemainingDuration > existingModifier.RemainingDuration) existingModifier.SetDuration(modifier.RemainingDuration);
        }

        else Modifiers.Add(modifier);
    }

    public void RemoveExpiredModifiers()
    {
        Modifiers = Modifiers.Where(m => !m.IsExpired).ToList();
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
                string day = modifier.RemainingDuration == 1 ? "day" : "days";
                string duration = modifier.IsInfinite ? "" : $" ({modifier.RemainingDuration} {day} remaining)";
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
