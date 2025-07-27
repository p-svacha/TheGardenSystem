using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_DevModePanel : MonoBehaviour
{
    public static UI_DevModePanel Instance;

    [Header("Elements")]
    public Button AddObjectButton;
    public Button AddResButton;

    public Image ChangeTerrainFrame;
    public Button ChangeTerrainButton;

    public Image ToggleTileOwnershipFrame;
    public Button ToggleTileOwnershipButton;

    public bool IsChangeTerrainActive { get; private set; }
    public bool IsToggleOwnershipActive { get; private set; }

    private void Awake()
    {
        Instance = this;
        AddObjectButton.onClick.AddListener(AddObjectButton_OnClick);
        AddResButton.onClick.AddListener(AddResButton_OnClick);
        ChangeTerrainButton.onClick.AddListener(ChangeTerrainButton_OnClick);
        ToggleTileOwnershipButton.onClick.AddListener(ToggleTileOwnershipButton_OnClick);
    }

    private void AddObjectButton_OnClick()
    {
        UI_AddObjectWindow.Instance.Show();
    }

    private void AddResButton_OnClick()
    {
        ResourceCollection resToAdd = new ResourceCollection();
        foreach(ResourceDef def in DefDatabase<ResourceDef>.AllDefs.Where(x => x.Type == ResourceType.Currency || x.Type == ResourceType.MarketResource))
        {
            resToAdd.AddResource(def, 10);
        }
        Game.Instance.AddResources(resToAdd);
    }

    private void ChangeTerrainButton_OnClick()
    {
        IsChangeTerrainActive = !IsChangeTerrainActive;
        ChangeTerrainFrame.enabled = IsChangeTerrainActive;
    }

    private void ToggleTileOwnershipButton_OnClick()
    {
        IsToggleOwnershipActive = !IsToggleOwnershipActive;
        ToggleTileOwnershipFrame.enabled = IsToggleOwnershipActive;
    }
}
