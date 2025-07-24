using UnityEngine;

public class UI_GameOverWindow : UI_WindowBase
{
    public static UI_GameOverWindow Instance;

    private void Awake()
    {
        Instance = this;
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
