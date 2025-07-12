using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TilemapTooltipController : MonoBehaviour
{
    [Header("Elements")]
    public Tilemap ObjectTilemap;
    public Camera UICamera;

    private MapTile LastTile;

    void Update()
    {
        if (HelperFunctions.IsMouseOverUi()) return;

        // 1) Compute which cell the mouse is over
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cell = ObjectTilemap.WorldToCell(worldPos);
        MapTile currentTile = Game.Instance.Map.GetTile(cell.x, cell.y);

        // 2) If we moved to a different cell or tile changed, reset timer & state
        if (currentTile != LastTile)
        {
            if (LastTile != null) NestedTooltipManager.Instance.NotifyObjectUnhovered(LastTile);
            if (currentTile != null) NestedTooltipManager.Instance.NotifyObjectHovered(currentTile);

            LastTile = currentTile;
        }
    }
}
