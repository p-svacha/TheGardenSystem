using System.Collections.Generic;
using UnityEngine;

public class UI_ResourcePanel : MonoBehaviour
{
    [Header("Elements")]
    public GameObject Container;

    [Header("Prefabs")]
    public UI_ResourceRow ResourceRowPrefab;
    public GameObject DelimiterPrefab;

    private Dictionary<ResourceDef, UI_ResourceRow> ResourceRows;

    public void Refresh()
    {
        ResourceRows = new Dictionary<ResourceDef, UI_ResourceRow>();

        HelperFunctions.DestroyAllChildredImmediately(Container, skipElements: 1);

        int index = 0;
        foreach(ResourceDef res in Game.Instance.Resources.Resources.Keys)
        {
            UI_ResourceRow row = GameObject.Instantiate(ResourceRowPrefab, Container.transform);
            row.Init(res);
            ResourceRows.Add(res, row);
            if (index == 0) GameObject.Instantiate(DelimiterPrefab, Container.transform); // Delimiter after currency
            index++;
        }
    }

    public Vector3 GetScreenSpacePositionOfResourceIcon(ResourceDef res)
    {
        return ResourceRows[res].Icon.transform.position + new Vector3(22, 0, 0);
    }
}
