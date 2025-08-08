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
    [Tooltip("Default/idle sprite.")]
    [SerializeField] private Sprite defaultSprite;
    [Tooltip("Sprite while cursor is over the button.")]
    [SerializeField] private Sprite hoverSprite;
    [Tooltip("Sprite while primary mouse is held down.")]
    [SerializeField] private Sprite pressedSprite;

    [Header("References (auto-filled if left null)")]
    [SerializeField] private Image targetImage;
    [SerializeField] private Button button;
    [Tooltip("Child TMP text to nudge while pressed.")]
    [SerializeField] private RectTransform textRect;

    [Header("Pressed Text Offset")]
    [Tooltip("How far to push the text while pressed (pixels). Use negative Y to move down.")]
    [SerializeField] private Vector2 pressedTextOffset = new Vector2(0f, -2f);
    [SerializeField] private bool snapToIntegerPositions = true;

    private bool isPointerOver;
    private bool isPointerDown;
    private Vector2 originalTextPos;

    private void Reset()
    {
        targetImage = GetComponent<Image>();
        button = GetComponent<Button>();
        var tmp = GetComponentInChildren<TMP_Text>(true);
        if (tmp != null) textRect = tmp.rectTransform;
        if (targetImage != null && defaultSprite == null) defaultSprite = targetImage.sprite;
    }

    private void Awake()
    {
        defaultSprite = ResourceManager.LoadSprite("Sprites/UI/Button_Default");
        hoverSprite = ResourceManager.LoadSprite("Sprites/UI/Button_Hovered");
        pressedSprite = ResourceManager.LoadSprite("Sprites/UI/Button_Pressed");

        if (!targetImage) targetImage = GetComponent<Image>();
        if (!button) button = GetComponent<Button>();
        if (!textRect)
        {
            var tmp = GetComponentInChildren<TMP_Text>(true);
            if (tmp != null) textRect = tmp.rectTransform;
        }
        if (!defaultSprite && targetImage) defaultSprite = targetImage.sprite;

        // Avoid fighting with Button's internal transition.
        if (button && button.transition != Selectable.Transition.None)
            button.transition = Selectable.Transition.None;
    }

    private void OnEnable()
    {
        if (textRect) originalTextPos = textRect.anchoredPosition;
        SetSprite(defaultSprite);
        ResetTextPosition();
        isPointerOver = isPointerDown = false;
    }

    private void OnDisable()
    {
        SetSprite(defaultSprite);
        ResetTextPosition();
        isPointerOver = isPointerDown = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
        if (!isPointerDown) SetSprite(hoverSprite ? hoverSprite : defaultSprite);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
        if (!isPointerDown) SetSprite(defaultSprite);
        // While holding mouse down and leaving the button, we keep the pressed state,
        // since requirement says "while the mouse is pressed".
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (button && !button.interactable) return;

        isPointerDown = true;
        SetSprite(pressedSprite ? pressedSprite : defaultSprite);
        OffsetText();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        isPointerDown = false;
        ResetTextPosition();
        SetSprite(isPointerOver ? (hoverSprite ? hoverSprite : defaultSprite) : defaultSprite);
    }

    private void SetSprite(Sprite s)
    {
        if (!targetImage) return;
        targetImage.sprite = s ? s : defaultSprite;
        // Do NOT call SetNativeSize() here; it would fight layout/slicing.
    }

    private void OffsetText()
    {
        if (!textRect) return;
        var pos = originalTextPos + pressedTextOffset;
        if (snapToIntegerPositions) pos = new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
        textRect.anchoredPosition = pos;
    }

    private void ResetTextPosition()
    {
        if (!textRect) return;
        var pos = snapToIntegerPositions
            ? new Vector2(Mathf.Round(originalTextPos.x), Mathf.Round(originalTextPos.y))
            : originalTextPos;
        textRect.anchoredPosition = pos;
    }
}