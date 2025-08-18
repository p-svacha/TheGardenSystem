using UnityEngine;
using UnityEngine.UI;

public class FlyingResourceIcon : MonoBehaviour
{
    public ResourceDef Resource { get; private set; }
    public MapTile SourceTile { get; private set; }
    public Vector3 SourcePosition { get; private set; }
    public Vector3 TargetPosition { get; private set; }
    public bool IsNegative { get; private set; }

    public bool IsDone { get; private set; }

    private float Timer;
    private float FlyingTime;

    public void Init(ResourceDef resource, MapTile sourceTile, bool isNegative)
    {
        IsDone = false;
        Resource = resource;
        SourceTile = sourceTile;
        IsNegative = isNegative;
        Vector3 sourceWorldPosition = new Vector3(sourceTile.Coordinates.x + 0.5f, sourceTile.Coordinates.y + 0.5f, 0f);
        SourcePosition = Camera.main.WorldToScreenPoint(sourceWorldPosition);
        if (resource.Type == ResourceType.TerrainModificationResource) TargetPosition = SourcePosition;
        else TargetPosition = GetTargetScreenPosition(resource);
        Debug.Log($"Target Position is {TargetPosition}");

        float distance = Vector3.Distance(SourcePosition, TargetPosition);
        FlyingTime = distance / HarvestAnimationManager.RESOURCE_ICON_FYLING_SPEED;
        Timer = 0f;

        transform.position = SourcePosition;

        // Renderer
        RectTransform rect = gameObject.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(44, 44);
        transform.position = SourcePosition;
        
        Image image = gameObject.AddComponent<Image>();
        image.sprite = resource.Sprite;        
    }

    private Vector3 GetTargetScreenPosition(ResourceDef resource)
    {
        return UI_HUD.Instance.ResourcePanel.GetScreenSpacePositionOfResourceIcon(resource);
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
        Game.Instance.OnResourceIconArrivesDuringHarvest(SourceTile, Resource, IsNegative);
        IsDone = true;
    }
}
