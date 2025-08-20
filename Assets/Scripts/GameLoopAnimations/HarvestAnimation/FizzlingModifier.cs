using UnityEngine;

public class FizzlingModifier : MonoBehaviour
{
    public Modifier Modifier { get; private set; }
    public bool IsDone { get; private set; }

    private float Timer;
    private float Duration;
    private SpriteRenderer Sr;

    // Visual params (tweak as desired)
    private const float StartScale = 1f;
    private const float EndScale = 1.6f; // quick expansion
    private const string SortingLayer = "ScatteringObject"; // or a dedicated FX layer

    /// <summary>
    /// worldPosition: where the modifier sprite should appear (center of its tile).
    /// duration: how long the fizz takes until fully faded (seconds).
    /// onFinished: callback when fade completes.
    /// </summary>
    public void Init(Modifier modifier, Vector3 worldPosition, float duration)
    {
        IsDone = false;
        Modifier = modifier;
        Duration = Mathf.Max(0.01f, duration);

        transform.position = worldPosition;
        transform.localScale = Vector3.one * StartScale;

        Sr = gameObject.AddComponent<SpriteRenderer>();
        Sr.sprite = modifier.Sprite;
        Sr.sortingLayerName = SortingLayer;

        // ensure full opacity at start
        var c = Sr.color; c.a = 1f; Sr.color = c;

        Timer = 0f;
    }

    /// <summary>Call this each frame with Time.deltaTime.</summary>
    public void UpdateAnimation(float deltaTime)
    {
        if (IsDone) return;

        Timer += deltaTime;
        float t = Mathf.Clamp01(Timer / Duration);

        // Smooth-ish easing for nicer feel: smoothstep
        float e = t * t * (3f - 2f * t);

        // Scale up
        float scale = Mathf.Lerp(StartScale, EndScale, e);
        transform.localScale = Vector3.one * scale;

        // Fade out
        var c = Sr.color;
        c.a = 1f - e;
        Sr.color = c;

        if (t >= 1f)
        {
            Finish();
        }
    }

    private void Finish()
    {
        IsDone = true;
    }
}
