using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UI_ShedWindow : UI_WindowBase
{
    public static UI_ShedWindow Instance;

    [Header("Elements")]
    public UI_Button CloseButton;
    public UI_ShedCabinet Cabinet;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        CloseButton.SetOnClick(Close_OnClick);
    }

    private void Close_OnClick()
    {
        gameObject.SetActive(false);
    }

    public void Show(GardenSector sector)
    {
        gameObject.SetActive(true);
        Cabinet.Init(sector.Objects);
    }
}
