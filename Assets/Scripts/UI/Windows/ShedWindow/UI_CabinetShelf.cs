using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_CabinetShelf : MonoBehaviour
{
    [Header("Elements")]
    public Image BackgroundImage;
    public GameObject ObjectContainer;

    [Header("Prefabs")]
    public UI_ShelfObject ShelfObjectPrefab;

    public void Init(Sprite background)
    {
        HelperFunctions.DestroyAllChildredImmediately(ObjectContainer);
        BackgroundImage.sprite = background;
    }

    public void AddObject(Object obj)
    {
        UI_ShelfObject elem = GameObject.Instantiate(ShelfObjectPrefab, ObjectContainer.transform);
        elem.Init(obj);
    }
}
