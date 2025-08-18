using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance;

    [Header("Elements")]
    public GameObject FlyingResourceIconsContainer;

    private void Awake()
    {
        Instance = this;
        _TMPResourceSpriteAsset = null;
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
