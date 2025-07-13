using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;

/// <summary>
/// Attach this to the container in the UI canvas that will hold all tooltips.
/// </summary>
public class NestedTooltipManager : MonoBehaviour
{
    public static NestedTooltipManager Instance;

    private const float TOOLTIP_DELAY = 1f; // The time in seconds that something needs to get hovered to spawn a tooltip
    private const float PIN_DELAY = 1.5f; // The time in seconds it takes for a tooltip to become pinned, allowing to hover into it and spawn nested tooltips
    public static Color DEFAULT_NESTED_LINK_COLOR = new Color(0.8f, 0.4f, 0f);
    private const int MOUSE_OFFSET = 2; // px
    private const int SCREEN_EDGE_OFFSET = 5; // px

    [Header("Tooltip frame colors")]
    public Color UnpinnedFrameColor;
    public Color PinnedFrameColor;

    [Header("Prefabs")]
    public TooltipWindow TooltipPrefab;


    /// <summary>
    /// The INestedTooltipTarget that is currently being hovered, may be null
    /// </summary>
    private INestedTooltipTarget CurrentHoveredTarget;

    /// <summary>
    /// Flag if CurrentHoveredTarget will spawn a new root tooltip if hovered long enough.
    /// <br/>If false, it will spawn a nested tooltip.
    /// </summary>
    private bool IsCurrentHoveredTargetRoot;

    /// <summary>
    /// How long the current hovered target has been hovered for.
    /// </summary>
    private float HoverTimer;

    // Stack of open windows in order of creation
    private readonly List<TooltipWindow> Windows = new List<TooltipWindow>();

    /// <summary>
    /// Dictionary holding information about which TMPro link id's belong to which INestedTooltipTargets.
    /// </summary>
    private static Dictionary<string, INestedTooltipTarget> LinkTargets;

    private void Awake()
    {
        Instance = this;
        LinkTargets = new Dictionary<string, INestedTooltipTarget>();
        HoverTimer = 0f;
        CurrentHoveredTarget = null;
    }

    private void Update()
    {
        // If the newest tooltip isn't positioned yet, do that.
        if (Windows.Count > 0 && !Windows.Last().IsPositioned) PositionTooltip(Windows.Last());

        // Accumulate hover time on current hovered target
        if (CurrentHoveredTarget != null)
        {
            bool doAccumulateTime = true;
            if (Windows.Count > 0 && Windows.Last().Target == CurrentHoveredTarget) doAccumulateTime = false; // Don't accumulate if target already has a tooltip
            if (Windows.Count > 0 && !Windows.Last().IsPinned) doAccumulateTime = false; // Don't accumulate time if outermost window isn't pinned yet
            if (doAccumulateTime)
            {
                HoverTimer += Time.deltaTime;
                if (HoverTimer >= TOOLTIP_DELAY)
                {
                    if (IsCurrentHoveredTargetRoot)
                    {
                         if (Windows.Count == 0) ShowRootTooltip(CurrentHoveredTarget); // Only show new root tooltip if no other tooltips are active
                    }
                    else ShowTooltip(CurrentHoveredTarget);
                }
            }
        }

        // Accumulate pin time on outermost tooltip
        if (Windows.Count > 0 && !Windows.Last().IsPinned)
        {
            if (Time.time - Windows.Last().CreatedAt >= PIN_DELAY)
            {
                PinTooltip(Windows.Last());
            }
        }

        // If neither the outermost tooltip nor the target creating it is hovered, destroy it
        if(Windows.Count > 0)
        {
            bool isOutermostWindowHovered = Windows.Last().IsHovered; // Tooltip window can only count as hovered when pinned
            bool isTargetSpawningOutermostWindowHovered = (CurrentHoveredTarget == Windows.Last().Target);
            if (!isOutermostWindowHovered && !isTargetSpawningOutermostWindowHovered) DestroyOutermostWindow();
        }
    }

    #region Positioning

