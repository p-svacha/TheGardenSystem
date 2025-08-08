using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance;

    [Header("Elements")]
    public UI_DatePanel DatePanel;
    public UI_ResourcePanel ResourcePanel;
    public UI_OrderPanel OrderPanel;
    public UI_Toggle AcquireTilesToggle;
    public Button ShopButton;

    private void Awake()
    {
        Instance = this;
        _TMPResourceSpriteAsset = null;
    }

    private void OnEnable()
    {
        ShopButton.onClick.AddListener(ShopButton_OnClick);
    }

    private void ShopButton_OnClick()
    {
        if (UI_ShopWindow.Instance.gameObject.activeSelf) return;
        UI_ShopWindow.Instance.Show();
    }

    #region TMP Sprite Asset

    private static TMP_SpriteAsset _TMPResourceSpriteAsset;
    public static TMP_SpriteAsset TMPResourceSpriteAsset
    {
        get
        {
            if (_TMPResourceSpriteAsset == null) _TMPResourceSpriteAsset = GenerateSpriteAsset();
            return _TMPResourceSpriteAsset;
        }
    }

    private static TMP_SpriteAsset GenerateSpriteAsset()
    {
        Def[] defs = DefDatabase<ResourceDef>.AllDefs.ToArray();
        int targetSize = 22;
        FilterMode filterMode = FilterMode.Point; // No interpolation because pixel art
        return RuntimeSpriteAssetBuilder.BuildResourceSpriteAsset(defs, targetSize, filterMode);
    }

    #endregion
}
