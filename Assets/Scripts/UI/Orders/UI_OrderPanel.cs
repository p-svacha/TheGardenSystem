using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_OrderPanel : MonoBehaviour
{
    [Header("Elements")]
    public TextMeshProUGUI DueResourcesText;

    public TextMeshProUGUI WeeklyDueText;
    public TextMeshProUGUI WeeklyResourcesText;
    public GameObject WeeklyContainer;

    public TextMeshProUGUI MandateDueText;
    public Button MandateButton;
    public GameObject MandateContainer;

    [Header("Prefabs")]
    public UI_Order OrderPrefab;

    private void Awake()
    {
        MandateButton.onClick.AddListener(ShowAllTownMandates);
    }

    public void Refresh()
    {
        int nextDueDay = Game.Instance.Day + (Game.DAYS_PER_WEEK - (Game.Instance.Day % Game.DAYS_PER_WEEK));
        if (Game.Instance.IsLastDayOfWeek)
        {
            nextDueDay = Game.Instance.Day;
            if (Game.Instance.GameState > GameState.ConfirmedScatter) nextDueDay += Game.DAYS_PER_WEEK;
        }


        // Due resources
        List<Order> dueWeeklyOrders = Game.Instance.ActiveOrders.Where(o => o.DueDay == nextDueDay).ToList();
        List<TownMandate> dueMandates = Game.Instance.TownMandates.Where(m => m.DueDay == nextDueDay).ToList();

        ResourceCollection dueResources = new ResourceCollection();
        foreach (Order dueOrder in dueWeeklyOrders) dueResources.AddResources(dueOrder.OrderedResources);
        foreach (TownMandate dueMandate in dueMandates) dueResources.AddResources(dueMandate.OrderedResources);
        DueResourcesText.text = dueResources.GetAsSingleLinkedString();

        // Weekly orders
        ResourceCollection weeklyRes = new ResourceCollection();
        foreach (Order order in dueWeeklyOrders) weeklyRes.AddResources(order.OrderedResources);
        WeeklyResourcesText.text = weeklyRes.GetAsSingleLinkedString();

        int daysToNextWeekly = nextDueDay - Game.Instance.Day + 1;
        WeeklyDueText.text = $"Due in {daysToNextWeekly} days";
        if (daysToNextWeekly == 1) WeeklyDueText.text = "Due today";

        HelperFunctions.DestroyAllChildredImmediately(WeeklyContainer, skipElements: 3);
        foreach (Order order in dueWeeklyOrders)
        {
            UI_Order elem = GameObject.Instantiate(OrderPrefab, WeeklyContainer.transform);
            elem.Init(order);
        }

        // Mandates
        int nextMandateDueDay = Game.Instance.NextTownMandate.DueDay;
        int daysToNextMandate = nextMandateDueDay - Game.Instance.Day + 1;
        MandateDueText.text = $"Due in {daysToNextMandate} days";
        if (daysToNextMandate == 1) MandateDueText.text = "Due today";

        HelperFunctions.DestroyAllChildredImmediately(MandateContainer, skipElements: 3);
        UI_Order mandate = GameObject.Instantiate(OrderPrefab, MandateContainer.transform);
        mandate.Init(Game.Instance.NextTownMandate);
    }

    private void ShowAllTownMandates()
    {
        UI_MandateWindow.Instance.Show();
    }
}
