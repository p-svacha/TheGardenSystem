using UnityEngine;
using TMPro;

public class UI_Order : MonoBehaviour
{
    [Header("Elements")]
    public TextMeshProUGUI CustomerText;
    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI ResourceText;
    public TextMeshProUGUI RewardText;
    public GameObject MissedOrdersContainer;

    [Header("Prefabs")]
    public GameObject MissedOrderPrefab;

    public void Init(Order order)
    {
        CustomerText.text = $"{order.Label}";
        LevelText.text = $"Level {order.Customer.OrderLevel}";
        ResourceText.text = order.OrderedResources.GetAsSingleLinkedString();
        RewardText.text = "| " + order.Reward.GetAsSingleLinkedString();

        // Missed orders
        int numMissedOrders = order.Customer.MissedOrders;
        HelperFunctions.DestroyAllChildredImmediately(MissedOrdersContainer);
        for (int i = 0; i < numMissedOrders; i++)
        {
            GameObject.Instantiate(MissedOrderPrefab, MissedOrdersContainer.transform);
        }
        MissedOrdersContainer.GetComponent<SimpleTooltipTarget>().Text = $"You have failed to deliver {numMissedOrders} orders in a row this customer. You will lose the game when failing to deliver {Game.CUSTOMER_ORDER_MISSES_IN_A_ROW_TO_LOSE_GAME} orders in a row.";
    }

    public void Init(TownMandate mandate)
    {
        CustomerText.text = $"{mandate.Def.Label}";
        LevelText.gameObject.SetActive(false);
        ResourceText.text = mandate.OrderedResources.GetAsSingleLinkedString();
        RewardText.text = "";
        MissedOrdersContainer.gameObject.SetActive(false);
    }
}
