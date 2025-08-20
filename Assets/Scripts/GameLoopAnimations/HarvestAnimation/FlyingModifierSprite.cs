using UnityEngine;

public class FlyingModifierSprite : MonoBehaviour
{
    public Modifier Modifier { get; private set; }
    public MapTile SourceTile { get; private set; }
    public MapTile TargetTile { get; private set; }
    public bool IsDone { get; private set; }

    // Config
    public const float SPRITE_FLYING_SPEED = 0.5f; // 2.5   // world units per second (used when source != target)
    public const float SAME_TILE_DURATION = 1.0f;  // seconds (used when source == target)

    private float Timer;
    private float Duration;
    private SpriteRenderer Sr;

    private Vector3 SourcePos;
    private Vector3 TargetPos;

    // Visual params
    private const float StartScale = 0f;
    private const float EndScale = 1f;
    private const string SortingLayer = "ScatteringObject";

    /// <summary>
    /// Move from sourceTile to targetTile while scaling from 0 -> 1.
    /// If source == target, use SAME_TILE_DURATION and only grow in place.
    /// Otherwise, duration is distance / OBJECT_FLYING_SPEED.
    /// </summary>
    public void Init(Modifier modifier, MapTile sourceTile, MapTile targetTile)
    {
        IsDone = false;
        Modifier = modifier;
        SourceTile = sourceTile;
        TargetTile = targetTile;

        // Tile centers in world space
        SourcePos = new Vector3(sourceTile.Coordinates.x + 0.5f, sourceTile.Coordinates.y + 0.5f, 0f);
        TargetPos = new Vector3(targetTile.Coordinates.x + 0.5f, targetTile.Coordinates.y + 0.5f, 0f);

        float distance = Vector3.Distance(SourcePos, TargetPos);
        if (sourceTile == targetTile)
            Duration = Mathf.Max(0.0001f, SAME_TILE_DURATION);
        else
            Duration = Mathf.Max(0.0001f, distance / SPRITE_FLYING_SPEED);

        // Place & initialize visuals
        transform.position = SourcePos;
        transform.localScale = Vector3.one * StartScale;

        Sr = gameObject.AddComponent<SpriteRenderer>();
        Sr.sprite = modifier.Sprite;
        Sr.sortingLayerName = SortingLayer;

        var c = Sr.color; c.a = 1f; Sr.color = c;

        Timer = 0f;
    }

    /// <summary>Call each frame with Time.deltaTime.</summary>
    public void UpdateAnimation(float deltaTime)
    {
        if (IsDone) return;

        Timer += deltaTime;
        float t = Mathf.Clamp01(Timer / Duration);

        // Smoothstep for nicer motion/scale
        float e = t * t * (3f - 2f * t);

        // Move
        transform.position = Vector3.Lerp(SourcePos, TargetPos, e);

        // Scale 0 -> 1
        float scale = Mathf.Lerp(StartScale, EndScale, e);
        transform.localScale = Vector3.one * scale;

        if (t >= 1f)
        {
            Finish();
        }
    }

    private void Finish()
    {
        transform.position = TargetPos;
        transform.localScale = Vector3.one * EndScale;
        Game.Instance.ApplyModifier(Modifier);
        IsDone = true;
    }
}
