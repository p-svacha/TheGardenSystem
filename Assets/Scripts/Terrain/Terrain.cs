using UnityEngine;

public class Terrain
{
    public MapTile Tile { get; private set; }
    public TerrainDef Def { get; private set; }

    public Terrain(MapTile tile, TerrainDef def)
    {
        Tile = tile;
        Def = def;
    }

    public virtual string Label => Def.Label;
    public virtual string LabelCap => Def.LabelCap;
    public virtual string Description => Def.Description;
    public virtual Sprite Sprite => Def.Sprite;
}
