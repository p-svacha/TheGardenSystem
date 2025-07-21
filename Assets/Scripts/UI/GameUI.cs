using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance;
    public static TMP_SpriteAsset TMPResourceSpriteAsset;

    [Header("Elements")]
    public UI_DatePanel DatePanel;
    public UI_ResourcePanel ResourcePanel;
    public UI_OrderPanel OrderPanel;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        TMPResourceSpriteAsset = RuntimeSpriteAssetBuilder.BuildResourceSpriteAsset();
    }
}
