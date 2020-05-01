using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    [SerializeField] Toggle logToggle = null;
    [SerializeField] LogUI logUI = null;

    [SerializeField] Toggle settingToggle = null;
    [SerializeField] GameObject settingUI = null;
    List<GameObject> optionList = new List<GameObject>();
    GameObject m_optionPrefab_Pass = null;
    GameObject m_optionPrefab_Check = null;
    [SerializeField] VerticalLayoutGroup m_optionGrid = null;

    [SerializeField] Toggle presentToggle = null;
    [SerializeField] PresentUI presentUI = null;

    [SerializeField] Toggle furnitureToggle = null;
    [SerializeField] FurnitureUI furnitureUI = null;

    [SerializeField] Toggle emoticonToggle = null;
    [SerializeField] EmoticonUI emoticonUI = null;

    [SerializeField] Toggle micToggle = null;
    [SerializeField] MicUI micUI = null;
    
    public LogUI SendTVPanel()
    {
        return logUI;
    }

    void CreateOptions()
    {
        CreateOption_Check("Option_UseTwippyTXTDonate", "UseTwippyTXTDonate", "도네 아이템 받기(트위피 텍스트 파일)");
        CreateOption_Password("Option_TwippyAddress", "TwippyLogAddress", "트위피 로그파일 경로", "주소를 입력해 주세요.");
        CreateOption_Check("Option_UseTwipSocketDonate", "UseTwipSocketDonate", "도네 아이템 받기(트윕 경로)");
        CreateOption_Password("Option_TwipWssAddress", "TwipWssAddress", "트윕 경로", "경로 유출에 주의하세요!");
        CreateOption_Password("Option_ToonationAddress", "ToonationAddress", "투네이션 경로", "경로 유출에 주의하세요!");
        CreateOption_Check("Option_SelectSpotLight", "SelectSpotLight", "호우! 제대로 놀아보자!");

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
    public void TogglePresentUI()
    {
        if (presentToggle.isOn)
        {
            presentUI.gameObject.SetActive(true);
            presentUI.SetPresentGroupPannel();
            presentUI.CreatePresentsGroup();
        }
        else
        {
            presentUI.CloseObject();
            presentUI.gameObject.SetActive(false);
        }
    }
    public void ToggleFurnitureUI()
    {
        if (furnitureToggle.isOn)
        {
            furnitureUI.gameObject.SetActive(true);
            furnitureUI.SetFurniturePannel();
            furnitureUI.CreateFurniture();
        }
        else
        {
            furnitureUI.ClearList();
            furnitureUI.gameObject.SetActive(false);
        }
    }
    public void ToggleEmoticonUI()
    {
        if (emoticonToggle.isOn)
        {
            emoticonUI.gameObject.SetActive(true);
            emoticonUI.SetPanel();
        }
        else
        {
            emoticonUI.gameObject.SetActive(false);
        }
    }
    public void ToggleMicUI()
    {
        if (micToggle.isOn)
        {
            micUI.gameObject.SetActive(true);
            micUI.SetPannel();
        }
        else
        {
            micUI.OutPannel();
            micUI.gameObject.SetActive(false);
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
    public void CreateOption_Check(string optionName, string optionKey, string name)
    {
        var obj = Instantiate(m_optionPrefab_Check) as GameObject;
        obj.transform.SetParent(m_optionGrid.transform);
        obj.transform.localScale = Vector3.one;
        obj.name = optionName;
        obj.GetComponent<OptionPanel_Check>().SetPanel(optionKey, name);
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
        DataManager.Instance.SaveDatas(DataManager.Instance.m_OptionData, DataManager.Instance.optionConfigTXT);
        SocketIO.SocketIOManager.Instance.ResetWebSocket();
        DonateManager.Instance.ResetDonateManager();
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
        m_optionPrefab_Check = Resources.Load("Prefab/UI/OptionPanel_Check") as GameObject;
    }
    private void Start()
    {
        SetDefaultPannel();
    }
    public void SetDefaultPannel()
    {
        logUI.gameObject.SetActive(true);
        settingUI.SetActive(false);
        presentUI.gameObject.SetActive(false);
        furnitureUI.gameObject.SetActive(false);
        emoticonUI.gameObject.SetActive(false);
        micUI.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    

    // Update is called once per frame
    void Update()
    {

    }
}
