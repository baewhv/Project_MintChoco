using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : SingletonMonoBehaviour<LightManager>
{
    int SpotLightSelect = 0;
    bool isLightOn;
    //[SerializeField] string[] PrefabName = null;
    [SerializeField] GameObject[] SpotLight = null;
    GameObject LightRMC;


    public void SetLightMRCUI(GameObject _obj)
    {
        LightRMC = _obj;
    }

    public void LightOff()
    {
        isLightOn = false;
        SpotLight[SpotLightSelect].SetActive(false);
        ObjectManager.Instance.LightMRCActive(false);
    }

    public void LightOn()
    {
        isLightOn = true;
        SpotLight[SpotLightSelect].SetActive(true);
        ObjectManager.Instance.LightMRCActive(true);
    }
    
    public bool IsLightOn()
    {
        return isLightOn;
    }

    public void SelectNextLight()
    {
        SpotLight[SpotLightSelect].SetActive(false);
        if (SpotLightSelect + 1 > SpotLight.Length - 1)
        {
            SpotLightSelect = 0;
        }
        else
        {
            SpotLightSelect++;
        }
        SpotLight[SpotLightSelect].SetActive(true);
        SaveLightOption();
    }

    public void SelectPrevLight()
    {
        SpotLight[SpotLightSelect].SetActive(false);
        if (SpotLightSelect - 1 < 0)
        {
            SpotLightSelect = SpotLight.Length -1;
        }
        else
        {
            SpotLightSelect--;
        }
        SpotLight[SpotLightSelect].SetActive(true);
        SaveLightOption();
    }

    void SaveLightOption()
    {
        DataManager.Instance.SaveValues(DataManager.Instance.m_OptionData, "SelectSpotLight", SpotLightSelect.ToString());
        DataManager.Instance.SaveDatas(DataManager.Instance.m_OptionData, DataManager.Instance.optionConfigTXT);
    }

    public void LoadLightOption()
    {
        var Data = DataManager.Instance.GetValues(DataManager.Instance.m_OptionData, "SelectSpotLight");
        bool checkBool;
        int checkInt;

        if (bool.TryParse(Data, out checkBool)) //여왕님 정상적인 플레이 후 제거!
        {
            //Debug.Log("LoadSuccess : bool");
            if (checkBool)
            {
                
                SpotLightSelect = 1;
            }
            else
            {
                SpotLightSelect = 0;
            }
        }
        else if(int.TryParse(Data, out checkInt))
        {
            //Debug.Log("LoadSuccess : int");
            SpotLightSelect = checkInt;
        }
    }

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    // Start is called before the first frame update
    protected override void OnStart()
    {
        base.OnStart();
        LoadLightOption();
        isLightOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
