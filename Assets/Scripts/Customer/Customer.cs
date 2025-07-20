using UnityEngine;

public class Customer
{
    public CustomerDef Def { get; private set; }
    public int OrderLevel { get; private set; }

    public Customer(CustomerDef def, int orderLevel)
    {
        Def = def;
        OrderLevel = orderLevel;
    }

    public void IncreaseLevel()
    {
        OrderLevel++;
    }

    public ResourceCollection GetCurrentLevelOrder() => new ResourceCollection(Def.Orders[OrderLevel - 1]);

    public string Label => Def.Label;
    public string LabelCap => Def.LabelCap;
    public string LabelCapWord => Def.LabelCapWord;
    public string Description => Def.Description;
    public string Backstory => Def.Backstory;
}
