using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectManager : SingletonMonoBehaviour<ObjectManager>
{
    public enum FURN_DATA
    {
        Spr_Name = 0,
        posX,
        posY,
        sizeX,
        sizeY,
        order,
        sort,
        DposX,
        DposY,
        DsizeX,
        DsizeY,
        Dorder,
        Dsort,
        Name,
        Specific,
        Max
    }

    Canvas foreCanvas;
    Canvas backCanvas;
    GameObject m_ObjectPrefab;
    GameObject m_OptionPanelPrefab;
    GameObject m_MornitorOption;
    GameObject m_Mornitor = null;

    public List<GameObject> m_ObjectList = new List<GameObject>();

    public int GetBackCanvasCount()
    {
        return backCanvas.transform.childCount;
    }

    void GetObjectData()
    {
        Dictionary<string, string> ErrorList = new Dictionary<string, string>();
        foreach (KeyValuePair<string, string> obj in DataManager.Instance.m_FurnitureData)
        {
            if (obj.Value.Length == 0)  //받은 키 값이 없으면 키 체크
            {
                return;
            }
            string[] str = obj.Value.Split('&');
            if (str.Length <= 1 || str.Length != (int)FURN_DATA.Max)
            {
                Debug.Log("check this");
                ErrorList.Add(obj.Key, "");
            }
            else
            {
                CreateObject(obj.Key, str);
            }
        }
        if (ErrorList.Count > 0)
        {
            foreach (KeyValuePair<string, string> obj in ErrorList)
            {
                DataManager.Instance.SaveValues(DataManager.Instance.m_FurnitureData, obj.Key, obj.Value);
            }
            DataManager.Instance.SaveDatas(DataManager.Instance.m_FurnitureData, DataManager.Instance.furnitureConfigTXT);
        }
        SetObjectPosition();
    }
    public void SaveObjectData(GameObject obj)
    {
        SetTvOptionPosition();
        RectTransform rt = (RectTransform)obj.transform;
        var obm = obj.GetComponent<FurnitureObject>();
        string and = "&";
        string str = obj.GetComponent<Image>().sprite.name + and +
            ((int)rt.localPosition.x).ToString() + and +
            ((int)rt.localPosition.y).ToString() + and +
            ((int)rt.sizeDelta.x).ToString() + and +
            ((int)rt.sizeDelta.y).ToString() + and +
            obm.GiveDefaultValue();
        DataManager.Instance.SaveValues(DataManager.Instance.m_FurnitureData, obj.name, str);
        DataManager.Instance.SaveDatas(DataManager.Instance.m_FurnitureData, DataManager.Instance.furnitureConfigTXT);
    }
    public void SaveObjectData()
    {
        SetTvOptionPosition();
        for (int i = 0; i < m_ObjectList.Count; i++)
        {
            RectTransform rt = (RectTransform)m_ObjectList[i].transform;
            var obm = m_ObjectList[i].GetComponent<FurnitureObject>();
            string and = "&";
            string str = m_ObjectList[i].GetComponent<Image>().sprite.name + and +
                ((int)rt.localPosition.x).ToString() + and +
                ((int)rt.localPosition.y).ToString() + and +
                ((int)rt.sizeDelta.x).ToString() + and +
                ((int)rt.sizeDelta.y).ToString() + and +
                obm.GiveDefaultValue();
            DataManager.Instance.SaveValues(DataManager.Instance.m_FurnitureData, m_ObjectList[i].name, str);
        }
        DataManager.Instance.SaveDatas(DataManager.Instance.m_FurnitureData, DataManager.Instance.furnitureConfigTXT);
    }

    public void AddObjectData(Sprite spr)
    {
        string and = "&";
        string str = spr.name + and +
            "0" + and +
            "0" + and +
            spr.rect.size.x.ToString() + and +
            spr.rect.size.y.ToString() + and +
            "True" + and +
            (foreCanvas.transform.childCount-1).ToString() + and +
            "0" + and +
            "0" + and +
            spr.rect.size.x.ToString() + and +
            spr.rect.size.y.ToString() + and +
            "True" + and +
            (foreCanvas.transform.childCount - 1).ToString() + and +
            spr.name + and +
            "2";
        string key = "furniture_add_" + spr.name;
        DataManager.Instance.AddValues(DataManager.Instance.m_FurnitureData, "furniture_add_" + spr.name, str);
        DataManager.Instance.CopySprite(spr.name);
        DataManager.Instance.SaveDatas(DataManager.Instance.m_FurnitureData, DataManager.Instance.furnitureConfigTXT);
        var newData = DataManager.Instance.GetValues(DataManager.Instance.m_FurnitureData, key);
        string[] newDatas = newData.Split('&');
        CreateObject(key, newDatas);
        SetTvOptionPosition();
    }

    void CreateObject(string key, string[] str)
    {
        var obj = Instantiate(m_ObjectPrefab) as GameObject;
        obj.name = key;
        Sprite spr;
        if (str[(int)FURN_DATA.Specific] == "2") //고정 항목이 아닌 추가 항목
        {
            spr = DataManager.Instance.SetStreamingImage(str[(int)FURN_DATA.Spr_Name]);

        }
        else
        {
            spr = Resources.Load<Sprite>("Texture/Furniture/" + str[(int)FURN_DATA.Spr_Name]);
        }

        if (spr != null)
        {
            obj.GetComponent<Image>().sprite = spr;
        }

        if (key == "Furniture_TV")
        {
            m_Mornitor = obj;

        }

        if (bool.Parse(str[(int)FURN_DATA.order]))
        {
            obj.transform.SetParent(foreCanvas.transform);
        }
        else
        {
            obj.transform.SetParent(backCanvas.transform);
        }

        RectTransform rt = (RectTransform)obj.transform;
        rt.localPosition = new Vector3(int.Parse(str[(int)FURN_DATA.posX]), int.Parse(str[(int)FURN_DATA.posY]), 0);
        rt.localScale = Vector3.one;
        rt.sizeDelta = new Vector3(int.Parse(str[(int)FURN_DATA.sizeX]), int.Parse(str[(int)FURN_DATA.sizeY]), 0);
        obj.GetComponent<FurnitureObject>().
            SetDefaultValue(
            key,
            bool.Parse(str[(int)FURN_DATA.order]),
            int.Parse(str[(int)FURN_DATA.sort]),
            int.Parse(str[(int)FURN_DATA.DposX]),
            int.Parse(str[(int)FURN_DATA.DposY]),
            int.Parse(str[(int)FURN_DATA.DsizeX]),
            int.Parse(str[(int)FURN_DATA.DsizeY]),
            bool.Parse(str[(int)FURN_DATA.Dorder]),
            int.Parse(str[(int)FURN_DATA.Dsort]),
            str[(int)FURN_DATA.Name],
            int.Parse(str[(int)FURN_DATA.Specific])
            );


        m_ObjectList.Add(obj);
    }

    public void DeleteObject(GameObject obj)
    {
        for (int i = 0; i < m_ObjectList.Count; i++)
        {
            if (m_ObjectList[i].name != obj.name)
            {
                continue;
            }
            Debug.Log(m_ObjectList[i].name);
            DataManager.Instance.DeleteValue(DataManager.Instance.m_FurnitureData, obj.name);
            DataManager.Instance.DeleteSprite(obj.GetComponent<Image>().sprite.name);
            DestroyImmediate(m_ObjectList[i]);
            m_ObjectList.Remove(m_ObjectList[i]);
            break;
        }
        SaveObjectSiblingPosition();
    }

    public void SetTvOptionPosition()
    {
        m_MornitorOption.transform.SetAsLastSibling();
        RectTransform rt = (RectTransform)m_MornitorOption.transform;
        rt.localPosition = new Vector3(m_Mornitor.transform.localPosition.x, m_Mornitor.transform.localPosition.y + 16);
    }

    void SetTVonOption()
    {
        
        var obj = Instantiate(m_OptionPanelPrefab) as GameObject;
        obj.transform.SetParent(foreCanvas.transform);
        obj.transform.localScale = Vector3.one;
        RectTransform rt = (RectTransform)obj.transform;
        rt.localPosition = new Vector3(m_Mornitor.transform.localPosition.x, m_Mornitor.transform.localPosition.y + 16);
        rt.sizeDelta = new Vector2(752f, 419f);
        InputManager.Instance.SetOptionUI(obj);
        DonateManager.Instance.getTVMessage(obj.GetComponent<OptionUI>().SendTVPanel());
        m_MornitorOption = obj;


    }

    public void SetObjectPosition()
    {
        for (int  i = 0; i < m_ObjectList.Count; i++)
        {
            m_ObjectList[i].transform.SetSiblingIndex(m_ObjectList[i].GetComponent<FurnitureObject>().m_sort);
        }
        
    }

    public void SaveObjectSiblingPosition()
    {
        for(int i = 0; i < m_ObjectList.Count; i++)
        {
            m_ObjectList[i].GetComponent<FurnitureObject>().m_sort = m_ObjectList[i].transform.GetSiblingIndex();
        }
        SaveObjectData();
    }

    public void ObjectMoveUp(GameObject obj)
    {
        for (int i = 0; i < m_ObjectList.Count; i++)
        {
            if (m_ObjectList[i] == obj)
            {
                int a = obj.transform.GetSiblingIndex();
                if (a + 1 != m_ObjectList.Count)
                {

                    obj.transform.SetSiblingIndex(a + 1);
                }
            }
        }
        SaveObjectSiblingPosition();
    }

    public void ObjectMoveDown(GameObject obj)
    {
        for (int i = 0; i < m_ObjectList.Count; i++)
        {
            if (m_ObjectList[i] != obj)
            {
                continue;
            }
            int a = obj.transform.GetSiblingIndex();
            if (a - 1 != m_ObjectList.Count)
            {
                obj.transform.SetSiblingIndex(a - 1);
            }
        }
        SaveObjectSiblingPosition();
    }

    public void ObjectSwitchPosition(GameObject obj)
    {
        if(obj.GetComponent<FurnitureObject>().frontOrder)
        {
            obj.GetComponent<FurnitureObject>().frontOrder = false;
            obj.transform.SetParent(backCanvas.transform);
            obj.transform.SetAsLastSibling();
        }
        else
        {
            obj.GetComponent<FurnitureObject>().frontOrder = true;
            obj.transform.SetParent(foreCanvas.transform);
            obj.transform.SetAsFirstSibling();
        }
        SaveObjectSiblingPosition();
        SaveObjectData();
    }

    override protected void OnAwake()
    {
        base.OnAwake();
        foreCanvas = GameObject.Find("ForwardCanvas").GetComponent<Canvas>();
        backCanvas = GameObject.Find("BackgroundCanvas").GetComponent<Canvas>();
        m_ObjectPrefab = Resources.Load("Prefab/FurniturePrefab") as GameObject;
        m_OptionPanelPrefab = Resources.Load("Prefab/UI/OptionPanel") as GameObject;
    }
    protected override void OnStart()
    {
        base.OnStart();
        GetObjectData();
        SetTVonOption();

    }

    void Update()
    {
        
    }
}
