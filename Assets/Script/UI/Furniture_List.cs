using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Furniture_List : MonoBehaviour
{
    string KeyValue;
    [SerializeField] Image Icon = null;
    [SerializeField] Text objName = null;
    [SerializeField] public Button UpButton = null;
    [SerializeField] public Button DownButton = null;
    public GameObject LinkedObject;

    public void SetPanel(string name, GameObject obj)
    {
        LinkedObject = obj;
        objName.text = obj.GetComponent<FurnitureObject>().GetName();
        Icon.sprite = LinkedObject.GetComponent<Image>().sprite;
        RectTransform rt = (RectTransform)Icon.transform;
        if (Icon.sprite.rect.size.x > Icon.sprite.rect.size.y)
        {
            rt.sizeDelta = new Vector2(100, (Icon.sprite.rect.size.y / Icon.sprite.rect.size.x) * 100);
        }
        else
        {
            rt.sizeDelta = new Vector2((Icon.sprite.rect.size.x / Icon.sprite.rect.size.y) * 100, 100);
        }
        gameObject.SetActive(false);
    }
    public void SetPanel(string _name, Sprite spr)
    {
        objName.text = _name;
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
        gameObject.SetActive(false);
    }

    public void _OpenFurnitureConfig(GameObject obj)
    {
        obj.GetComponent<Furniture_Config>().SetConfig(LinkedObject);
        obj.SetActive(true);
    }
    public void _OpenFurnitureAdd(GameObject obj, Sprite spr)
    {
        obj.GetComponent<Furniture_Config>().SetAdd(spr);
        obj.SetActive(true);
    }

    //public void _ListUpButton()
    //{

    //}
    //public void _ListDownButton()
    //{

    //}
}
