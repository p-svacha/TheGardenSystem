using UnityEngine;

public class TownMandate : Order
{
    public int MonthIndex { get; private set; }
    public TownMandateDef Def { get; private set; }

    public TownMandate(int month, TownMandateDef def)
    {
        Def = def;
        Customer = Game.Instance.TownCouncil;
        MonthIndex = month;
        DueDay = (month + 1) * Game.DAYS_PER_MONTH;
        OrderedResources = new ResourceCollection(def.OrderedResources);
    }

    public override string Label => "Town Mandate";
}
