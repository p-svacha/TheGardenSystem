using UnityEngine;

public class ScatteringObject : MonoBehaviour
{
    public Object Object { get; private set; }
    public MapTile SourceTile { get; private set; }
    public MapTile TargetTile { get; private set; }

    public bool IsDone { get; private set; }

    private float Timer;
    private Vector3 SourcePosition;
    private Vector3 TargetPosition;
    private float FlyingTime;

    public void Init(Object obj, MapTile sourceTile, MapTile targetTile)
    {
        IsDone = false;
        Object = obj;
        SourceTile = sourceTile;
        TargetTile = targetTile;

        SourcePosition = new Vector3(sourceTile.Coordinates.x + 0.5f, sourceTile.Coordinates.y + 0.5f, 0f);
        TargetPosition = new Vector3(targetTile.Coordinates.x + 0.5f, targetTile.Coordinates.y + 0.5f, 0f);
        float distance = Vector3.Distance(SourcePosition, TargetPosition);
        FlyingTime = distance / ScatterAnimationManager.OBJECT_FYLING_SPEED;
        Timer = 0f;

        transform.position = SourcePosition;

        // Sprite renderer
        SpriteRenderer sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = obj.Sprite;
        sr.sortingLayerName = "ScatteringObject";
    }

    public void UpdateAnimation(float deltaTime)
    {
        Timer += deltaTime;
        float delta = Timer / FlyingTime;
        if (delta >= 1f) OnArriveAtTarget();
        else transform.position = Vector3.Lerp(SourcePosition, TargetPosition, delta);
    }

    private void OnArriveAtTarget()
    {
        transform.position = TargetPosition;
        Game.Instance.OnObjectArrivedDuringScatter(Object, TargetTile);
        IsDone = true;
    }
}
