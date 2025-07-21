using System.Collections.Generic;
using UnityEngine;

public class TerrainDef : Def, INestedTooltipTarget
{
    new public Sprite Sprite => ResourceManager.LoadSprite("Sprites/Terrain/" + DefName);

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

    public TerrainDef NextFertilityTerrain { get; private set; }
    public TerrainDef PrevFertilityTerrain { get; private set; }

    public override void ResolveReferences()
    {
        if (BetterFertilityTerrainDefName != "") NextFertilityTerrain = DefDatabase<TerrainDef>.GetNamed(BetterFertilityTerrainDefName);
        if (WorseFertilityTerrainDefName != "") PrevFertilityTerrain = DefDatabase<TerrainDef>.GetNamed(WorseFertilityTerrainDefName);
    }
    
    public bool IsAffectedByFertility => NextFertilityTerrain != null || PrevFertilityTerrain != null;

    #region

    public string GetTooltipTitle() => LabelCapWord;
    public string GetToolTipBodyText(out List<INestedTooltipTarget> references)
    {
        references = new List<INestedTooltipTarget>();

        string desc = Description;

        if (IsAffectedByFertility) desc += "\n";
        if (NextFertilityTerrain != null) desc += $"\nTurns into {NextFertilityTerrain.GetNestedTooltipLink()} at 10 Fertility.";
        if (PrevFertilityTerrain != null) desc += $"\nTurns into {PrevFertilityTerrain.GetNestedTooltipLink()} at -10 Fertility.";

        return desc;
    }

    public string NestedTooltipLinkId => $"TerrainDef_{DefName}";
    public string NestedTooltipLinkText => LabelCapWord;
    public Color NestedTooltipLinkColor => NestedTooltipManager.DEFAULT_NESTED_LINK_COLOR;

    #endregion
}
