using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance;

    private static TMP_SpriteAsset _TMPResourceSpriteAsset;
    public static TMP_SpriteAsset TMPResourceSpriteAsset
    {
        get
        {
            if (_TMPResourceSpriteAsset == null) _TMPResourceSpriteAsset = RuntimeSpriteAssetBuilder.BuildResourceSpriteAsset();
            return _TMPResourceSpriteAsset;
        }
    }

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
}
