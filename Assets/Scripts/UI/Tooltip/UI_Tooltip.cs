using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Actual tooltip window that gets spawned and managed by the TooltipManager.
/// IMPORTANT: UI Element holding this script must have:
///   > Anchor and Pivot at 0/0
///   > A NestedTooltipTextEventHandler script attached to the BodyText
/// </summary>
public class UI_Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image OuterFrame;
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI BodyText;

    public ITooltipTarget Target;
    public Vector3 MousePosition { get; private set; }
    public bool IsPositioned { get; set; }
    public float CreatedAt { get; private set; }
    public bool IsHovered { get; private set; }

    /// <summary>
    /// Gets called once from the NestedTooltipManager to set the text and listeners.
    /// <br/>Positioning will be done 1 frame later by the manager.
    /// </summary>
    public void Init(string title, string bodyText)
    {
        TitleText.text = title;
        BodyText.text = bodyText;

        // Record creation time and mouse position
        MousePosition = Input.mousePosition;
        if (TooltipManager.SNAP_TO_PIXELS) MousePosition = new Vector3(Mathf.Round(MousePosition.x), Mathf.Round(MousePosition.y), MousePosition.z);
        CreatedAt = Time.time;

        // Set inactive. One frame later the NestedTooltipManager will correctly position and reenable this tooltip.
        IsPositioned = false;

        // Seems weird, but is necessary so width and height of the window get set properly
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
        if (TooltipManager.SNAP_TO_PIXELS)
        {
            GetComponent<ContentSizeFitter>().enabled = false;
            SnapSizeToWholePixels();
        }
        gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData e)
    {
        IsHovered = true;
    }

    public void OnPointerExit(PointerEventData e)
    {
        IsHovered = false;
    }


    private void SnapSizeToWholePixels()
    {
        var canvas = GetComponentInParent<Canvas>();
        if (!canvas) return;

        var rt = (RectTransform)transform;
        float sf = canvas.scaleFactor;

        // Snap size to whole screen pixels
        float w = Mathf.Round(rt.rect.width * sf) / sf;
        float h = Mathf.Round(rt.rect.height * sf) / sf;
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);

        // (Optional) if your frame Image uses 9-slice and isn't stretched via anchors,
        // also snap its rect:
        if (OuterFrame)
        {
            var frt = OuterFrame.rectTransform;
            frt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
            frt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
        }
    }
}
