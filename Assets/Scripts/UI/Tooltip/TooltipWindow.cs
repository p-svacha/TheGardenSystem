using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TooltipWindow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image OuterFrame;
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI BodyText;
    [HideInInspector] public TooltipWindow ParentWindow;
    [HideInInspector] public bool IsHovered;

    public bool IsPinned { get; private set; }
    private float CreatedAt;
    private TMP_TextEventHandler TextEventHandler;

    private const float PIN_DELAY = 1.5f;

    public void Init(string title, string body, TooltipWindow parent = null)
    {
        ParentWindow = parent;
        TitleText.text = title;
        BodyText.text = body;

        // Record creation time
        CreatedAt = Time.time;
        IsPinned = false;
        OuterFrame.color = new Color(0.29f, 0.29f, 0.29f);  // Default grey

        // Hook link selection events once
        if (TextEventHandler == null)
        {
            TextEventHandler = BodyText.gameObject.AddComponent<TMP_TextEventHandler>();
            TextEventHandler.onLinkSelection.AddListener(OnLinkSelected);
        }
    }

    private void Update()
    {
        // Check pin delay
        if (!IsPinned && Time.time - CreatedAt >= PIN_DELAY)
        {
            IsPinned = true;
            OuterFrame.color = Color.yellow;
        }
    }

    private void OnLinkSelected(string linkID, string linkText, int linkIndex)
    {
        if (!IsPinned) return;

        // look up the def you linked to
        var def = GetDefFromLinkId(linkID);

        // anchor the new window to THIS window’s RectTransform
        var rt = GetComponent<RectTransform>();

        NestedTooltipManager.Instance.ShowTooltip(
            def.LabelCap,
            def.Description,
            rt,
            this   // parent = this window
        );
    }

    private Def GetDefFromLinkId(string linkID)
    {
        string[] linkParts = linkID.Split('_');
        string type = linkParts[0];
        string defName = linkParts[1];

        if(type == "tag")
        {
            return DefDatabase<ObjectTagDef>.GetNamed(defName);
        }

        throw new System.NotImplementedException();
    }

    public void OnPointerEnter(PointerEventData e)
    {
        IsHovered = true;
        NestedTooltipManager.Instance.NotifyWindowHover(this, true);
    }

    public void OnPointerExit(PointerEventData e)
    {
        IsHovered = false;
        NestedTooltipManager.Instance.NotifyWindowHover(this, false);
    }
}
