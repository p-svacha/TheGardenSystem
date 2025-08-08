using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[DisallowMultipleComponent]
[RequireComponent(typeof(Image), typeof(Button))]
public class UI_Button : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [Header("Sprites")]
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite hoverSprite;
    [SerializeField] private Sprite pressedSprite;

    [Header("References (auto-filled if left null)")]
    [SerializeField] private Image targetImage;
    public Button Button;
    [SerializeField] private RectTransform textRect;
    [SerializeField] private RectTransform iconRect;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Pressed Offset")]
    [Tooltip("How far to push content while pressed (pixels). Use negative Y to move down.")]
    [SerializeField] private Vector2 pressedTextOffset = new Vector2(0f, -2f);
    [SerializeField] private bool snapToIntegerPositions = true;

    [Header("Toggle Mode")]
    [Tooltip("If true, clicking toggles the 'on' state and fires OnToggle.")]
    [SerializeField] private bool actsAsToggle = false;
    [Tooltip("Initial toggle state if Acts As Toggle is enabled.")]
    [SerializeField] private bool isToggled = false;
    [Tooltip("When toggled ON, use the pressed sprite as the base look.")]
    [SerializeField] private bool toggleUsesPressedLook = true;

    public event Action<bool> OnToggle;

    private bool isPointerOver;
    private bool isPointerDown;
    private bool pressedOffsetApplied;

    private Vector2 originalTextPos;
    private Vector2 originalIconPos;

    private void Reset()
    {
        targetImage = GetComponent<Image>();
        Button = GetComponent<Button>();
        TryAutoWireTMP();
        TryAutoWireIcon();
        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
        if (!canvasGroup) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        if (targetImage != null && defaultSprite == null) defaultSprite = targetImage.sprite;
    }

    private void Awake()
    {
        // Your sprite loading — keep as-is if you use a custom loader.
        defaultSprite = ResourceManager.LoadSprite("Sprites/UI/Button_Default");
        hoverSprite = ResourceManager.LoadSprite("Sprites/UI/Button_Hovered");
        pressedSprite = ResourceManager.LoadSprite("Sprites/UI/Button_Pressed");

        targetImage = GetComponent<Image>();
        Button = GetComponent<Button>();
        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
        if (!canvasGroup) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (!textRect) TryAutoWireTMP();
        if (!iconRect) TryAutoWireIcon();

        if (!defaultSprite && targetImage) defaultSprite = targetImage.sprite;

        // Avoid fighting with Button transitions.
        if (Button && Button.transition != Selectable.Transition.None)
            Button.transition = Selectable.Transition.None;
    }

    private void OnEnable()
    {
        CacheOriginalPositions();
        isPointerOver = isPointerDown = false;
        pressedOffsetApplied = false;
        RefreshVisual(); // will apply pressed offset if toggled-on
    }

    private void OnDisable()
    {
        isPointerOver = isPointerDown = false;
        ResetPositions();
        SetSprite(defaultSprite);
    }

    // ---------- Pointer Events ----------
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (Button && !Button.interactable) return;

        isPointerDown = true;
        RefreshVisual(forcePressed: true); // no direct OffsetPositions() here
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        isPointerDown = false;

        if (isPointerOver && (!Button || Button.interactable) && actsAsToggle)
            SetToggled(!isToggled, invokeEvent: true);

        RefreshVisual(); // no direct ResetPositions() here
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
        if (!isPointerDown) RefreshVisual();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
        if (!isPointerDown) RefreshVisual();
    }

    // ---------- Public API ----------
    public void Show()
    {
        if (!canvasGroup) return;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    public void Hide()
    {
        if (!canvasGroup) return;
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public void SetActsAsToggle(bool value)
    {
        actsAsToggle = value;
        RefreshVisual();
    }

    public void SetToggled(bool value, bool invokeEvent = false)
    {
        if (!actsAsToggle)
        {
            // Still allow explicit toggling to force a look if the caller insists.
            // But callers should enable actsAsToggle beforehand for clarity.
        }

        if (isToggled == value) { RefreshVisual(); return; }

        isToggled = value;
        RefreshVisual();

        if (invokeEvent)
            OnToggle?.Invoke(isToggled);
    }

    public bool GetToggled() => isToggled;

    // ---------- Internals ----------
    private void RefreshVisual(bool forcePressed = false)
    {
        bool showPressed =
            forcePressed ||
            isPointerDown ||
            (actsAsToggle && isToggled && toggleUsesPressedLook);

        if (showPressed)
            SetSprite(pressedSprite ? pressedSprite : defaultSprite);
        else if (isPointerOver)
            SetSprite(hoverSprite ? hoverSprite : defaultSprite);
        else
            SetSprite(defaultSprite);

        ApplyPressedOffset(showPressed);
    }

    private void SetSprite(Sprite s)
    {
        if (!targetImage) return;
        targetImage.sprite = s ? s : defaultSprite;
        // Intentionally do not call SetNativeSize() to preserve slicing/layout.
    }

    private void OffsetPositions()
    {
        Vector2 Offset(Vector2 basePos)
        {
            var p = basePos + pressedTextOffset;
            return snapToIntegerPositions ? new Vector2(Mathf.Round(p.x), Mathf.Round(p.y)) : p;
        }

        if (textRect) textRect.anchoredPosition = Offset(originalTextPos);
        if (iconRect) iconRect.anchoredPosition = Offset(originalIconPos);
    }

    private void ResetPositions()
    {
        Vector2 Snap(Vector2 p) => snapToIntegerPositions
            ? new Vector2(Mathf.Round(p.x), Mathf.Round(p.y))
            : p;

        if (textRect) textRect.anchoredPosition = Snap(originalTextPos);
        if (iconRect) iconRect.anchoredPosition = Snap(originalIconPos);
    }

    private void CacheOriginalPositions()
    {
        if (textRect) originalTextPos = textRect.anchoredPosition;
        if (iconRect) originalIconPos = iconRect.anchoredPosition;
    }

    private void TryAutoWireTMP()
    {
        var tmp = GetComponentInChildren<TMP_Text>(true);
        if (tmp) textRect = tmp.rectTransform;
    }

    private void TryAutoWireIcon()
    {
        // Find first Image in children that's NOT the root Image.
        var allImages = GetComponentsInChildren<Image>(true);
        foreach (var img in allImages)
        {
            if (img == null) continue;
            if (targetImage == null || img != targetImage)
            {
                iconRect = img.rectTransform;
                break;
            }
        }
    }

    private void ApplyPressedOffset(bool apply)
    {
        if (apply == pressedOffsetApplied) return;
        if (apply) OffsetPositions(); else ResetPositions();
        pressedOffsetApplied = apply;
    }
}
