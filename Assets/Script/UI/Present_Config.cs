using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Present_Config : MonoBehaviour
{
    PresentGroup m_presentObject;
    bool isSetting = false;

    int beforePrice = 1000;
    bool beforeIsSame = false;
    int beforeExtract = 1;
    int beforePriceValue = 0;

    GameObject m_PresenticonPrefab = null;

    List<GameObject> m_itemList = new List<GameObject>();
    [SerializeField] Text m_name = null;
    [SerializeField] InputField inputPrice = null;
    [SerializeField] Toggle toggleIsSame  = null;
    [SerializeField] InputField inputExtract = null;
    [SerializeField] InputField inputPriceValue = null;
    [SerializeField] HorizontalLayoutGroup iconGroup = null;
    [SerializeField] Button ApplyButton = null;
    [SerializeField] Button CancelButton = null;
    [SerializeField] Button ResetButton = null;
    [SerializeField] Button DeleteButton = null;

    public void SetPresentGroupConfig(PresentGroup target)
    {
        m_presentObject = target;
        SetText();
        SetBeforeValue();
        //SetConfigListeners();
        ApplyButton.onClick.AddListener(_ApplyConfig);
        CancelButton.onClick.AddListener(_CancelConfig);
        if (target.m_GroupData == PresentGroup.GroupData.Default)
        {
            ResetButton.interactable = true;
            DeleteButton.interactable = false;
            ResetButton.onClick.AddListener(() => { _ResetState(); });
        }
        else
        {
            ResetButton.interactable = false;
            DeleteButton.interactable = true;
            DeleteButton.onClick.AddListener(() => { _DeleteConfig(); });
        }
        for(int i = 0; i < target.m_presentList.Length; i++)
        {
            CreatePresentsIcon(target.m_presentList[i]);
        }

        
        isSetting = true;
    }

    void CreatePresentsIcon(int _sprNum)
    {
        if(m_PresenticonPrefab == null)
        {
            SetPrefab();
        }
        var obj = Instantiate(m_PresenticonPrefab) as GameObject;
        obj.transform.SetParent(iconGroup.transform);
        obj.transform.localScale = Vector3.one;

        obj.GetComponent<OptionGrid_Icon>().SetPanel(PresentsManager.Instance.m_presents[_sprNum]);
        //obj.GetComponent<Button>().onClick.AddListener(() => { obj.GetComponent<Present_List>()._OpenPresentConfig(m_PresentConfig, gameObject); });
        m_itemList.Add(obj);
    }

    void SetText()  //받은 값 최초 1회 배치.
    {
        inputPrice.text = m_presentObject.m_price.ToString();
        toggleIsSame.isOn = m_presentObject.m_isSame;
        inputExtract.text = m_presentObject.m_extractCount.ToString();
        inputPriceValue.text = m_presentObject.m_addPrice.ToString();
        m_name.text = m_presentObject.m_name;
    }

    void SetText(PresentGroup obj)  //다른 값(초기값 등)을 받을 때 배치용
    {
        inputPrice.text = obj.m_price.ToString();
        toggleIsSame.isOn = obj.m_isSame;
        inputExtract.text = obj.m_extractCount.ToString();
        inputPriceValue.text = obj.m_addPrice.ToString();
        m_name.text = obj.m_name;
    }

    void ResetText()  //받은 값 최초 1회 배치.
    {
        for(int i = 0; i < PresentsManager.Instance.m_presentGroup.Length; i++)
        {
            if(PresentsManager.Instance.m_presentGroup[i].m_key == m_presentObject.m_key)
            {
                SetText(PresentsManager.Instance.m_presentGroup[i]);
            }
        }
        
    }

    void SetBeforeValue()
    {
        beforePrice = m_presentObject.m_price;
        beforeIsSame = m_presentObject.m_isSame;
        beforeExtract = m_presentObject.m_extractCount;
        beforePriceValue = m_presentObject.m_addPrice;
    }

    void ApplyData()
    {
        m_presentObject.m_price = int.Parse(inputPrice.text);
        m_presentObject.m_isSame = toggleIsSame.isOn;
        m_presentObject.m_extractCount = int.Parse(inputExtract.text);
        m_presentObject.m_addPrice = int.Parse(inputPriceValue.text);
        if (m_presentObject.m_GroupData == PresentGroup.GroupData.AddObject)
        {
            m_presentObject.m_name = string.Format("{0}원 {1}도네", m_presentObject.m_price, m_presentObject.m_isSame ? "" : "이상 "); ;
        }
    }


    #region ButtonActions
    public void _ApplyConfig()
    {
        ApplyData();
        PresentsManager.Instance.SavePresentGroupData(m_presentObject);
        Destroy(gameObject);
    }

    public void _ApplyAddConfig()
    {
        ApplyData();
        PresentsManager.Instance.AddPresentGroupData(m_presentObject);
        transform.parent.GetComponent<FurnitureUI>().CreateFurniture();
        Destroy(gameObject);
    }

    public void _CancelConfig()
    {
        Destroy(gameObject);
    }

    public void _CancelAddConfig()
    {
        m_presentObject = null;
        Destroy(this);
    }

    public void _ResetState()
    {
        ResetText();
    }
    public void _DeleteConfig()
    {
        transform.parent.GetComponent<PresentUI>().CreatePresentsGroup();
        gameObject.SetActive(false);
    }
    #endregion

    
    public void Awake()
    {
        SetPrefab();
    }

    public void SetPrefab()
    {
        m_PresenticonPrefab = Resources.Load("Prefab/UI/OptionGrid_Icon") as GameObject;
    }



    public void ValueChanged(string str)
    {
        if (!isSetting)
        {
            return;
        }
        int getInt = 0;
        switch (str)
        {
            case "Price":
                if (!int.TryParse(inputPrice.text, out getInt))
                {
                    getInt = 1000;
                }
                if (getInt > 500000)
                {
                    getInt = 500000;
                    inputPrice.text = 500000.ToString();
                }
                else if (getInt < 1000)
                {
                    getInt = 1000;
                    inputPrice.text = 1000.ToString();
                }
                if(getInt % 100 != 0)
                {
                    getInt -= getInt % 100;
                }
                break;
            case "Extract":
                if (!int.TryParse(inputExtract.text, out getInt))
                {
                    getInt = 1;
                }
                if (getInt > 10)
                {
                    getInt = 10;
                    inputExtract.text = 10.ToString();
                }
                else if (getInt < 1)
                {
                    getInt = 1;
                    inputExtract.text = 1.ToString();
                }
                break;
            case "PriceValue":
                if (!int.TryParse(inputPriceValue.text, out getInt))
                {
                    getInt = 0;
                }
                if (getInt > 500000)
                {
                    getInt = 500000;
                    inputPriceValue.text = 500000.ToString();
                }
                else if (getInt < 0)
                {
                    getInt = 0;
                    inputPriceValue.text = 0.ToString();
                }
                break;
        }
    }
}
