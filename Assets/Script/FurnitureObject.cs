using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureObject : MonoBehaviour
{
    private string key;
    private string m_objName;
    private int specific;
    public bool viewCamMode;

    public string GetName()
    {
        return m_objName;
    }

    public string GetKey()
    {
        return key;
    }

    public int GetSpecific()
    {
        return specific;
    }
    public void SetSpecific(int value)
    {
        specific = value;
    }


    public bool frontOrder;
    public int m_sort;
    
    int defaultX;
    int defaultY;
    int defaultWidth;
    int defaultHeight;
    bool defaultOrder;
    int defaultSort;

    public void ResetValue()
    {
        var rt = (RectTransform)transform;
        rt.localPosition = new Vector2(defaultX, defaultY);
        rt.sizeDelta = new Vector2(defaultWidth, defaultHeight);
        frontOrder = defaultOrder;
        m_sort = defaultSort;
    }

    public void SetDefaultValue(string _key, bool order, int sort, int _posX, int _posY, int _sizeX, int _sizeY, bool _frontOrder, int _sort, string _name, int _specific)
    {
        key = _key;
        frontOrder = order;
        m_sort = sort;
        defaultX = _posX;
        defaultY = _posY;
        defaultWidth = _sizeX;
        defaultHeight = _sizeY;
        defaultOrder = _frontOrder;
        defaultSort = _sort;
        m_objName = _name;
        SetSpecific(_specific);
    }
    public string GiveDefaultValue()
    {
        string and = "&";
        return frontOrder.ToString() + and +
            m_sort.ToString() + and +
            defaultX.ToString() + and +
            defaultY.ToString() + and +
            defaultWidth.ToString() + and +
            defaultHeight.ToString() + and +
            defaultOrder.ToString() + and +
            defaultSort.ToString() + and +
            m_objName + and +
            GetSpecific().ToString();
    }
}
