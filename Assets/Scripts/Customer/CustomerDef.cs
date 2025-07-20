using System.Collections.Generic;
using UnityEngine;

public class CustomerDef : Def
{
    /// <summary>
    /// A very short, flavorful background of this customer.
    /// </summary>
    public string Backstory { get; init; }

    /// <summary>
    /// The resources that this customer orders, indexed by order level.
    /// </summary>
    public List<ResourceCollection> Orders { get; init; }
}
