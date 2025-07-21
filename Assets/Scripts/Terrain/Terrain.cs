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

        if (Fertility >= 10 && Def.NextFertilityTerrain != null) Game.Instance.SetTerrain(Tile.Coordinates, Def.NextFertilityTerrain);
        if (Fertility <= -10 && Def.PrevFertilityTerrain != null) Game.Instance.SetTerrain(Tile.Coordinates, Def.PrevFertilityTerrain);
    }

    public bool IsAffectedByFertility => Def.IsAffectedByFertility;

    public virtual string Label => Def.Label;
    public string LabelCap => Label.CapitalizeFirst();
    public string LabelCapWord => Label.CapitalizeEachWord();
    public virtual string Description => Def.Description;
    public virtual Sprite Sprite => Def.Sprite;

    public string GetDescriptionForTileTooltip()
    {
        string desc = "";

        if (IsAffectedByFertility) desc += $"Current Fertility: {Fertility}";

        return desc;
    }
}
