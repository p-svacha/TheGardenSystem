using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_HUD : MonoBehaviour
{
    [Header("Elements")]
    public UI_DatePanel DatePanel;
    public UI_ResourcePanel ResourcePanel;
    public UI_OrderPanel OrderPanel;
    public UI_Button AcquireTilesControl;
    public UI_Button ShopControl;

    private void OnEnable()
    {
        ShopControl.SetOnClick(ShopButton_OnClick);
    }

    private void ShopButton_OnClick()
    {
        if (UI_ShopWindow.Instance.gameObject.activeSelf) return;
        UI_ShopWindow.Instance.Show();
    }
}
