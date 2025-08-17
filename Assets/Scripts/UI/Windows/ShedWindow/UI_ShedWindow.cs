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

    public GardenSector DisplayedSector { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        CloseButton.SetOnClick(Close_OnClick);
    }

    private void Close_OnClick()
    {
        Game.Instance.CloseShed(DisplayedSector);
    }

    public void Show(GardenSector sector)
    {
        gameObject.SetActive(true);
        Cabinet.Init(sector.Objects);
        DisplayedSector = sector;
    }

    public override void Hide()
    {
        base.Hide();
        DisplayedSector = null;
    }
}
