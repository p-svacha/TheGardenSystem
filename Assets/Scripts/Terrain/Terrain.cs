using System.Collections.Generic;
using UnityEngine;

public class Terrain : INestedTooltipTarget
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

    #region INestedTooltipTaget

    public string GetTooltipTitle() => LabelCap;
    public string GetToolTipBodyText(out List<INestedTooltipTarget> references)
    {
        references = new List<INestedTooltipTarget>();
        return Description;
    }

    public string NestedTooltipLinkId => $"Terrain_{Def.DefName}";
    public string NestedTooltipLinkText => LabelCap;
    public Color NestedTooltipLinkColor => NestedTooltipManager.DEFAULT_NESTED_LINK_COLOR;

    #endregion
}
