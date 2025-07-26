using UnityEngine;
using UnityEngine.UI;

public class UI_AddObjectWindow : UI_WindowBase
{
    public static UI_AddObjectWindow Instance;

    [Header("Elements")]
    public GameObject Container;

    [Header("Prefabs")]
    public GameObject RowPrefab;
    public UI_AddObjectWindowElement ElementPrefab;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        ConfirmButton.onClick.AddListener(Close);
    }

    public void Show()
    {
        // Object list
        HelperFunctions.DestroyAllChildredImmediately(Container);
        int objPerRow = 10;
        int index = 0;
        GameObject row = null;
        foreach (ObjectDef def in DefDatabase<ObjectDef>.AllDefs)
        {
            if (index % objPerRow == 0)
            {
                row = GameObject.Instantiate(RowPrefab, Container.transform);
                HelperFunctions.DestroyAllChildredImmediately(row);
            }
            UI_AddObjectWindowElement elem = GameObject.Instantiate(ElementPrefab, row.transform);
            elem.Init(def);
            index++;
        }

        gameObject.SetActive(true);
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }
}
