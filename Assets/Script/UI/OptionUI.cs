using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    [SerializeField] Toggle settingToggle = null;
    [SerializeField] GameObject settingUI = null;
    [SerializeField] Toggle donateToggle = null;
    [SerializeField] GameObject donateUI = null;
    [SerializeField] Toggle furnitureToggle = null;
    [SerializeField] GameObject furnitureUI = null;

    GameObject m_optionPrefab_Pass = null;
    [SerializeField] VerticalLayoutGroup m_optionGrid = null;
    List<GameObject> optionList = new List<GameObject>();
    List<GameObject> donateList = new List<GameObject>();
    List<GameObject> furnitureList = new List<GameObject>();
    DataManager m_dataManager = null;

    void CreateOptions()
    {
        CreateOption_Password("Option_TwippyAddress", "TwippyLogAddress", "트위피 로그파일 경로", "주소를 입력해 주세요.");
    }

    public void ToggleSettingUI()
    {
        if (settingToggle.isOn)
        {
            settingUI.SetActive(true);
            if(optionList.Count == 0)
            {
                CreateOptions();
                
            }
            SetOptions();
        }
        else
        {
            settingUI.SetActive(false);
        }
    }
    public void ToggleDonateUI()
    {
        if (donateToggle.isOn)
        {
            donateUI.SetActive(true);
            //if (optionList.Count == 0)
            //{
            //    CreateOptions();

            //}
            //SetOptions();
        }
        else
        {
            donateUI.SetActive(false);
        }
    }
    public void ToggleFurnitureUI()
    {
        if (furnitureToggle.isOn)
        {
            furnitureUI.SetActive(true);
            //if (optionList.Count == 0)
            //{
            //    CreateOptions();

            //}
            //SetOptions();
        }
        else
        {
            furnitureUI.SetActive(false);
        }
    }

    public void CreateOption_Password(string optionName, string optionKey, string name, string placeholder)
    {
        var obj = Instantiate(m_optionPrefab_Pass) as GameObject;
        obj.transform.SetParent(m_optionGrid.transform);
        obj.transform.localScale = Vector3.one;
        obj.name = optionName;
        obj.GetComponent<OptionPanel_Password>().SetPanel(optionKey, name, placeholder);
        //obj.GetComponent<OptionKey>().SetKey(optionKey);
        optionList.Add(obj);
    }


    void SetOptions()
    {
        for (int i = 0; i < optionList.Count; i++)
        {
            optionList[i].SendMessage("SetData");
        }
    }
    public void SaveDatas()
    {
        for (int i = 0; i < optionList.Count; i++)
        {
            optionList[i].SendMessage("SaveData");
        }
        m_dataManager.SaveDatas();
    }

    void Awake()
    {
        m_optionPrefab_Pass = Resources.Load("Prefab/UI/OptionPanel_Password") as GameObject;
        m_dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
    }
    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
