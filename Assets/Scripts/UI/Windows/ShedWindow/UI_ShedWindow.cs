using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class UI_ShedWindow : UI_WindowBase
{
    public static UI_ShedWindow Instance;

    [Header("Elements")]
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI NumTilesText;
    public TextMeshProUGUI NumObjectsText;
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
        // Header
        TitleText.text = sector.Name;
        NumObjectsText.text = sector.NumObjects.ToString();
        NumTilesText.text = sector.NumTiles.ToString();
        
        // Objects
        List<Object> objectsToDisplay = new List<Object>(sector.Objects);

        // Only show remaining objects during scatter
        if(Game.Instance.GameState == GameState.Afternoon || Game.Instance.GameState == GameState.Evening)
        {
            objectsToDisplay = objectsToDisplay.Where(o => o.Tile == null).ToList();
        }

        gameObject.SetActive(true);
        Cabinet.Init(objectsToDisplay);
        DisplayedSector = sector;
    }

    public override void Hide()
    {
        base.Hide();
        DisplayedSector = null;
    }
}
