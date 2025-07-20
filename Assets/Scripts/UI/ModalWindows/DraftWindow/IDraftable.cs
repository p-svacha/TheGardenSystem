using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface that all objects that are draftable must implement.
/// </summary>
public interface IDraftable
{
    public string DraftDisplay_Title { get; }
    public Sprite DraftDisplay_Sprite { get; }
    public string DraftDisplay_DescriptionPre { get; }
    public string DraftDisplay_DescriptionMain { get; }
    public string DraftDisplay_DescriptionPost { get; }

}
