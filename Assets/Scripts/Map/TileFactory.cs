using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Class responsible for creating Tile objects usable in Unity's tilemaps.
/// </summary>
public static class TileFactory
{
    /// <summary>
    /// Creates an AnimatedTile from a sprite sheet path under Resources.
    /// frameDurationSec = time per frame (e.g., 0.4f).
    /// </summary>
    public static AnimatedTile CreateAnimatedTileFromSpriteSheet(string resourcesPath, float frameDurationSec)
    {
        // Load all sliced sprites (expects sheet already sliced in Sprite Editor)
        var frames = Resources.LoadAll<Sprite>(resourcesPath);
        if (frames == null || frames.Length == 0)
            throw new System.Exception($"No sprites found at Resources/{resourcesPath}");

        // Ensure deterministic order: by rect.x, then by name as fallback
        System.Array.Sort(frames, (a, b) =>
        {
            int cmp = a.rect.x.CompareTo(b.rect.x);
            return cmp != 0 ? cmp : string.Compare(a.name, b.name, System.StringComparison.Ordinal);
        });

        var anim = ScriptableObject.CreateInstance<AnimatedTile>();
        anim.m_AnimatedSprites = frames;
        // AnimatedTile speed is cycles per second (full loop), so convert from per-frame duration:
        float cycleSeconds = frameDurationSec * frames.Length; // e.g., 0.4 * 4 = 1.6s
        float cyclesPerSecond = 1f / cycleSeconds;            // e.g., 0.625
        anim.m_MinSpeed = anim.m_MaxSpeed = cyclesPerSecond;
        anim.m_TileColliderType = Tile.ColliderType.None;
        return anim;
    }

    /// <summary>
    /// Creates a simple static Tile from a single sprite.
    /// </summary>
    public static Tile CreateTileFromSprite(Sprite sprite)
    {
        var tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = sprite;
        tile.colliderType = Tile.ColliderType.None;
        return tile;
    }

}
