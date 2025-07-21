using System.Collections.Generic;
using UnityEngine;

public class Terrain
{
    public MapTile Tile { get; private set; }
    public TerrainDef Def { get; private set; }
    public int Fertility { get; private set; }

    public Terrain(MapTile tile, TerrainDef def)
    {
        Tile = tile;
        Def = def;
        Fertility = 0;
    }

    public void AdjustFertility(int amount)
    {
        Fertility += amount;

        if (Fertility >= 10 && HasNextFertilityLevel) Game.Instance.SetTerrain(Tile.Coordinates, Def.NextFertilityTerrain);
        if (Fertility <= -10 && HasPrevFertilityLevel) Game.Instance.SetTerrain(Tile.Coordinates, Def.PrevFertilityTerrain);
    }

    public bool IsAffectedByFertility => Def.IsAffectedByFertility;
    public bool HasNextFertilityLevel => Def.NextFertilityTerrain != null;
    public bool HasPrevFertilityLevel => Def.PrevFertilityTerrain != null;

    public virtual string Label => Def.Label;
    public string LabelCap => Label.CapitalizeFirst();
    public string LabelCapWord => Label.CapitalizeEachWord();
    public virtual string Description => Def.Description;
    public virtual Sprite Sprite => Def.Sprite;
    public virtual List<ObjectEffect> Effects => Def.Effects;

    public string GetDescriptionForTileTooltip()
    {
        string desc = "";

        if (IsAffectedByFertility) desc += $"Current Fertility: {Fertility}";

        return desc;
    }
}
