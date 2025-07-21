using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// IMPORTANT: UI Element holding this script must have:
///   > Anchor and Pivot at 0/0
///   > A NestedTooltipTextEventHandler script attached to the BodyText
/// </summary>
public class TooltipWindow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image OuterFrame;
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI BodyText;

    public INestedTooltipTarget Target;
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
        CreatedAt = Time.time;

        // Set inactive. One frame later the NestedTooltipManager will correctly position and reenable this tooltip.
        IsPositioned = false;

        // Seems weird, but is necessary so width and height of the window get set properly
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
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
}
