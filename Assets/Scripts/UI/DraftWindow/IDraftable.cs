using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface that all objects that are draftable must implement.
/// </summary>
public interface IDraftable
{
    public string DraftDisplay_Title { get; }
    public string DraftDisplay_Description { get; }
    public Sprite DraftDisplay_Sprite { get; }
}
