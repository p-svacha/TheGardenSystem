using UnityEngine;

public class UI_ResourcePanel : MonoBehaviour
{
    [Header("Elements")]
    public GameObject Container;

    [Header("Prefabs")]
    public UI_ResourceRow ResourceRowPrefab;

    public void Refresh()
    {
        HelperFunctions.DestroyAllChildredImmediately(Container, skipElements: 1);

        foreach(var res in Game.Instance.Resources.Resources)
        {
            UI_ResourceRow row = GameObject.Instantiate(ResourceRowPrefab, Container.transform);
            row.Init(res.Key);
        }
    }
}
