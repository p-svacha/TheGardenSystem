using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class UI_ShopContainer : MonoBehaviour
{
    [Header("Elements")]
    public TextMeshProUGUI ResourceValueText;
    public GameObject ResourceContainer;
    public TextMeshProUGUI ObjectCategoryTitle;
    public GameObject ObjectContainer;

    [Header("Prefabs")]
    public UI_ShopElement ShopElementPrefab;

    private UI_ShopWindow ShopWindow;
    private UI_ShopContainer TargetContainer;

    private bool ShowEmptyResources;
    public ResourceCollection Resources;
    private Dictionary<ResourceDef, UI_ShopElement> ResourceElements;

    private Dictionary<ObjectDef, UI_ShopElement> ObjectElements;
    public Dictionary<ObjectDef, int> Objects;

    public void Init(UI_ShopWindow shopWindow, bool showEmptyResources, string resourceValueText, ResourceCollection resources, bool showObjects, Dictionary<ObjectDef, int> objects, UI_ShopContainer targetContainer)
    {
        ShopWindow = shopWindow;
        TargetContainer = targetContainer;

        // Resources
        Resources = new ResourceCollection(resources);
        ResourceElements = new Dictionary<ResourceDef, UI_ShopElement>();
        ResourceValueText.text = resourceValueText;
        ShowEmptyResources = showEmptyResources;
        RefreshResources();

        // Objects
        Objects = objects;
        ObjectElements = new Dictionary<ObjectDef, UI_ShopElement>();
        HelperFunctions.DestroyAllChildredImmediately(ObjectContainer);
        if (showObjects)
        {
            foreach (var obj in objects)
            {
                UI_ShopElement elem = GameObject.Instantiate(ShopElementPrefab, ObjectContainer.transform);
                elem.Init(obj.Key, obj.Value, () => ShopWindow.MoveObject(obj.Key, obj.Value, this, targetContainer));
                ObjectElements[obj.Key] = elem;
            }
        }
        else ObjectCategoryTitle.text = "";
    }


    public void AddResource(ResourceDef resource, int amount)
    {
        Resources.AddResource(resource, amount);
        RefreshResources();
    }
    public void RemoveResource(ResourceDef resource, int amount)
    {
        Resources.RemoveResource(resource, amount);
        RefreshResources();
    }
    private void RefreshResources()
    {
        HelperFunctions.DestroyAllChildredImmediately(ResourceContainer);
        foreach (var res in Resources.Resources)
        {
            if (!ShowEmptyResources && res.Value == 0) continue;
            UI_ShopElement elem = GameObject.Instantiate(ShopElementPrefab, ResourceContainer.transform);
            elem.Init(res.Key, res.Value, () => ShopWindow.MoveResource(res.Key, this, TargetContainer));
            ResourceElements[res.Key] = elem;
        }
    }

    public void AddObject(ObjectDef objectDef, int price)
    {
        Objects.Add(objectDef, price);
        UI_ShopElement elem = GameObject.Instantiate(ShopElementPrefab, ObjectContainer.transform);
        elem.Init(objectDef, price, () => ShopWindow.MoveObject(objectDef, price, this, TargetContainer));
        while(elem.transform.GetSiblingIndex() > 0 && ObjectElements.First(x => x.Value == elem.transform.parent.GetChild(elem.transform.GetSiblingIndex() - 1).GetComponent<UI_ShopElement>()).Key.Tier.Index > objectDef.Tier.Index)
        {
            elem.transform.SetSiblingIndex(elem.transform.GetSiblingIndex() - 1);
        }
        ObjectElements[objectDef] = elem;
    }
    public void RemoveObject(ObjectDef objectDef)
    {
        Objects.Remove(objectDef);
        GameObject.Destroy(ObjectElements[objectDef].gameObject);
        ObjectElements.Remove(objectDef);
    }
}
