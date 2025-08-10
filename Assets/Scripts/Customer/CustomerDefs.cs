using System.Collections.Generic;
using UnityEngine;

public static class CustomerDefs
{
    public static List<CustomerDef> Defs => new List<CustomerDef>()
    {
        new CustomerDef()
        {
            DefName = "TownCouncil",
            Label = "town council",
            Description = "The town council is your employer and therefore most important customer. They give you monthly mandates for resources they need for various reason.",
            Backstory = "The town council owns the garden you are managing and expects you to deliver monthly orders for various reasons. The council is operating town activities like festivals, lunches, parties and fairs, as well as managing the town stockpiles.",
            IsWeeklyCustomer = false,
        },

        new CustomerDef()
        {
            DefName = "TownCanteen",    
            Label = "town canteen",
            Description = "The local canteen serving affordable lunches to public workers. Will order food exclusively.",
            Backstory = "Nestled behind the town hall, the Town Canteen has been around for decades, feeding bus drivers, clerks, and school staff with no-frills, hearty meals. It’s publicly funded and always on the lookout for reliable suppliers who can offer basic produce in steady quantities. Your garden caught their eye thanks to your note on the community board.",
            GoldRewardPerLevel = 5,
            OrderIncreases = new Dictionary<int, ResourceCollection>()
            {
                { 1, new(new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 5 } }) },
                { 3, new(new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 15 } }) },
                { 7, new(new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 20 } }) },
                { 11, new(new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 30 } }) },
                { 16, new(new Dictionary<ResourceDef, int>() { { ResourceDefOf.Food, 40 } }) },
            },
        },

        new CustomerDef()
        {
            DefName = "LocalHerbalist",
            Label = "local herbalist",
            Description = "A small apothecary known for hand-crafted remedies and teas. Will order herbs exclusively.",
            Backstory = "Mira, the town herbalist, has run her shop for years, mixing poultices and calming infusions for anyone in need. Her shop smells like mint and mystery, and she’s always looking for a steady supply of quality herbs.",
            GoldRewardPerLevel = 5,
            OrderIncreases = new Dictionary<int, ResourceCollection>()
            {
                { 1, new(new Dictionary<ResourceDef, int>() { { ResourceDefOf.Herbs, 10 } }) },
                { 6, new(new Dictionary<ResourceDef, int>() { { ResourceDefOf.Herbs, 20 } }) },
                { 11, new(new Dictionary<ResourceDef, int>() { { ResourceDefOf.Herbs, 30 } }) },
                { 16, new(new Dictionary<ResourceDef, int>() { { ResourceDefOf.Herbs, 40 } }) },
            },
        },

        new CustomerDef()
        {
            DefName = "FestivalCommittee",
            Label = "festival committee",
            Description = "Requests beautiful decorations for town events and parades.",
            Backstory = "The town’s seasonal festival is a beloved tradition, and the committee is always preparing months in advance. They're happy to support your garden in exchange for fresh ornaments—flowers, petals, and other pleasant surprises.",
            GoldRewardPerLevel = 5,
            OrderIncreases = new Dictionary<int, ResourceCollection>()
            {
                { 1, new(new Dictionary<ResourceDef, int>() { { ResourceDefOf.Decorations, 10 } }) },
                { 6, new(new Dictionary<ResourceDef, int>() { { ResourceDefOf.Decorations, 10 }, { ResourceDefOf.Herbs, 10 } }) },
                { 11, new(new Dictionary<ResourceDef, int>() { { ResourceDefOf.Decorations, 15 }, { ResourceDefOf.Herbs, 15 } }) },
                { 16, new(new Dictionary<ResourceDef, int>() { { ResourceDefOf.Decorations, 20 }, { ResourceDefOf.Herbs, 20 } }) },
            },
        },

        new CustomerDef()
        {
            DefName = "StoveSmokeSupply",
            Label = "Stove & Smoke Supply Co.",
            Description = "Buys kindling in bulk for stoves and smokehouses.",
            Backstory = "A family-run supplier of firewood, smokehouse fuel, and oven starters. They’ve taken an interest in your garden’s renewable kindling output and want to see how reliable you can be.",
            GoldRewardPerLevel = 5,
            OrderIncreases = new Dictionary<int, ResourceCollection>()
            {
                { 1, new(new Dictionary<ResourceDef, int>() { { ResourceDefOf.Kindle, 10 } }) },
                { 6, new(new Dictionary<ResourceDef, int>() { { ResourceDefOf.Kindle, 20 } }) },
                { 11, new(new Dictionary<ResourceDef, int>() { { ResourceDefOf.Kindle, 30 } }) },
                { 16, new(new Dictionary<ResourceDef, int>() { { ResourceDefOf.Kindle, 40 } }) },
            },
        },
    };
}
