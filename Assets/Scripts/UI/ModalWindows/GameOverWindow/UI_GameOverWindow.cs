using UnityEngine;

public class UI_GameOverWindow : UI_WindowBase
{
    public static UI_GameOverWindow Instance;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    public void Show(string title)
    {
        WindowTitle.text = title;
        gameObject.SetActive(true);
    }
}
