using UnityEngine;

public class ObjectTierDef : Def
{
    /// <summary>
    /// The default amount of currency that objects of this tier cost when buying from the shop.
    /// </summary>
    public int MarketValue { get; init; }

    /// <summary>
    /// The color representing this tier.
    /// </summary>
    public Color Color { get; init; }
}
