using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : SingletonMonoBehaviour<InputManager>
{
    //KeyCode emoticon1 = KeyCode.Alpha1;
    //KeyCode emoticon2 = KeyCode.Alpha2;
    //KeyCode emoticon3 = KeyCode.Alpha3;
    //KeyCode emoticon4 = KeyCode.Alpha4;
    //KeyCode emoticon5 = KeyCode.Alpha5;
    //KeyCode emoticon6 = KeyCode.Alpha6;
    //KeyCode emoticon7 = KeyCode.Alpha7;
    //KeyCode emoticon8 = KeyCode.Alpha8;
    KeyCode OptionKey = 0;
    KeyCode ResetKey = 0;
    KeyCode LightKey = 0;
    KeyCode CromakeyMode = 0;
    KeyCode SwitchWeapon = 0;

    GameObject optionUI = null;
    Character character;
    [SerializeField] GameObject[] SpotLight = null;
    bool canSpotLight = true;
    int SpotLightSelect = 0;

    void GetEmoticonInput()
    {

    }

    public void SetOptionUI(GameObject obj)
    {
        optionUI = obj;
    }

    void SelectLight()
    {
        var Data = DataManager.Instance.GetValues(DataManager.Instance.m_OptionData, "SelectSpotLight");
        if (bool.Parse(Data))
        {
            
            SpotLightSelect = 1;
        }
        else
        {
            SpotLightSelect = 0;
        }
    }

    override protected void OnStart()
    {
        base.OnStart();
        OptionKey = KeyCode.O;
        ResetKey = KeyCode.F5;
        LightKey = KeyCode.L;
        CromakeyMode = KeyCode.Semicolon;
        SwitchWeapon = KeyCode.Tab;
        GetEmoticonInput();
        SpotLight[0].SetActive(false);
        SpotLight[1].SetActive(false);
        character = GameObject.Find("Character").GetComponent<Character>();
    }

    public void ResetApplication()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    void CloseOption()
    {
        optionUI.GetComponent<OptionUI>().DestroyOptions();
        optionUI.SetActive(false);

        character.ReplayMovement();
        SpotLight[SpotLightSelect].SetActive(false);
        MicManager.Instance.isSpotlightMode = false;
        LoopbackAudio.Instance.isSpotlightMode = false;
        PresentsManager.Instance.testDropPresent = true;
    }

    void OpenOption()
    {
        optionUI.GetComponent<OptionUI>().SetDefaultPannel();
        optionUI.SetActive(true);

        character.StopMovement();
        PresentsManager.Instance.testDropPresent = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(OptionKey) && !ChromaKeyManager.Instance.GetisHide())
        {
            if (optionUI.activeSelf)
            {
                CloseOption();
            }
            else
            {
                OpenOption();
            }
        }
        if (Input.GetKeyDown(ResetKey))
        {
            ResetApplication();
        }
        if (Input.GetKeyDown(LightKey) && !optionUI.activeSelf && canSpotLight)
        {
            SelectLight();
            if (SpotLight[SpotLightSelect].activeSelf)
            {
                SpotLight[SpotLightSelect].SetActive(false);
                MicManager.Instance.isSpotlightMode = false;
                LoopbackAudio.Instance.isSpotlightMode = false;
            }
            else
            {
                SpotLight[SpotLightSelect].SetActive(true);
                MicManager.Instance.isSpotlightMode = true;
                LoopbackAudio.Instance.isSpotlightMode = true;
            }
        }
        if (Input.GetKeyDown(CromakeyMode))
        {
            ChromaKeyManager.Instance.SetChromaKeyMode();
            if(ChromaKeyManager.Instance.GetisHide())
            {
                CloseOption();
                canSpotLight = false;
                SpotLight[SpotLightSelect].SetActive(false);
                character.SetChromaKeyMode();
            }
            else
            {
                canSpotLight = true;
                character.DisableChromaKeyMode();
            }
        }
        if (Input.GetKeyDown(SwitchWeapon))
        {
            character.SwitchWeapon();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int i = 0;
            if (character.CheckEffect(out i))
            {
                EffectManager.Instance.CreateEffect(position, i);
            }
        }
    }
}
