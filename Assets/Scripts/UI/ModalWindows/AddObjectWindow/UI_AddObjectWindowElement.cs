using UnityEngine;
using UnityEngine.UI;

public class UI_AddObjectWindowElement : MonoBehaviour
{
    [Header("Elements")]
    public Button Button;
    public Image Sprite;

    public void Init(ObjectDef def)
    {
        Sprite.sprite = def.Sprite;
        Button.onClick.AddListener(() => AddObject(def));
    }

    private void AddObject(ObjectDef def)
    {
        Debug.Log($"{def.DefName} was added to inventory through dev mode.");
        Game.Instance.AddObjectToInventory(def);
    }
}
