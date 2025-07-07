using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TilemapTooltipController : MonoBehaviour
{
    [Header("Elements")]
    public Tilemap ObjectTilemap;
    public Camera UICamera;

    private const float HOVER_DELAY = 0.5f;

    private Vector3Int LastCell;
    private TileBase LastTile;
    private float HoverTimer;
    private bool TooltipVisible;

    void Update()
    {
        // 1) Compute which cell the mouse is over
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cell = ObjectTilemap.WorldToCell(worldPos);
        TileBase currentTile = ObjectTilemap.GetTile(cell);

        // 2) If we moved to a different cell or tile changed, reset timer & state
        if (cell != LastCell || currentTile != LastTile)
        {
            LastCell = cell;
            LastTile = currentTile;
            HoverTimer = 0f;
            TooltipVisible = false;
            if (!NestedTooltipManager.Instance.RootWindowIsPinned()) NestedTooltipManager.Instance.DestroyAllWindows();
        }

        // 3) If there's an object tile here, accumulate hover time
        if (LastTile != null && !NestedTooltipManager.Instance.RootWindowIsPinned())
        {
            HoverTimer += Time.deltaTime;
            if (HoverTimer >= HOVER_DELAY && !TooltipVisible)
            {
                TooltipVisible = true;
                ShowTileTooltip(cell);
            }
        }
    }

    private void ShowTileTooltip(Vector3Int cell)
    {
        // Look up the MapTile and its ObjectDef
        MapTile mapTile = Game.Instance.Map.GetTile(cell.x, cell.y);
        if (mapTile.Object == null) return;

        // Create a tiny RectTransform at the mouse's screen position for anchoring
        var anchorGO = new GameObject("TooltipAnchor", typeof(RectTransform));
        var fakeRect = anchorGO.GetComponent<RectTransform>();
        fakeRect.SetParent(UICamera.transform, false);
        fakeRect.position = Input.mousePosition;

        // Show the tooltip window
        NestedTooltipManager.Instance.ShowTooltip(
            mapTile.Object.LabelCap,
            mapTile.Object.GetTooltipDescription(),
            fakeRect,
            parent: null  // First‐level tooltip
        );

        // Clean up anchor
        Destroy(anchorGO);
    }
}
