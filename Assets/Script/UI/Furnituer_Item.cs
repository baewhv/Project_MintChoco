using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Furnituer_Item : MonoBehaviour
{
    string KeyValue;
    [SerializeField] Image Icon = null;
    public GameObject LinkedObject;


    public void SetPanel(string name, GameObject obj)
    {
        KeyValue = name;
        LinkedObject = obj;
        Icon.sprite = LinkedObject.GetComponent<Image>().sprite;
        RectTransform rt = (RectTransform)Icon.transform;
        if(Icon.sprite.rect.size.x> Icon.sprite.rect.size.y)
        {
            rt.sizeDelta = new Vector2(100, (Icon.sprite.rect.size.y / Icon.sprite.rect.size.x) * 100);
        }
        else
        {
            rt.sizeDelta = new Vector2((Icon.sprite.rect.size.x / Icon.sprite.rect.size.y) * 100,  100);
        }
    }
    public void SetPanel(Sprite spr)
    {
        KeyValue = "furniture_" + spr.name;
        Icon.sprite = spr;
        RectTransform rt = (RectTransform)Icon.transform;
        if (Icon.sprite.rect.size.x > Icon.sprite.rect.size.y)
        {
            rt.sizeDelta = new Vector2(100, (Icon.sprite.rect.size.y / Icon.sprite.rect.size.x) * 100);
        }
        else
        {
            rt.sizeDelta = new Vector2((Icon.sprite.rect.size.x / Icon.sprite.rect.size.y) * 100, 100);
        }
    }

    public void OpenFurnitureConfig(GameObject obj)
    {
        obj.GetComponent<Furniture_Config>().SetConfig(LinkedObject);
        obj.SetActive(true);
    }
    public void OpenFurnitureAdd(GameObject obj, Sprite spr)
    {
        obj.GetComponent<Furniture_Config>().SetAdd(spr);
        obj.SetActive(true);
    }
}
