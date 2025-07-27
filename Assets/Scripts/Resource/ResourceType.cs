using UnityEngine;

public enum ResourceType
{
    /// <summary>
    /// Resource that is accumulative and is used for transactions, buying new tiles and objects. Customers will never order currency.
    /// </summary>
    Currency,

    /// <summary>
    /// Market resources are material resources you accumulate. They can be ordered by customers, so you need to accumulate enough to deliver.
    /// </summary>
    MarketResource,

    /// <summary>
    /// Abstract resources are non-accumulative and non-material. They get applied directly towards something in the day they get produced.
    /// </summary>
    AbstractResource,
}
