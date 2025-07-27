using UnityEngine;

public class UI_ResourcePanel : MonoBehaviour
{
    [Header("Elements")]
    public GameObject Container;

    [Header("Prefabs")]
    public UI_ResourceRow ResourceRowPrefab;
    public GameObject DelimiterPrefab;

    public void Refresh()
    {
        HelperFunctions.DestroyAllChildredImmediately(Container, skipElements: 1);

        int index = 0;
        foreach(ResourceDef res in Game.Instance.Resources.Resources.Keys)
        {
            UI_ResourceRow row = GameObject.Instantiate(ResourceRowPrefab, Container.transform);
            row.Init(res);
            if (index == 0) GameObject.Instantiate(DelimiterPrefab, Container.transform); // Delimiter after currency
            index++;
        }
    }
}
