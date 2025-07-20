using UnityEngine;

public enum ResourceType
{
    /// <summary>
    /// Market resources are material resources you accumulate. They can be ordered by customers, so you need to accumulate enough to deliver.
    /// </summary>
    MarketResource,

    /// <summary>
    /// Abstract resources are non-accumulative and non-material. They get applied directly towards something in the day they get produced.
    /// </summary>
    AbstractResource,
}
