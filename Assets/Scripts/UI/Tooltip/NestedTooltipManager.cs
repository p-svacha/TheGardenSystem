using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;

public class NestedTooltipManager : MonoBehaviour
{
    public static NestedTooltipManager Instance;
    [Tooltip("A tooltip prefab with TooltipWindow component")]
    public GameObject TooltipPrefab;

    // Stack of open windows in order of creation
    private readonly List<TooltipWindow> Windows = new List<TooltipWindow>();

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Call this when you hover a link or a UI element.
    /// </summary>
    public void ShowTooltip(string title, string body, RectTransform originRect, TooltipWindow parent = null)
    {
        // 1) If we already have deeper windows beyond parent, destroy them
        if (parent != null)
        {
            int idx = Windows.IndexOf(parent);
            DestroyWindowsFrom(idx + 1);
        }
        else
        {
            DestroyAllWindows();
        }

        // 2) Instantiate new window and init
        var go = Instantiate(TooltipPrefab, transform);
        var window = go.GetComponent<TooltipWindow>();
        window.Init(title, body, parent);
        Windows.Add(window);

        // 3) Position: world corners of origin, pick bottom-right or right edge
        Vector3[] corners = new Vector3[4];
        originRect.GetWorldCorners(corners);
        Vector3 anchorPos = corners[2]; // top-right
        // slight offset
        anchorPos += new Vector3(10f, -10f, 0f);
        go.GetComponent<RectTransform>().position = anchorPos;
    }

    /// <summary>
    /// Notify manager that a window gained/lost hover.
    /// </summary>
    public void NotifyWindowHover(TooltipWindow window, bool isHovering)
    {
        // If you exit the topmost window but still hover its parent, destroy only topmost
        if (!isHovering && window == Windows.Last())
        {
            DestroyWindowsFrom(Windows.Count - 1);
        }
    }

    public void DestroyAllWindows()
    {
        foreach (var w in Windows) Destroy(w.gameObject);
        Windows.Clear();
    }

    private void DestroyWindowsFrom(int startIndex)
    {
        for (int i = Windows.Count - 1; i >= startIndex; i--)
        {
            Destroy(Windows[i].gameObject);
            Windows.RemoveAt(i);
        }
    }

    public bool IsAnyWindowUnderPointer()
    {
        // Check if pointer is over *any* open TooltipWindow’s RectTransform
        return Windows.Any(w => RectTransformUtility.RectangleContainsScreenPoint(
            w.GetComponent<RectTransform>(),
            Input.mousePosition,
            w.GetComponentInParent<Canvas>().worldCamera
        ));
    }

    public bool RootWindowIsPinned()
    {
        return Windows.Count > 0 && Windows[0].IsPinned;
    }
}
