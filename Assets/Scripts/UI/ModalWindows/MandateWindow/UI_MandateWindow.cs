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
        ConfirmButton.SetOnClick(Close);
    }

    public void Show()
    {
        gameObject.SetActive(true);

        // Get current month
        int currentMonth = Game.Instance.Month;
        if (Game.Instance.IsLastDayOfMonth && Game.Instance.IsNight) currentMonth++;

        // Mandates
        HelperFunctions.DestroyAllChildredImmediately(MandateContainer);
        int index = 0;
        GameObject column = null;
        foreach (TownMandate mandate in Game.Instance.TownMandates)
        {
            bool isCompleted = mandate.MonthIndex < currentMonth;
            if (index % NUM_MANDATES_PER_COLUMN == 0)
            {
                column = GameObject.Instantiate(ColumnPrefab, MandateContainer.transform);
            }
            UI_MandateWindowRow elem = GameObject.Instantiate(MandatePrefab, column.transform);
            elem.Init(mandate, isCompleted);
            index++;
        }

        // Resources
        ResourceCollection remainingRes = new ResourceCollection();
        if (Game.Instance.IsLastDayOfMonth && Game.Instance.IsNight) currentMonth++;
        for (int i = currentMonth; i < Game.Instance.TownMandates.Count ; i++)
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
