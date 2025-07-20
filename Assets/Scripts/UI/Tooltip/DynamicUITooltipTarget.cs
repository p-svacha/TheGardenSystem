using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach this script to a GameObject with a TMPro UI Text to automatically detect tooltip links in it and spawn tooltips for them.
/// </summary>
public class DynamicUITooltipTarget : MonoBehaviour
{
    /// <summary>
    /// Gets called once from the NestedTooltipManager to set the text and listeners.
    /// <br/>Positioning will be done 1 frame later by the manager.
    /// </summary>
    private void Awake()
    {
        // Hook link selection events once
        gameObject.TryGetComponent(out NestedTooltipTextEventHandler textEventHandler);
        if (textEventHandler == null) textEventHandler = gameObject.gameObject.AddComponent<NestedTooltipTextEventHandler>();
        textEventHandler.onLinkSelection.AddListener(OnLinkHovered);
        textEventHandler.onLinkUnhover.AddListener(OnLinkUnhovered);
    }

    private void OnLinkHovered(string linkId, string linkText, int linkIndex)
    {
        NestedTooltipManager.Instance.NotifyTooltipLinkHovered(linkId, isRoot: true);
    }

    private void OnLinkUnhovered(string linkId, string linkText, int linkIndex)
    {
        NestedTooltipManager.Instance.NotifyTooltipLinkUnhovered(linkId);
    }
}
