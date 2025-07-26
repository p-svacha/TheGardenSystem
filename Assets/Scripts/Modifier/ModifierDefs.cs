using System.Collections.Generic;
using UnityEngine;

public static class ModifierDefs
{
    public static List<ModifierDef> Defs => new List<ModifierDef>()
    {
        new ModifierDef()
        {
            DefName = "VerdantlyIdolized",
            Label = "verdantly idolized",
            Description = "Blessed by the Verdant Idol. This object radiates fertile energy, enriching the soil beneath it.",
            IsStackable = false,
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
        },

        new ModifierDef()
        {
            DefName = "Scorched",
            Label = "scorched",
            Description = "Was recently scorched by intense heat, reducing fertility.",
            IsStackable = false,
            Effect = new SelfEffect()
            {
                EffectOutcome = new EffectOutcome()
                {
                    ResourceProductionModifier = new Dictionary<ResourceDef, int>()
                    {
                        { ResourceDefOf.Fertility, -1 },
                    },
                }
            }
        },
    };
}
