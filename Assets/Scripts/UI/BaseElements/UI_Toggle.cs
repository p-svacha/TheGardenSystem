using UnityEngine;
using UnityEngine.UI;


public class UI_Toggle : MonoBehaviour
{
    [Header("Elements")]
    public Button Button;
    public Image OuterFrame;

    public bool IsToggled { get; private set; }

    public event System.Action<bool> OnToggle;

    private void Awake()
    {
        Button.onClick.AddListener(Toggle);
    }

    public void Toggle()
    {
        IsToggled = !IsToggled;
        Refresh();
        OnToggle?.Invoke(IsToggled);
    }

    public void SetState(bool value)
    {
        if (IsToggled == value) return;
        IsToggled = value;
        Refresh();
        OnToggle?.Invoke(IsToggled);
    }

    private void Refresh()
    {
        OuterFrame.color = IsToggled ? Color.red : new Color(0.337f, 0.349f, 0.376f);
    }

    public void Hide()
    {
        SetState(false);
        gameObject.SetActive(false);
    }
    public void Show() => gameObject.SetActive(true);
}
