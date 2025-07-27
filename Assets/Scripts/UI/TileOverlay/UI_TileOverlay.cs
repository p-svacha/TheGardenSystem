using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_TileOverlay : MonoBehaviour
{
    private MapTile Tile;

    [Header("Elements")]
    public Image OuterFrame;
    public Image InnerFrame;
    public TextMeshProUGUI Text;

    public void Init(MapTile tile, string text, bool showBackground = true)
    {
        Tile = tile;
        Text.text = text;
        if (!showBackground)
        {
            OuterFrame.enabled = false;
            InnerFrame.enabled = false;
        }
        UpdateSizeAndPosition();
    }

    public void UpdateSizeAndPosition()
    {
        transform.position = Camera.main.WorldToScreenPoint(new Vector3(Tile.Coordinates.x + 0.5f, Tile.Coordinates.y + 0.5f, 0));
        float scale = (4f / CameraHandler.Instance.ZoomLevel);
        transform.localScale = new Vector3(scale, scale, 1f);
    }
}
