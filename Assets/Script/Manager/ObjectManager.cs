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
    DataManager DM;
    GameObject m_ObjectPrefab;
    GameObject m_OptionPanelPrefab;

    public List<GameObject> m_ObjectList = new List<GameObject>();

    void GetObjectData()
    {
        Dictionary<string, string> ErrorList = new Dictionary<string, string>();
        foreach (KeyValuePair<string,string> obj in DM.m_FurnitureData)
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
                DM.SaveValues(DM.m_FurnitureData, obj.Key, obj.Value);
            }
            DM.SaveDatas(DM.m_FurnitureData, DM.furnitureConfigTXT);
        }
    }
    public void SaveObjectData(GameObject obj)
    {
        RectTransform rt = (RectTransform)obj.transform;
        var obm = obj.GetComponent<FurnitureObject>();
        string and = "&";
        string str = obj.GetComponent<Image>().sprite.name + and+
            ((int)rt.localPosition.x).ToString() + and +
            ((int)rt.localPosition.y).ToString() + and +
            ((int)rt.sizeDelta.x).ToString() + and +
            ((int)rt.sizeDelta.y).ToString() + and +
            obm.GiveDefaultValue();
        DM.SaveValues(DM.m_FurnitureData, obj.name, str);
        DM.SaveDatas(DM.m_FurnitureData, DM.furnitureConfigTXT);
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
            0 + and +
            "0" + and +
            "0" + and +
            spr.rect.size.x.ToString() + and +
            spr.rect.size.y.ToString() + and +
            "True" + and +
            0 + and +
            spr.name + and +
            "2";
        string key = "furniture_add_" + spr.name;
        DM.AddValues(DM.m_FurnitureData, "furniture_add_" + spr.name, str);
        DM.CopySprite(spr.name);
        DM.SaveDatas(DM.m_FurnitureData, DM.furnitureConfigTXT);
        var newData = DM.GetValues(DM.m_FurnitureData, key);
        string[] newDatas = newData.Split('&');
        CreateObject(key, newDatas);
    }

    void CreateObject(string key, string[] str)
    {
        var obj = Instantiate(m_ObjectPrefab) as GameObject;
        obj.name = key;
        Sprite spr;
        if (str[(int)FURN_DATA.Specific] == "2")
        {
            spr = DM.SetStreamingImage(str[(int)FURN_DATA.Spr_Name]);
            
        }
        else
        {
            spr = Resources.Load<Sprite>(str[(int)FURN_DATA.Spr_Name]);
        }
        if (spr != null)
        {
            obj.GetComponent<Image>().sprite = spr;
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
            DM.DeleteValue(DM.m_FurnitureData, obj.name);
            DM.DeleteSprite(obj.GetComponent<Image>().sprite.name);
            Destroy(m_ObjectList[i]);
            m_ObjectList.Remove(m_ObjectList[i]);
            
        }
        DM.SaveDatas(DM.m_FurnitureData, DM.furnitureConfigTXT);
    }

    void SetTV()
    {
        for(int i = 0; i < m_ObjectList.Count; i++)
        {
            if(m_ObjectList[i].name == "Furniture_TV")
            {
                var obj = Instantiate(m_OptionPanelPrefab) as GameObject;
                obj.transform.SetParent(m_ObjectList[i].transform);
                obj.transform.localScale = Vector3.one;
                RectTransform rt = (RectTransform)obj.transform;
                rt.localPosition = new Vector3(0, 16);
                rt.sizeDelta = new Vector2(752f, 419f);
                GameObject.Find("InputManager").GetComponent<InputManager>().SetOptionUI(obj);
                GameObject.Find("DonateManager").GetComponent<DonateManager>().getTVMessage(obj.GetComponent<OptionUI>().SendTVPanel());
            }
        }
    }

    override protected void OnAwake()
    {
        base.OnAwake();
        DM = GameObject.Find("DataManager").GetComponent<DataManager>();
        foreCanvas = GameObject.Find("ForwardCanvas").GetComponent<Canvas>();
        backCanvas = GameObject.Find("BackgroundCanvas").GetComponent<Canvas>();
        m_ObjectPrefab = Resources.Load("Prefab/FurniturePrefab") as GameObject;
        m_OptionPanelPrefab = Resources.Load("Prefab/UI/OptionPanel") as GameObject;

    }
    protected override void OnStart()
    {
        base.OnStart();
        GetObjectData();

        SetTV();
    }

    void Update()
    {
        
    }
}
