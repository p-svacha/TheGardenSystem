using UnityEngine;
using UnityEngine.UI;

public class UI_DevModePanel : MonoBehaviour
{
    public static UI_DevModePanel Instance;

    [Header("Elements")]
    public Button AddObjectButton;

    public Image ChangeTerrainFrame;
    public Button ChangeTerrainButton;

    public bool IsChangeTerrainActive { get; private set; }

    private void Awake()
    {
        Instance = this;
        AddObjectButton.onClick.AddListener(AddObjectButton_OnClick);
        ChangeTerrainButton.onClick.AddListener(ChangeTerrainButton_OnClick);
    }

    private void AddObjectButton_OnClick()
    {
        UI_AddObjectWindow.Instance.Show();
    }

    private void ChangeTerrainButton_OnClick()
    {
        IsChangeTerrainActive = !IsChangeTerrainActive;
        ChangeTerrainFrame.enabled = IsChangeTerrainActive;
    }
}