    private void PositionTooltip(TooltipWindow tooltip)
    {
        RectTransform rect = tooltip.GetComponent<RectTransform>();
        Vector3 position = tooltip.MousePosition + new Vector3(MOUSE_OFFSET, MOUSE_OFFSET, 0);

        // Fit on screen
        float tooltipWidth = rect.rect.width;
        float tooltipHeight = rect.rect.height;

        // If tooltip would go off the right edge, nudge left
        if (position.x + tooltipWidth > Screen.width - SCREEN_EDGE_OFFSET)
            position.x = Screen.width - tooltipWidth - SCREEN_EDGE_OFFSET;

        // If it would go off the top
        if (position.y + tooltipHeight > Screen.height - SCREEN_EDGE_OFFSET)
            position.y = Screen.height - tooltipHeight - SCREEN_EDGE_OFFSET;

        tooltip.transform.position = position;
        tooltip.IsPositioned = true;

        tooltip.gameObject.SetActive(true);
    }

    #endregion

    #region Internal Tooltip Handling

    private void ShowRootTooltip(INestedTooltipTarget target)
    {
        // Always destroy all previous tooltips since this a new root tooltip
        if(Windows.Count > 0) DestroyAllWindows();

        // Create new root tooltip
        ShowTooltip(target);
    }

    /// <summary>
    /// Shows a tooltip without destroying existing tooltips.
    /// </summary>
    private void ShowTooltip(INestedTooltipTarget target)
    {
        // Create new tooltip
        TooltipWindow tooltip = Instantiate(TooltipPrefab, transform);
        tooltip.Init(target);
        tooltip.OuterFrame.color = UnpinnedFrameColor;
        Windows.Add(tooltip);

        // Register tooltip references
        foreach (INestedTooltipTarget referencedTarget in target.GetToolTipReferences())
        {
            string linkId = referencedTarget.NestedTooltipLinkId;
            if (LinkTargets.ContainsKey(linkId))
            {
                // Verify that existing link points to same object
                if (LinkTargets[linkId] != referencedTarget) throw new System.Exception($"The linkId {linkId} already exists in LinkTargets but points to a different object.\nIt points to {LinkTargets[linkId]} but now it should point to {referencedTarget}.");
                continue;
            }
            LinkTargets.Add(linkId, referencedTarget);
        }
    }

    private void PinTooltip(TooltipWindow tooltip)
    {
        tooltip.IsPinned = true;
        tooltip.OuterFrame.color = PinnedFrameColor;
    }

    public void DestroyAllWindows()
    {
        Debug.Log("DestroyAllWindows");
        foreach (var w in Windows) Destroy(w.gameObject);
        Windows.Clear();
        LinkTargets.Clear();
    }

    public void ResetTooltips()
    {
        DestroyAllWindows();
        HoverTimer = 0f;
    }

    private void DestroyOutermostWindow()
    {
        if (Windows.Count == 1)
        {
            DestroyAllWindows();
            return;
        }

        Destroy(Windows.Last().gameObject);
        Windows.RemoveAt(Windows.Count - 1);
    }

    #endregion

    #region Events

    /// <summary>
    /// Gets called when a INestedTooltipTarget starts getting hovered on any GameObject (UI element, tilemap tile or 3D object).
    /// </summary>
    public void NotifyObjectHovered(INestedTooltipTarget target, bool isRoot = true)
    {
        if (CurrentHoveredTarget != null)
        {
            Debug.LogWarning("This function may only be called if no object is currently hovered. Make sure to call NotifyObjectUnhovered() on the previous target first.");
            CurrentHoveredTarget = null;
        }
        Debug.Log("NotifyObjectHovered " + target.NestedTooltipLinkText);

        CurrentHoveredTarget = target;
        IsCurrentHoveredTargetRoot = isRoot;
    }

    /// <summary>
    /// Gets called when a INestedTooltipTarget stops getting hovered on any GameObject (UI element, tilemap tile or 3D object).
    /// </summary>
    public void NotifyObjectUnhovered(INestedTooltipTarget target)
    {
        CurrentHoveredTarget = null;
        HoverTimer = 0f;
    }

    public void NotifyTooltipLinkHovered(string linkId)
    {
        Debug.Log("NotifyTooltipLinkHovered " + linkId);

        // Get tooltip target from link id
        INestedTooltipTarget target = LinkTargets[linkId];

        // Redirect to default notify
        NotifyObjectHovered(target, isRoot: false);
    }

    public void NotifyTooltipLinkUnhovered(string linkId)
    {
        Debug.Log("NotifyTooltipLinkUnhovered " + linkId);

        // Get tooltip target from link id
        INestedTooltipTarget target = LinkTargets[linkId];

        // Redirect to default notify
        NotifyObjectUnhovered(target);
    }

    #endregion
}
