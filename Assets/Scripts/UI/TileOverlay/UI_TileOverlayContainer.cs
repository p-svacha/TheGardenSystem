using System.Collections.Generic;
using UnityEngine;

public class UI_TileOverlayContainer : MonoBehaviour
{
    public static UI_TileOverlayContainer Instance;

    [Header("Prefabs")]
    public UI_TileOverlay OverlayPrefab;

    private List<UI_TileOverlay> ActiveOverlays;

    private void Awake()
    {
        Instance = this;
        ActiveOverlays = new List<UI_TileOverlay>();
    }

    private void Start()
    {
        CameraHandler.Instance.OnCameraChanged += OnCameraChanged;
    }

    public void Clear()
    {
        HelperFunctions.DestroyAllChildredImmediately(gameObject);
        ActiveOverlays.Clear();
    }

    public void ShowTileCostOverlay()
    {
        Clear();
        foreach (MapTile tile in Game.Instance.Map.UnownedTiles)
        {
            bool canBuy = Game.Instance.Resources.HasResources(tile.AcquireCost);
            string textColorHex = canBuy ? ResourceManager.WhiteTextColorHex : ResourceManager.RedTextColorHex;
            ShowOverlay(tile, tile.AcquireCost.GetAsSingleLinkedString(textColorHex: textColorHex), showBackground: false);
        }
    }

    private void ShowOverlay(MapTile tile, string text, bool showBackground = true)
    {
        UI_TileOverlay overlay = GameObject.Instantiate(OverlayPrefab, transform);
        overlay.Init(tile, text, showBackground);
        ActiveOverlays.Add(overlay);
    }

    private void OnCameraChanged()
    {
        foreach (UI_TileOverlay overlay in ActiveOverlays) overlay.UpdateSizeAndPosition();
    }
}
