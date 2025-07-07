using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class TooltipTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string Title;
    public string Text;

    public bool Disabled;

    [HideInInspector] public bool IsFocussed;
    private float Delay = 0.5f;
    [HideInInspector] public float CurrentDelay;

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsFocussed = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip();
        CurrentDelay = 0;
    }

    private void Update()
    {
        if (Disabled) return;
        if (IsFocussed)
        {
            if (CurrentDelay < Delay) CurrentDelay += UnityEngine.Time.deltaTime;
            else ShowTooltip();
        }
    }

    private void ShowTooltip()
    {
        var originRect = GetComponent<RectTransform>();
        NestedTooltipManager.Instance.ShowTooltip(
            Title,          // your tooltip title
            Text,           // your tooltip body (may contain <link=id>colored text</link>)
            originRect,     // so manager knows where to anchor the window
            null            // parent = null for first level
        );
    }

    public void HideTooltip()
    {
        NestedTooltipManager.Instance.DestroyAllWindows();
    }
}

