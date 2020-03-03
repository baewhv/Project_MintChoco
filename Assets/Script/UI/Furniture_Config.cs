using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Furniture_Config : MonoBehaviour
{
    //토글(bool) 숨기기
    //너비 / 높이
    //위치 (x,y)
    GameObject targetObject;
    bool isSetting = false;
    
    float beforeX;
    float beforeY;
    float beforeWidth;
    float beforeHeight;
    int beforeSort;
    bool beforeFront;

    [SerializeField] GameObject addFurniture= null;
    [SerializeField] GameObject configFurniture = null;

    [SerializeField] Text m_name = null;
    [SerializeField] Image Icon = null;
    [SerializeField] InputField inputX = null;
    [SerializeField] InputField inputY = null;
    [SerializeField] InputField inputWidth = null;
    [SerializeField] InputField inputHeight = null;
    [SerializeField] InputField inputSort = null;
    [SerializeField] Button ApplyButton = null;
    [SerializeField] Button CancelButton = null;
    [SerializeField] Button ResetButton = null;
    [SerializeField] Button DeleteButton = null;
    [SerializeField] Button ApplyAddButton = null;
    [SerializeField] Button CancelAddButton = null;

    public void SetConfig(GameObject target)
    {
        configFurniture.SetActive(true);
        addFurniture.SetActive(false);
        targetObject = target;
        Icon.sprite = targetObject.GetComponent<Image>().sprite;
        RectTransform rt = (RectTransform)Icon.transform;
        if (Icon.sprite.rect.size.x > Icon.sprite.rect.size.y)
        {
            rt.sizeDelta = new Vector2(150, (Icon.sprite.rect.size.y / Icon.sprite.rect.size.x) * 150);
        }
        else
        {
            rt.sizeDelta = new Vector2((Icon.sprite.rect.size.x / Icon.sprite.rect.size.y) * 150, 150);
        }
        SetText();
        SetBeforeValue();
        //SetConfigListeners();
        ResetButton.gameObject.SetActive(true);
        DeleteButton.gameObject.SetActive(true);
        isSetting = true;

        ResetButton.gameObject.SetActive(true);
        if (targetObject.GetComponent<FurnitureObject>().GetSpecific() == 2)
        {
            DeleteButton.gameObject.SetActive(true);
        }
        else
        {
            DeleteButton.gameObject.SetActive(false);
        }

    }

    //void SetConfigListeners()   //왜 여기서 에러가 날까 고민
    //{
    //    //ApplyButton.onClick.RemoveListener(_ApplyAddConfig );
    //    //CancelButton.onClick.RemoveListener(_CancelAddConfig);
    //    ApplyButton.onClick.RemoveAllListeners();
    //    CancelButton.onClick.RemoveAllListeners();
    //}

    public void SetAdd(Sprite spr)
    {
        configFurniture.SetActive(false);
        addFurniture.SetActive(true);
        Icon.sprite = spr;
        RectTransform rt = (RectTransform)Icon.transform;
        if (Icon.sprite.rect.size.x > Icon.sprite.rect.size.y)
        {
            rt.sizeDelta = new Vector2(150, (Icon.sprite.rect.size.y / Icon.sprite.rect.size.x) * 150);
        }
        else
        {
            rt.sizeDelta = new Vector2((Icon.sprite.rect.size.x / Icon.sprite.rect.size.y) * 150, 150);
        }
        ResetButton.gameObject.SetActive(false);
        DeleteButton.gameObject.SetActive(false);
        //SetAddListeners();
    }
    //void SetAddListeners()
    //{
    //    //ApplyButton.onClick.RemoveListener( _ApplyConfig );
    //    //CancelButton.onClick.RemoveListener( _CancelConfig );
    //    ApplyButton.onClick.RemoveAllListeners();
    //    CancelButton.onClick.RemoveAllListeners();
    //}

    void SetText()
    {
        var obj = targetObject.GetComponent<FurnitureObject>();
        var rt = (RectTransform)targetObject.transform;
        inputSort.text = obj.m_sort.ToString();
        inputX.text = ((int)rt.localPosition.x).ToString();
        inputY.text = ((int)rt.localPosition.y).ToString();
        inputWidth.text = ((int)rt.sizeDelta.x).ToString();
        inputHeight.text = ((int)rt.sizeDelta.y).ToString();
        m_name.text = obj.GetName();
    }

    void SetBeforeValue()
    {
        RectTransform rt = (RectTransform)targetObject.transform;
        beforeX = rt.localPosition.x;
        beforeY = rt.localPosition.y;
        beforeWidth = rt.sizeDelta.x;
        beforeHeight = rt.sizeDelta.y;
        beforeSort = targetObject.GetComponent<FurnitureObject>().m_sort;
        beforeFront = targetObject.GetComponent<FurnitureObject>().frontOrder; 
    }

    void ReturnBeforeValue()
    {
        if(targetObject == null)    //원인 찾을것!
        {
            return;
        }
        RectTransform rt = (RectTransform)targetObject.transform;
        rt.localPosition = new Vector2(beforeX, beforeY);
        rt.sizeDelta = new Vector2(beforeWidth, beforeHeight);
        targetObject.GetComponent<FurnitureObject>().m_sort = beforeSort;
        targetObject.GetComponent<FurnitureObject>().frontOrder = beforeFront;
    }

    public void _ApplyConfig()
    {
        var obj = GameObject.Find("ObjectManager").GetComponent<ObjectManager>();
        obj.SaveObjectData(targetObject);
        isSetting = false;
        gameObject.SetActive(false);
    }
    public void _ApplyAddConfig()
    {
        var obj = GameObject.Find("ObjectManager").GetComponent<ObjectManager>();
        obj.AddObjectData(Icon.sprite);
        transform.parent.GetComponent<FurnitureUI>().CreateFurniture();
        gameObject.SetActive(false);
    }

    public void _CancelConfig()
    {
        ReturnBeforeValue();
        targetObject = null;
        isSetting = false;
        gameObject.SetActive(false);
    }

    public void _CancelAddConfig()
    {
        var obj = GameObject.Find("ObjectManager").GetComponent<ObjectManager>();
        gameObject.SetActive(false);
    }


    public void _ResetState()
    {
        targetObject.GetComponent<FurnitureObject>().ResetValue() ;

        SetText();
    }
    public void _DeleteConfig()
    {
        var obj = GameObject.Find("ObjectManager").GetComponent<ObjectManager>();
        obj.DeleteObject(targetObject);
        transform.parent.GetComponent<FurnitureUI>().CreateFurniture();
        gameObject.SetActive(false);
    }


    public void ValueChanged(string str)
    {
        if (!isSetting)
        {
            return;
        }
        RectTransform rt = (RectTransform)targetObject.transform;
        int getInt;
        switch (str)
        {
            case "posX":
                Debug.Log("text : " + inputX.text);
                if (!int.TryParse(inputX.text, out getInt))
                {
                    getInt = 0;
                }
                if(getInt > 960)
                {
                    getInt = 960;
                    inputX.text = 960.ToString();
                }
                else if(getInt < - 960)
                {
                    getInt = -960;
                    inputX.text = (-960).ToString();
                }
                rt.localPosition = new Vector2(getInt, rt.localPosition.y);
                break;
            case "posY":
                if (!int.TryParse(inputY.text, out getInt))
                {
                    getInt = 0;
                }
                if (getInt > 540)
                {
                    getInt = 540;
                    inputY.text = 540.ToString();
                }
                else if (getInt < -540)
                {
                    getInt = -540;
                    inputY.text = 540.ToString();
                }
                rt.localPosition = new Vector2(rt.localPosition.x, getInt);
                break;
            case "sizeX":
                if (!int.TryParse(inputWidth.text, out getInt))
                {
                    getInt = 0;
                }
                if (getInt > 2000)
                {
                    getInt = 2000;
                    inputWidth.text = 2000.ToString();
                }
                else if (getInt < 0)
                {
                    getInt = 0;
                    inputWidth.text = 0.ToString();
                }
                rt.sizeDelta = new Vector2(getInt, rt.sizeDelta.y);
                break;
            case "sizeY":
                if (!int.TryParse(inputHeight.text, out getInt))
                {
                    getInt = 0;
                }
                if (getInt > 2000)
                {
                    getInt = 2000;
                    inputHeight.text = 2000.ToString();
                }
                else if (getInt < 0)
                {
                    getInt = 0;
                    inputHeight.text = 0.ToString();
                }
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, getInt);
                break;
            case "sort":
                if (!int.TryParse(inputSort.text, out getInt))
                {
                    getInt = 0;
                }
                if (getInt > 100)
                {
                    getInt = 100;
                    inputSort.text = 100.ToString();
                }
                else if (getInt < 0)
                {
                    getInt = 0;
                    inputSort.text = 0.ToString();
                }
                targetObject.GetComponent<FurnitureObject>().m_sort = getInt;
                break;
        }
    }

    void Awake()
    {
        ResetButton.onClick.AddListener(() => { _ResetState(); });
        DeleteButton.onClick.AddListener(() => { _DeleteConfig(); });
        ApplyButton.onClick.AddListener(_ApplyConfig);
        CancelButton.onClick.AddListener(_CancelConfig);
        ApplyAddButton.onClick.AddListener(_ApplyAddConfig);
        CancelAddButton.onClick.AddListener(_CancelAddConfig);
    }
}
