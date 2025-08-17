using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ResourceRow : MonoBehaviour
{
    [Header("Elements")]
    public Image Icon;
    public TextMeshProUGUI ValueText;
    public TextMeshProUGUI PlusValueText;
    public UI_ResourceProductionOverlayActivator ProductionLensActivator;

    public void Init(ResourceDef res)
    {
        Icon.sprite = res.Sprite;
        ValueText.text = Game.Instance.Resources.Resources[res].ToString();

        if (Game.Instance.GameState == GameState.Noon || Game.Instance.GameState == GameState.Afternoon)
        {
            ResourceProduction production = Game.Instance.CurrentFinalResourceProduction[res];
            PlusValueText.text = $"(+{production.GetValue()})";
            PlusValueText.GetComponent<TooltipTarget_Reference>().Init(production);
        }
        else
        {
            PlusValueText.text = "";
            PlusValueText.GetComponent<TooltipTarget_Reference>().Disabled = true;
        }

        ProductionLensActivator.Init(res);

        // Tooltips
        Icon.GetComponent<TooltipTarget_Reference>().Init(res);
    }
}
