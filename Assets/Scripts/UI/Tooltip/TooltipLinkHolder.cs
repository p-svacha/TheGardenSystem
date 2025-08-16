using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Attach this script to a GameObject with a TMPro UI Text component to automatically detect tooltip links in it and spawn tooltips for them.
/// <br/>Specifically, this detects when "<link>" links in a TMPro UI Text are hovered and unhovered, and informs the TooltipManager.
/// <br/>Also necessary that custom sprites work since it attaches the correct sprite sheet to the text.
/// </summary>
public class TooltipLinkHolder : MonoBehaviour
{
    private TMP_Text m_TextComponent;

    private Camera m_Camera;
    private Canvas m_Canvas;

    private int m_selectedLink = -1;

    // Check this in the inspector if this element is already on a tooltip
    public bool IsOnTooltipWindow;

    /// <summary>
    /// Gets called once from the NestedTooltipManager to set the text and listeners.
    /// <br/>Positioning will be done 1 frame later by the manager.
    /// </summary>
    private void Awake()
    {
        // Get a reference to the text component.
        m_TextComponent = gameObject.GetComponent<TMP_Text>();

        // Get a reference to the camera rendering the text taking into consideration the text component type.
        if (m_TextComponent.GetType() == typeof(TextMeshProUGUI))
        {
            m_Canvas = gameObject.GetComponentInParent<Canvas>();
            if (m_Canvas != null)
            {
                if (m_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                    m_Camera = null;
                else
                    m_Camera = m_Canvas.worldCamera;
            }
        }
        else
        {
            m_Camera = Camera.main;
        }

        m_selectedLink = -1;
    }

    private void Start()
    {
        // Attach correct sprite sheet to Text Component
        gameObject.GetComponent<TextMeshProUGUI>().spriteAsset = GameUI.TMPResourceSpriteAsset;
    }

    private void OnLinkHovered(string linkId)
    {
        TooltipManager.Instance.NotifyTooltipLinkHovered(linkId, isRoot: !IsOnTooltipWindow);
    }

    private void OnLinkUnhovered(string linkId)
    {
        TooltipManager.Instance.NotifyTooltipLinkUnhovered(linkId);
    }

    private void LateUpdate()
    {
        if (TMP_TextUtilities.IsIntersectingRectTransform(m_TextComponent.rectTransform, Input.mousePosition, m_Camera))
        {
            // Check if mouse intersects with any links.
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(m_TextComponent, Input.mousePosition, m_Camera);

            // If we moved off a link, fire “unhover” for the old one
            if (linkIndex == -1 && m_selectedLink != -1)
            {
                var oldInfo = m_TextComponent.textInfo.linkInfo[m_selectedLink];
                OnLinkUnhovered(oldInfo.GetLinkID());
                m_selectedLink = -1;
            }

            // Handle new Link selection.
            if (linkIndex != -1 && linkIndex != m_selectedLink)
            {
                m_selectedLink = linkIndex;

                // If we were on a different link, unhover that first
                if (m_selectedLink != -1)
                {
                    var prev = m_TextComponent.textInfo.linkInfo[m_selectedLink];
                    OnLinkUnhovered(prev.GetLinkID());
                }
                m_selectedLink = linkIndex;

                // Get information about the link.
                TMP_LinkInfo linkInfo = m_TextComponent.textInfo.linkInfo[linkIndex];

                // Send the event to any listeners.
                OnLinkHovered(linkInfo.GetLinkID());
            }
        }
        else
        {
            // If we exit all text, unhover any outstanding link
            if (m_selectedLink != -1)
            {
                var exitInfo = m_TextComponent.textInfo.linkInfo[m_selectedLink];
                OnLinkUnhovered(exitInfo.GetLinkID());
            }

            // Reset all selections given we are hovering outside the text container bounds.
            m_selectedLink = -1;
        }
    }
}
