using System.Collections.Generic;
using UnityEngine;

public class UI_TileOverlayContainer : MonoBehaviour
{
    public static UI_TileOverlayContainer Instance;

    [Header("Prefabs")]
    public UI_TileOverlay OverlayPrefab;

    private Dictionary<MapTile, UI_TileOverlay> Overlays;
    private List<MapTile> ActiveOverlays;

    private void Awake()
    {
        Instance = this;

        ActiveOverlays = new List<MapTile>();
    }

    private void Start()
    {
        CameraHandler.Instance.OnCameraChanged += OnCameraChanged;
    }

    public void InitOverlays()
    {
        // Create one overlay for each tile
        Overlays = new Dictionary<MapTile, UI_TileOverlay>();
        foreach(MapTile tile in Game.Instance.Map.AllTiles)
        {
            UI_TileOverlay overlay = GameObject.Instantiate(OverlayPrefab, transform);
            overlay.Init(tile);
            Overlays[tile] = overlay;
        }
    }

    public void HideAllOverlays()
    {
        foreach (MapTile tile in ActiveOverlays) Overlays[tile].Hide();
        ActiveOverlays.Clear();
    }

    public void ShowTileCostOverlay()
    {
        if(ActiveOverlays.Count > 0) HideAllOverlays();
        foreach (MapTile tile in Game.Instance.Map.UnownedTiles)
        {
            bool canBuy = Game.Instance.Resources.HasResources(tile.AcquireCost);
            string textColorHex = canBuy ? ResourceManager.WhiteTextColorHex : ResourceManager.RedTextColorHex;
            ShowOverlay(tile, tile.AcquireCost.GetAsSingleLinkedString(textColorHex: textColorHex), showBackground: false);
        }
        UpdateOverlays();
    }

    public void ShowResourceProductionOverlay(ResourceDef resource)
    {
        if (ActiveOverlays.Count > 0) HideAllOverlays();
        foreach (MapTile tile in Game.Instance.Map.OwnedTiles)
        {
            int production = Game.Instance.CurrentPerTileResourceProduction[tile][resource].GetValue();
            if (production != 0)
            {
                string text = $"{resource.GetTooltipLink()} {production}";
                ShowOverlay(tile, text, showBackground: true);
            }
        }
        UpdateOverlays();
    }

    private void ShowOverlay(MapTile tile, string text, bool showBackground = true)
    {
        Overlays[tile].Show(text, showBackground);
        ActiveOverlays.Add(tile);
    }

    private void OnCameraChanged()
    {
        UpdateOverlays();
    }

    /// <summary>
    /// Updates the size and position of all overlays based on camera position and zoom.
    /// </summary>
    private void UpdateOverlays()
    {
        foreach (MapTile tile in ActiveOverlays) Overlays[tile].UpdateSizeAndPosition();
    }
}
