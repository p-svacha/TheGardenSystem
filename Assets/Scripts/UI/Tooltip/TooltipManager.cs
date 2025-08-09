using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;

/// <summary>
/// Attach this to the container in the UI canvas that will hold all tooltips.
/// <br/>The TooltipManager handles the spawning and removing of all tooltips, including nested tooltips.
/// </summary>
public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;
    private bool DEBUG_ENABLED = false;

    private const float TOOLTIP_DELAY = 1f; // The time in seconds that something needs to get hovered to spawn a tooltip
    public static Color DEFAULT_NESTED_LINK_COLOR = new Color(0.624f, 0.478f, 0.325f);
    private const int MOUSE_OFFSET = 2; // px
    private const int SCREEN_EDGE_OFFSET = 5; // px

    /// <summary>
    /// If set to true, all tooltip windows will have position and size snapped to a full integer.
    /// </summary>
    public const bool SNAP_TO_PIXELS = true;

    [Header("Prefabs")]
    public UI_Tooltip TooltipPrefab;


    /// <summary>
    /// The ITooltipTarget that is currently being hovered, may be null
    /// </summary>
    private ITooltipTarget CurrentHoveredTarget;

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
    private readonly List<UI_Tooltip> Windows = new List<UI_Tooltip>();

    /// <summary>
    /// Dictionary holding information about which TMPro link id's belong to which ITooltipTargets.
    /// <br/>Targets referenced here are always the same and never change (usually Defs).
    /// </summary>
    private static Dictionary<string, ITooltipTarget> StaticLinkTargets;

    /// <summary>
    /// Dictionary holding information about which TMPro link id's, that are currently active in a shown tooltip, belong to which ITooltipTargets.
    /// <br/>Targets referenced here may be temporary objects that only exist in the current context.
    /// </summary>
    private static Dictionary<string, ITooltipTarget> DynamicLinkTargets;

    private void Awake()
    {
        Instance = this;
        HoverTimer = 0f;
        CurrentHoveredTarget = null;
    }

    private void Start()
    {
        InitStaticLinkTargets();
        DynamicLinkTargets = new Dictionary<string, ITooltipTarget>();
    }

    private void InitStaticLinkTargets()
    {
        StaticLinkTargets = new Dictionary<string, ITooltipTarget>();

        // ObjectDefs
        foreach (ObjectDef def in DefDatabase<ObjectDef>.AllDefs) StaticLinkTargets.Add(def.NestedTooltipLinkId, def);

        // ResourceDefs
        foreach (ResourceDef def in DefDatabase<ResourceDef>.AllDefs) StaticLinkTargets.Add(def.NestedTooltipLinkId, def);

        // ObjectTagDefs
        foreach (ObjectTagDef def in DefDatabase<ObjectTagDef>.AllDefs) StaticLinkTargets.Add(def.NestedTooltipLinkId, def);

        // ObjectModifierDefs
        foreach (ModifierDef def in DefDatabase<ModifierDef>.AllDefs) StaticLinkTargets.Add(def.NestedTooltipLinkId, def);

        // TerrainDefs
        foreach (TerrainDef def in DefDatabase<TerrainDef>.AllDefs) StaticLinkTargets.Add(def.NestedTooltipLinkId, def);
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

        // If neither the outermost tooltip nor the target creating it is hovered, destroy it
        if(Windows.Count > 0)
        {
            bool isOutermostWindowHovered = Windows.Last().IsHovered; // Tooltip window can only count as hovered when pinned
            bool isTargetSpawningOutermostWindowHovered = (CurrentHoveredTarget == Windows.Last().Target);
            if (!isOutermostWindowHovered && !isTargetSpawningOutermostWindowHovered) DestroyOutermostWindow();
        }
    }

    #region Positioning

    private void PositionTooltip(UI_Tooltip tooltip)
    {
        RectTransform rect = (RectTransform)tooltip.transform;
        Vector3 originalPosition = tooltip.MousePosition + new Vector3(MOUSE_OFFSET, MOUSE_OFFSET, 0);
        Vector2 adjustedPosition = new Vector2(originalPosition.x, originalPosition.y);

        // Fit on screen
        float scaleFactor = GetComponentInParent<Canvas>().scaleFactor;
        float tooltipWidth = rect.rect.width * scaleFactor;
        float tooltipHeight = rect.rect.height * scaleFactor;

        // If tooltip would go off the right edge, nudge left
        if (originalPosition.x + tooltipWidth > Screen.width - SCREEN_EDGE_OFFSET)
            adjustedPosition.x = Screen.width - tooltipWidth - SCREEN_EDGE_OFFSET;

        // If it would go off the top
        if (originalPosition.y + tooltipHeight > Screen.height - SCREEN_EDGE_OFFSET)
            adjustedPosition.y = Screen.height - tooltipHeight - SCREEN_EDGE_OFFSET;

        // Debug.Log($"Tooltip is {tooltipWidth}x{tooltipHeight} at position ({originalPosition.x},{originalPosition.y}), Screen is {Screen.width}x{Screen.height}. Adjusting position to ({adjustedPosition.x},{adjustedPosition.y}).");

        tooltip.transform.position = adjustedPosition;
        tooltip.IsPositioned = true;

        tooltip.gameObject.SetActive(true);
    }

    #endregion

    #region Internal Tooltip Handling

    private void ShowRootTooltip(ITooltipTarget target)
    {
        // Always destroy all previous tooltips since this a new root tooltip
        if(Windows.Count > 0) DestroyAllWindows();

        // Create new root tooltip
        ShowTooltip(target);
    }

    /// <summary>
    /// Shows a tooltip without destroying existing tooltips.
    /// </summary>
    private void ShowTooltip(ITooltipTarget target)
    {
        if (DEBUG_ENABLED) Debug.Log($"Spawning tooltip for {target.NestedTooltipLinkId}");

        // Create new tooltip
        UI_Tooltip tooltip = Instantiate(TooltipPrefab, transform);
        string titleText = target.GetTooltipTitle();
        List<ITooltipTarget> dynamicTooltipReferences = new List<ITooltipTarget>();
        string bodyText = target.GetTooltipBodyText(dynamicTooltipReferences);
        tooltip.Init(titleText, bodyText);
        tooltip.Target = target;
        Windows.Add(tooltip);

        // Register tooltip references
        foreach (ITooltipTarget referencedTarget in dynamicTooltipReferences)
        {
            string linkId = referencedTarget.NestedTooltipLinkId;
            if (DynamicLinkTargets.ContainsKey(linkId))
            {
                // Verify that existing link points to same object
                if (DynamicLinkTargets[linkId] != referencedTarget) throw new System.Exception($"The linkId {linkId} already exists in LinkTargets but points to a different object.\nIt points to {DynamicLinkTargets[linkId]} but now it should point to {referencedTarget}.");
                continue;
            }
            DynamicLinkTargets.Add(linkId, referencedTarget);
        }
    }

    public void DestroyAllWindows()
    {
        foreach (var w in Windows) Destroy(w.gameObject);
        Windows.Clear();
        DynamicLinkTargets.Clear();
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
    /// Gets called when a ITooltipTarget starts getting hovered on any GameObject (UI element, tilemap tile or 3D object).
    /// </summary>
    public void NotifyObjectHovered(ITooltipTarget target, bool isRoot = true, bool spawnsInstantTooltip = false)
    {
        if (target.GetTooltipTitle() == "" && target.GetTooltipBodyText(new()) == "") return;
        if (CurrentHoveredTarget != null)
        {
            if (DEBUG_ENABLED) Debug.LogWarning("This function may only be called if no object is currently hovered. Make sure to call NotifyObjectUnhovered() on the previous target first.");
            CurrentHoveredTarget = null;
        }
        if (DEBUG_ENABLED) Debug.Log("NotifyObjectHovered " + target.NestedTooltipLinkId);

        HoverTimer = 0f;
        CurrentHoveredTarget = target;
        IsCurrentHoveredTargetRoot = isRoot;
        if (spawnsInstantTooltip) HoverTimer = TOOLTIP_DELAY;
    }

    /// <summary>
    /// Gets called when a ITooltipTarget stops getting hovered on any GameObject (UI element, tilemap tile or 3D object).
    /// </summary>
    public void NotifyObjectUnhovered(ITooltipTarget target)
    {
        if (target != CurrentHoveredTarget) return;

        if (DEBUG_ENABLED) Debug.Log("NotifyObjectUnhovered " + target.NestedTooltipLinkId);
        CurrentHoveredTarget = null;
        HoverTimer = 0f;
    }

    public void NotifyTooltipLinkHovered(string linkId, bool isRoot)
    {
        if (DEBUG_ENABLED) Debug.Log("NotifyTooltipLinkHovered " + linkId);

        // Get tooltip target from link id
        if(!StaticLinkTargets.ContainsKey(linkId) && !DynamicLinkTargets.ContainsKey(linkId))
        {
            throw new System.Exception($"The provided linkId {linkId} is neither registered in StaticLinkTargets nor in DynamicLinkTargets. If it's a reference to a static object, such as a Def, make sure to register it in NestedTooltipManager.InitStaticLinkTargets(). If it's a reference to a dynamic object from another tooltip, make sure to add it to the out List references of that ITooltipTargets GetToolTipBodyText().\n\nStatic Links:\n{StaticLinkTargets.Keys.ToList().DebugList()}");
        }
        ITooltipTarget target = StaticLinkTargets.ContainsKey(linkId) ? StaticLinkTargets[linkId] : DynamicLinkTargets[linkId];

        // Redirect to default notify
        NotifyObjectHovered(target, isRoot);
    }

    public void NotifyTooltipLinkUnhovered(string linkId)
    {
        if (DEBUG_ENABLED) Debug.Log("NotifyTooltipLinkUnhovered " + linkId);

        // Get tooltip target from link id
        if (!StaticLinkTargets.ContainsKey(linkId) && !DynamicLinkTargets.ContainsKey(linkId))
        {
            throw new System.Exception($"The provided linkId {linkId} is neither registered in StaticLinkTargets nor in DynamicLinkTargets. If it's a reference to a static object, such as a Def, make sure to register it in NestedTooltipManager.InitStaticLinkTargets(). If it's a reference to a dynamic object from another tooltip, make sure to add it to the out List references of that ITooltipTargets GetToolTipBodyText().\n\nStatic Links:\n{StaticLinkTargets.Keys.ToList().DebugList()}");
        }
        ITooltipTarget target = StaticLinkTargets.ContainsKey(linkId) ? StaticLinkTargets[linkId] : DynamicLinkTargets[linkId];

        // Redirect to default notify
        NotifyObjectUnhovered(target);
    }

    #endregion
}
