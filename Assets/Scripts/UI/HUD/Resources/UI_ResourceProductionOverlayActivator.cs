using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attach this to an element to make it show the resource production overlay for a specific resource when hovered.
/// </summary>
public class UI_ResourceProductionOverlayActivator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ResourceDef Resource { get; private set; }
    public void Init(ResourceDef res)
    {
        Resource = res;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Game.Instance.GameState != GameState.Afternoon) return;
        UI_TileOverlayContainer.Instance.ShowResourceProductionOverlay(Resource);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UI_TileOverlayContainer.Instance.HideAllOverlays();
    }
}
