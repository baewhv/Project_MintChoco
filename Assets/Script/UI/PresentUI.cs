using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PresentUI : MonoBehaviour
{
    List<GameObject> presentList = new List<GameObject>();
    [SerializeField] GridLayoutGroup m_presentGrid = null;
    [SerializeField] VerticalLayoutGroup m_presentGroupList = null;

    GameObject m_PresentIcon = null;
    GameObject m_PresentGrouplistPrefab = null;
    GameObject m_addOption = null;
    GameObject m_PresentConfig  = null;

    public void CreatePresentsGroup()    //도네 그룹을 우선 만든다.
    {
        ClearList();
        m_presentGroupList.gameObject.SetActive(true);
        m_presentGrid.gameObject.SetActive(false);
        GetComponent<ScrollRect>().content = (RectTransform)m_presentGroupList.transform;
        var obj = PresentsManager.Instance.m_presentGroup;
        for (int i = 0; i < obj.Length; i++)
        {
            CreatePresentsGroupListIcon(obj[i]);
        }
        CreateAddDonateGroup();
        //SortFurniture();
    }

    void CreatePresentsGroupListIcon(PresentGroup _obj)
    {
        var obj = Instantiate(m_PresentGrouplistPrefab) as GameObject;
        obj.transform.SetParent(m_presentGroupList.transform);
        obj.transform.localScale = Vector3.one;

        
        obj.GetComponent<Present_List>().SetPanel(_obj);
        obj.GetComponent<Button>().onClick.AddListener(() => { obj.GetComponent<Present_List>()._OpenPresentConfig(m_PresentConfig, this.gameObject); });
        presentList.Add(obj);
    }

    public void CreateAddDonateGroup()
    {
        var obj = Instantiate(m_addOption) as GameObject;
        obj.transform.SetParent(m_presentGroupList.transform);
        
        obj.transform.localScale = Vector3.one;
        //obj.GetComponent<Button>().onClick.AddListener(() => { _OpenSpriteList(); });
        presentList.Add(obj);
    }

    public void ClearList()
    {
        for (int i = 0; i < presentList.Count; i++)
        {
            DestroyImmediate(presentList[i]);
        }
        presentList.Clear();
    }


    void Awake()
    {
        m_PresentIcon = Resources.Load("Prefab/UI/Furniture_Icon") as GameObject;
        m_PresentGrouplistPrefab = Resources.Load("Prefab/UI/Present_Group_List") as GameObject;
        m_addOption = Resources.Load("Prefab/UI/OptionList_Add") as GameObject;
        m_PresentConfig = Resources.Load("Prefab/UI/Present_Config") as GameObject;
        m_presentGrid.gameObject.SetActive(false);
        m_presentGroupList.gameObject.SetActive(false);
        SetPresentGroupPannel();
    }

    public void SetPresentGroupPannel()
    {
        m_PresentConfig.SetActive(false);
    }
}
