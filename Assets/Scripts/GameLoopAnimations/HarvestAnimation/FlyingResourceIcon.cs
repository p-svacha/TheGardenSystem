using UnityEngine;
using UnityEngine.UI;

public class FlyingResourceIcon : MonoBehaviour
{
    public const float ICON_SIZE = 44;

    public ResourceDef Resource { get; private set; }
    public MapTile SourceTile { get; private set; }
    public Vector3 SourcePosition { get; private set; }
    public Vector3 TargetPosition { get; private set; }
    public bool IsNegative { get; private set; }

    public bool IsDone { get; private set; }

    private float Timer;
    private float FlyingTime;


    // --- Hop (terrain modification) parameters ---
    // All units here are in *screen pixels* and *seconds* (since we animate in Screen Space - Overlay).
    private bool UseHopTrajectory;
    private float HopGravity;          // Negative value, pixels/s^2 (downwards).
    private float HopInitialVy;        // Initial upward velocity, pixels/s.
    private float HopDuration;         // Total time until we land again (s).
    private float HopApexHeight;       // Desired apex height above the source (pixels).

    // Cache for transform to avoid repeated property lookups.
    private RectTransform Rect;

    /// <summary>Initialize and start the animation.</summary>
    public void Init(ResourceDef resource, MapTile sourceTile, bool isNegative)
    {
        IsDone = false;
        Resource = resource;
        SourceTile = sourceTile;
        IsNegative = isNegative;

        // Add negative overlay
        if (IsNegative)
        {
            GameObject negativeOverlay = new GameObject();
            negativeOverlay.transform.SetParent(transform);
            Image negativeOverlayIcon = negativeOverlay.AddComponent<Image>();
            negativeOverlayIcon.sprite = ResourceManager.LoadSprite("Sprites/Resources/NegativeOverlay");

            RectTransform negRect = negativeOverlay.GetComponent<RectTransform>();
            if(negRect == null) negRect = negativeOverlay.AddComponent<RectTransform>();
            negRect.sizeDelta = new Vector2(ICON_SIZE, ICON_SIZE);
        }

        // Compute screen-space start (UI is Screen Space - Overlay)
        Vector3 sourceWorldPosition = new Vector3(sourceTile.Coordinates.x + 0.5f, sourceTile.Coordinates.y + 0.5f, 0f);
        SourcePosition = Camera.main.WorldToScreenPoint(sourceWorldPosition);

        // Decide animation type and target position
        if (resource.Type == ResourceType.TerrainModificationResource)
        {
            UseHopTrajectory = true;
            TargetPosition = SourcePosition; // lands where it started
        }
        else
        {
            UseHopTrajectory = false;
            TargetPosition = GetResourceIconScreenPosition(resource);
        }

        // Compute travel time for linear flight
        float distance = Vector3.Distance(SourcePosition, TargetPosition);
        FlyingTime = Mathf.Max(0.0001f, distance / HarvestAnimationManager.RESOURCE_ICON_FYLING_SPEED);
        Timer = 0f;

        // Create/ensure required UI components
        RectTransform existingRect = GetComponent<RectTransform>();
        Rect = existingRect != null ? existingRect : gameObject.AddComponent<RectTransform>();
        Rect.sizeDelta = new Vector2(ICON_SIZE, ICON_SIZE);

        transform.position = SourcePosition;

        Image image = GetComponent<Image>();
        if (image == null) image = gameObject.AddComponent<Image>();
        image.sprite = resource.Sprite;

        // Configure hop parameters if needed
        if (UseHopTrajectory)
        {
            // Reasonable defaults; tweak to taste or expose if needed.
            // Apex of ~120 px above start; gravity magnitude ~6000 px/s^2 gives a snappy hop.
            HopApexHeight = 120f;
            HopGravity = -6000f; // downwards acceleration

            // v0 = sqrt(2 * |g| * h)
            HopInitialVy = Mathf.Sqrt(2f * (-HopGravity) * HopApexHeight);

            // Total air time until we return to y0: T = 2 * v0 / |g|
            HopDuration = (2f * HopInitialVy) / (-HopGravity);

            // Reuse Timer as our time accumulator.
            Timer = 0f;
        }
    }

    private Vector3 GetResourceIconScreenPosition(ResourceDef resource)
    {
        return UI_HUD.Instance.ResourcePanel.GetScreenSpacePositionOfResourceIcon(resource);
    }

    public void UpdateAnimation(float deltaTime)
    {
        if (IsDone) return;
        Timer += deltaTime;

        if (UseHopTrajectory)
        {
            // Vertical hop in screen space. X/Z remain constant.
            // Kinematics: y(t) = y0 + v0*t + 0.5*a*t^2
            float t = Timer;

            // If we've exceeded the planned hop duration, snap to landing.
            if (t >= HopDuration)
            {
                Vector3 landed = SourcePosition;
                transform.position = landed;
                OnArriveAtTarget();
                return;
            }

            float dy = (HopInitialVy * t) + (0.5f * HopGravity * t * t);
            float currentY = SourcePosition.y + dy;

            // If we crossed below the source Y before HopDuration due to step size, clamp and finish.
            if (currentY <= SourcePosition.y && t > 0.0001f)
            {
                transform.position = SourcePosition;
                OnArriveAtTarget();
                return;
            }

            // Maintain the original screen X/Z; only Y changes.
            transform.position = new Vector3(SourcePosition.x, currentY, SourcePosition.z);
        }
        else
        {
            // Linear flight (UI space Lerp).
            float delta = Timer / FlyingTime;
            if (delta >= 1f)
            {
                OnArriveAtTarget();
            }
            else
            {
                transform.position = Vector3.Lerp(SourcePosition, TargetPosition, delta);
            }
        }
    }

    private void OnArriveAtTarget()
    {
        transform.position = TargetPosition;
        Game.Instance.OnResourceIconArrivesDuringHarvest(SourceTile, Resource, IsNegative);
        IsDone = true;
    }
}
