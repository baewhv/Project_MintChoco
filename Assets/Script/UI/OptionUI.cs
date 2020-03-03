using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    [SerializeField] Toggle logToggle = null;
    [SerializeField] GameObject logUI = null;

    [SerializeField] Toggle settingToggle = null;
    [SerializeField] GameObject settingUI = null;
    List<GameObject> optionList = new List<GameObject>();
    GameObject m_optionPrefab_Pass = null;
    [SerializeField] VerticalLayoutGroup m_optionGrid = null;

    [SerializeField] Toggle donateToggle = null;
    [SerializeField] GameObject donateUI = null;
    List<GameObject> donateList = new List<GameObject>();

    [SerializeField] Toggle furnitureToggle = null;
    [SerializeField] FurnitureUI furnitureUI = null;

    [SerializeField] Toggle emoticonToggle = null;
    [SerializeField] EmoticonUI emoticonUI = null;

    [SerializeField] LogUI m_LogUI = null;
    DataManager m_dataManager = null;

    public LogUI SendTVPanel()
    {
        return m_LogUI;
    }

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
            furnitureUI.gameObject.SetActive(true);
            var furniture = furnitureUI.GetComponent<FurnitureUI>();
            furniture.SetFurniturePannel();
            furniture.CreateFurniture();
        }
        else
        {
            furnitureUI.gameObject.SetActive(false);
        }
    }
    public void ToggleEmoticonUI()
    {
        if (emoticonToggle.isOn)
        {
            emoticonUI.gameObject.SetActive(true);
        }
        else
        {
            emoticonUI.gameObject.SetActive(false);
        }
    }

    public void CreateOption_Password(string optionName, string optionKey, string name, string placeholder)
    {
        var obj = Instantiate(m_optionPrefab_Pass) as GameObject;
        obj.transform.SetParent(m_optionGrid.transform);
        obj.transform.localScale = Vector3.one;
        obj.name = optionName;
        obj.GetComponent<OptionPanel_Password>().SetPanel(optionKey, name, placeholder);
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
        m_dataManager.SaveDatas(m_dataManager.m_OptionData, m_dataManager.optionConfigTXT);
    }
    public void DestroyOptions()
    {
        for(int i = 0; i < optionList.Count; i++)
        {
            Destroy(optionList[i]);
        }
        furnitureUI.ClearList();
        optionList.Clear();
        logToggle.isOn = true;

    }
    void Awake()
    {
        m_optionPrefab_Pass = Resources.Load("Prefab/UI/OptionPanel_Password") as GameObject;
        m_dataManager = GameObject.Find("DataManager").GetComponent<DataManager>();
    }
    private void Start()
    {
        SetDefaultPannel();
    }
    public void SetDefaultPannel()
    {
        logUI.SetActive(true);
        settingUI.SetActive(false);
        donateUI.SetActive(false);
        furnitureUI.gameObject.SetActive(false);
        emoticonUI.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    

    // Update is called once per frame
    void Update()
    {

    }
}
