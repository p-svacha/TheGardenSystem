using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UI_ShedCabinet : MonoBehaviour
{
    [Header("Elements")]
    public GameObject Container;

    [Header("Prefabs")]
    public UI_CabinetShelf ShelfPrefab;

    public void Init(List<Object> objects)
    {
        HelperFunctions.DestroyAllChildredImmediately(Container, skipElements: 1);

        int numObjects = objects.Count;
        int objPerShelf = Game.Instance.GetShedCabinetShelfObjectAmount();
        int numShelves = Game.Instance.GetShedCabinetShelfAmount();

        int objIndex = 0;
        for (int i = 0; i < numShelves; i++)
        {
            UI_CabinetShelf shelf = GameObject.Instantiate(ShelfPrefab, Container.transform);
            Sprite shelfSprite = i == numShelves - 1 ? ResourceManager.LoadSprite("Sprites/UI/Shed/ShelfSegmentBottom") : ResourceManager.LoadSprite("Sprites/UI/Shed/ShelfSegment");
            shelf.Init(background: shelfSprite);

            for (int j = 0; j < objPerShelf; j++)
            {
                if (objIndex >= numObjects) continue;
                shelf.AddObject(objects[objIndex]);
                objIndex++;
            }
        }
    }
}
