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
    [SerializeField] private UI_Button GameLoopButton;

    private void OnEnable()
    {
        ShopControl.SetOnClick(ShopButton_OnClick);
        GameLoopButton.SetOnClick(GameLoopButton_OnClick);
    }

    private void ShopButton_OnClick()
    {
        if (UI_ShopWindow.Instance.gameObject.activeSelf) return;
        UI_ShopWindow.Instance.Show();
    }

    private void GameLoopButton_OnClick()
    {
        Game.Instance.AdvanceGameLoop();
    }

    public void RefreshGameLoopButton()
    {
        switch (Game.Instance.GameState)
        {
            case GameState.Morning:
                GameLoopButton.Show();
                GameLoopButton.SetText("Start Day");
                break;

            case GameState.Noon:
                GameLoopButton.Hide();
                break;

            case GameState.Afternoon:
                GameLoopButton.Show();
                GameLoopButton.SetText("Harvest");
                break;

            case GameState.Evening_HarvestAnimation:
                GameLoopButton.Hide();
                break;

            case GameState.Evening_PostHarvest:
                GameLoopButton.Show();
                if (Game.Instance.IsLastDayOfYear) GameLoopButton.SetText("End Year");
                else if (Game.Instance.IsLastDayOfMonth) GameLoopButton.SetText("End Month");
                else if (Game.Instance.IsLastDayOfWeek) GameLoopButton.SetText("End Week");
                else GameLoopButton.SetText("End Day");

                if (Game.Instance.IsShedWindowOpen) GameLoopButton.SetInteractable(false);
                else GameLoopButton.SetInteractable(true);
                break;

            case GameState.Night_ObjectDraft:
            case GameState.Night_OrderSelection:
                GameLoopButton.Hide();
                break;
        }
    }
}
