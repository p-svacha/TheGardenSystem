using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_WindowBase : MonoBehaviour
{
    [Header("Window Base")]
    public TextMeshProUGUI WindowTitle;
    public Button ConfirmButton;

    protected virtual void Awake()
    {
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        gameObject.SetActive(false);
    }

    protected void Init(string title, System.Action confirmCallback)
    {
        WindowTitle.text = title;
        ConfirmButton.onClick.RemoveAllListeners();
        ConfirmButton.onClick.AddListener(confirmCallback.Invoke);
    }
}
