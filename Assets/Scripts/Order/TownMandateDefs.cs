using System.Collections.Generic;
using UnityEngine;

public static class TownMandateDefs
{
    public static List<TownMandateDef> Defs => new List<TownMandateDef>()
    {
        // Jan
        new TownMandateDef()
        {
            DefName = "WinterSupplies",
            Label = "Winter Supplies",
            Description = "The town needs preserved goods and warmth for colder households.",
            OrderedResources = new(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Food, 15 },
                { ResourceDefOf.Kindle, 5 },
            }),
        },


        // Feb
        new TownMandateDef()
        {
            DefName = "SchoolLunch",
            Label = "School Lunches",
            Description = "The primary school kitchen is reopening. Simple meals for energetic kids.",
            OrderedResources = new(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Food, 35 },
                { ResourceDefOf.Herbs, 5 },
            }),
        },

        // Mar
        new TownMandateDef()
        {
            DefName = "SpringWellnessFestival",
            Label = "Spring Wellness Festival",
            Description = "Local apothecaries and herbalists host a market of healing and teas.",
            OrderedResources = new(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Herbs, 40 },
                { ResourceDefOf.Ornaments, 20 },
            }),
        },

        // Apr
        new TownMandateDef()
        {
            DefName = "TownBeautificationWeek",
            Label = "Town Beautification Week",
            Description = "Volunteers are decorating public spaces and planters across town.",
            OrderedResources = new(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Ornaments, 80 },
            }),
        },

        // May
        new TownMandateDef()
        {
            DefName = "FireFestival",
            Label = "Festival of Fire",
            Description = "An ancient bonfire festival honoring the changing season.",
            OrderedResources = new(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Kindle, 65 },
                { ResourceDefOf.Ornaments, 35 },
            }),
        },

        // Jun
        new TownMandateDef()
        {
            DefName = "SummerTradeCaravan",
            Label = "Summer Trade Caravan",
            Description = "The town is preparing a gift shipment for visiting merchants.",
            OrderedResources = new(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Food, 25 },
                { ResourceDefOf.Herbs, 25 },
                { ResourceDefOf.Ornaments, 25 },
                { ResourceDefOf.Kindle, 25 },
                { ResourceDefOf.Fiber, 25 },
            }),
        },

        // Jul
        new TownMandateDef()
        {
            DefName = "WeaversFair",
            Label = "Weaver's Fair",
            Description = "Tailors, spinners, and dyers from nearby towns gather to showcase work.",
            OrderedResources = new(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Fiber, 80 },
                { ResourceDefOf.Herbs, 40 },
                { ResourceDefOf.Ornaments, 20 },
            }),
        },

        // Aug
        new TownMandateDef()
        {
            DefName = "MidSummerBanquet",
            Label = "Mid-Summer Banquet",
            Description = "A grand feast in the central square. Public and guests welcome.",
            OrderedResources = new(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Food, 100 },
                { ResourceDefOf.Herbs, 30 },
                { ResourceDefOf.Ornaments, 30 },
            }),
        },

        // Sep
        new TownMandateDef()
        {
            DefName = "RainstockPrep",
            Label = "Rainstock Prep",
            Description = "Autumn rains are coming. Stockpiling waterproof goods and dry fuel.",
            OrderedResources = new(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Fiber, 70 },
                { ResourceDefOf.Kindle, 70 },
                { ResourceDefOf.Food, 40 },
            }),
        },

        // Oct
        new TownMandateDef()
        {
            DefName = "FragranceFestival",
            Label = "Festival of Fragrance",
            Description = "A town-wide parade of petals, perfumes, and herbal arrangements.",
            OrderedResources = new(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Ornaments, 100 },
                { ResourceDefOf.Herbs, 60 },
                { ResourceDefOf.Food, 40 },
            }),
        },

        // Nov
        new TownMandateDef()
        {
            DefName = "HarvestExchange",
            Label = "Harvest Exchange",
            Description = "The annual harvest fair allows townsfolk to trade surplus goods freely.",
            OrderedResources = new(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Food, 70 },
                { ResourceDefOf.Fiber, 60 },
                { ResourceDefOf.Kindle, 60 },
                { ResourceDefOf.Ornaments, 30 },
            }),
        },

        // Dec
        new TownMandateDef()
        {
            DefName = "WinterCrownCeremony",
            Label = "Winter Crown Ceremony",
            Description = "A ceremonial offering for the town's oldest tradition.",
            OrderedResources = new(new Dictionary<ResourceDef, int>()
            {
                { ResourceDefOf.Food, 85 },
                { ResourceDefOf.Ornaments, 65 },
                { ResourceDefOf.Herbs, 55 },
                { ResourceDefOf.Kindle, 35 },
            }),
        },
    };
}
