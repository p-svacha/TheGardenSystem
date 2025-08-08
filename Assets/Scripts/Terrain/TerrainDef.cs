using System.Collections.Generic;
using UnityEngine;

public class TerrainDef : Def, ITooltipTarget
{
    public override Sprite Sprite => ResourceManager.LoadSprite("Sprites/Terrain/" + DefName);

    /// <summary>
    /// The DefName of the terrain that this terrain transforms to when reaching a fertility of 10.
    /// <br/>May be empty if increased fertility doesn't affect this.
    /// </summary>
    public string BetterFertilityTerrainDefName { get; init; } = "";

    /// <summary>
    /// The DefName of the terrain that this terrain transforms to when reaching a fertility of -10.
    /// <br/>May be empty if decreased fertility doesn't affect this.
    /// </summary>
    public string WorseFertilityTerrainDefName { get; init; } = "";

    /// <summary>
    /// The effects of this terrain.
    /// </summary>
    public List<ObjectEffect> Effects { get; init; } = new();

    public TerrainDef NextFertilityTerrain { get; private set; }
    public TerrainDef PrevFertilityTerrain { get; private set; }

    public override void ResolveReferences()
    {
        foreach (ObjectEffect effect in Effects) effect.EffectSource = this;
        if (BetterFertilityTerrainDefName != "") NextFertilityTerrain = DefDatabase<TerrainDef>.GetNamed(BetterFertilityTerrainDefName);
        if (WorseFertilityTerrainDefName != "") PrevFertilityTerrain = DefDatabase<TerrainDef>.GetNamed(WorseFertilityTerrainDefName);
    }

    public override bool Validate()
    {
        foreach (ObjectEffect effect in Effects)
        {
            if (!effect.Validate(out string invalidReason)) ThrowValidationError($"TerrainDef {DefName} has an invalid Effect: {invalidReason}");
        }

        return true;
    }

    public bool IsAffectedByFertility => NextFertilityTerrain != null || PrevFertilityTerrain != null;

    #region ITooltipTarget

    public string GetTooltipTitle() => LabelCapWord;
    public string GetTooltipBodyText(List<ITooltipTarget> references)
    {
        string desc = Description;

        if (Effects.Count > 0)
        {
            desc += "\n";
            foreach (ObjectEffect effect in Effects) desc += "\n"+effect.GetDescription();
        }

        if (IsAffectedByFertility) desc += "\n";
        if (NextFertilityTerrain != null) desc += $"\nTurns into {NextFertilityTerrain.GetTooltipLink()} at 10 Fertility.";
        if (PrevFertilityTerrain != null) desc += $"\nTurns into {PrevFertilityTerrain.GetTooltipLink()} at -10 Fertility.";

        return desc;
    }

    public string NestedTooltipLinkId => $"TerrainDef_{DefName}";
    public string NestedTooltipLinkText => LabelCapWord;
    public Color NestedTooltipLinkColor => TooltipManager.DEFAULT_NESTED_LINK_COLOR;

    #endregion
}
