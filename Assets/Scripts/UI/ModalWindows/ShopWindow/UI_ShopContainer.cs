using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UI_ShopContainer : MonoBehaviour
{
    [Header("Elements")]
    public TextMeshProUGUI ResourceValueText;
    public GameObject ResourceContainer;
    public GameObject ObjectCategory;
    public GameObject ObjectContainer;

    [Header("Prefabs")]
    public UI_ShopElement ShopElementPrefab;

    private UI_ShopWindow ShopWindow;
    private UI_ShopContainer TargetContainer;

    public Dictionary<ResourceDef, int> Resources;
    private Dictionary<ResourceDef, UI_ShopElement> ResourceElements;

    public Dictionary<ObjectDef, UI_ShopElement> ObjectElements;
    private Dictionary<ObjectDef, int> Objects;

    public void Init(UI_ShopWindow shopWindow, string resourceValueText, Dictionary<ResourceDef, int> resources, bool showObjects, Dictionary<ObjectDef, int> objects, UI_ShopContainer targetContainer)
    {
        ShopWindow = shopWindow;
        TargetContainer = targetContainer;

        // Resources
        Resources = resources;
        ResourceElements = new Dictionary<ResourceDef, UI_ShopElement>();
        ResourceValueText.text = resourceValueText;
        HelperFunctions.DestroyAllChildredImmediately(ResourceContainer);
        foreach(var res in resources)
        {
            UI_ShopElement elem = GameObject.Instantiate(ShopElementPrefab, ResourceContainer.transform);
            elem.Init(res.Key, res.Value, () => ShopWindow.MoveResource(res.Key, 1, this, targetContainer));
            ResourceElements[res.Key] = elem;
        }

        // Objects
        Objects = objects;
        ObjectElements = new Dictionary<ObjectDef, UI_ShopElement>();
        ObjectCategory.SetActive(showObjects);
        if (showObjects)
        {
            HelperFunctions.DestroyAllChildredImmediately(ObjectContainer);
            foreach (var obj in objects)
            {
                UI_ShopElement elem = GameObject.Instantiate(ShopElementPrefab, ObjectContainer.transform);
                elem.Init(obj.Key, obj.Value, () => ShopWindow.MoveObject(obj.Key, obj.Value, this, targetContainer));
                ObjectElements[obj.Key] = elem;
            }
        }
    }


    public void AddResource(ResourceDef resource, int amount)
    {
        Resources[resource] += amount;
        RefreshResources();
    }
    public void RemoveResource(ResourceDef resource, int amount)
    {
        Resources[resource] -= amount;
        RefreshResources();
    }
    private void RefreshResources()
    {
        foreach (var res in Resources) ResourceElements[res.Key].SetText(Resources[res.Key].ToString());
    }

    public void AddObject(ObjectDef objectDef, int price)
    {
        Objects.Add(objectDef, price);
        UI_ShopElement elem = GameObject.Instantiate(ShopElementPrefab, ObjectContainer.transform);
        elem.Init(objectDef, price, () => ShopWindow.MoveObject(objectDef, price, this, TargetContainer));
        ObjectElements[objectDef] = elem;
    }
    public void RemoveObject(ObjectDef objectDef)
    {
        Objects.Remove(objectDef);
        GameObject.Destroy(ObjectElements[objectDef].gameObject);
        ObjectElements.Remove(objectDef);
    }
}
