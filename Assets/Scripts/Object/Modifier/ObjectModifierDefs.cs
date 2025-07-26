using System.Collections.Generic;
using UnityEngine;

public static class ObjectModifierDefs
{
    public static List<ObjectModifierDef> Defs => new List<ObjectModifierDef>()
    {
        new ObjectModifierDef()
        {
            DefName = "VerdantlyIdolized",
            Label = "verdantly idolized",
            Description = "Blessed by the Verdant Idol. This object radiates fertile energy, enriching the soil beneath it.",
            Effect = new SelfEffect()
            {
                EffectOutcome = new EffectOutcome()
                {
                    ResourceProductionModifier = new Dictionary<ResourceDef, int>()
                    {
                        { ResourceDefOf.Fertility, 1 },
                    },
                }
            },
            IsStackable = false,
        },
    };
}
