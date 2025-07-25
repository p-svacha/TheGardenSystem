using UnityEngine;
using TMPro;

public class UI_MandateWindow : UI_WindowBase
{
    public static UI_MandateWindow Instance;
    private static int NUM_MANDATES_PER_COLUMN = 6;

    [Header("Elements")]
    public GameObject MandateContainer;
    public TextMeshProUGUI RemainingResourcesText;

    [Header("Prefabs")]
    public GameObject ColumnPrefab;
    public UI_MandateWindowRow MandatePrefab;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        ConfirmButton.onClick.AddListener(Close);
    }

    public void Show()
    {
        gameObject.SetActive(true);

        // Mandates
        HelperFunctions.DestroyAllChildredImmediately(MandateContainer);
        int index = 0;
        GameObject column = null;
        foreach (TownMandate mandate in Game.Instance.TownMandates)
        {
            if (index % NUM_MANDATES_PER_COLUMN == 0)
            {
                column = GameObject.Instantiate(ColumnPrefab, MandateContainer.transform);
            }
            UI_MandateWindowRow elem = GameObject.Instantiate(MandatePrefab, column.transform);
            elem.Init(mandate);
            index++;
        }

        // Resources
        ResourceCollection remainingRes = new ResourceCollection();
        for (int i = Game.Instance.Month; i < Game.Instance.TownMandates.Count ; i++)
        {
            remainingRes.AddResources(Game.Instance.TownMandates[i].OrderedResources);
        }
        RemainingResourcesText.text = remainingRes.GetAsSingleLinkedString();
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }
}
