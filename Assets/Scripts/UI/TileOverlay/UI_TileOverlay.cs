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

    public void Init(MapTile tile)
    {
        Tile = tile;
        gameObject.SetActive(false);
    }

    public void Show(string text, bool showBackground = true)
    {
        Text.text = text;
        if (!showBackground)
        {
            if (OuterFrame.enabled) OuterFrame.enabled = false;
            if (InnerFrame.enabled) InnerFrame.enabled = false;
        }
        else
        {
            if (!OuterFrame.enabled) OuterFrame.enabled = true;
            if (!InnerFrame.enabled) InnerFrame.enabled = true;
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void UpdateSizeAndPosition()
    {
        transform.position = Camera.main.WorldToScreenPoint(new Vector3(Tile.Coordinates.x + 0.5f, Tile.Coordinates.y + 0.5f, 0));
        float scale = (4f / CameraHandler.Instance.ZoomLevel);
        transform.localScale = new Vector3(scale, scale, 1f);
    }
}
