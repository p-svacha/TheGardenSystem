using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_WindowBase : MonoBehaviour
{
    [Header("Window Base")]
    public TextMeshProUGUI WindowTitle;
    public Button ConfirmButton;

    protected void Init(string title, System.Action confirmCallback)
    {
        WindowTitle.text = title;
        ConfirmButton.onClick.RemoveAllListeners();
        ConfirmButton.onClick.AddListener(confirmCallback.Invoke);
    }
}
