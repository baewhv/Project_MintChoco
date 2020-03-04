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

    GameObject optionUI = null;
    Character character;
    [SerializeField] GameObject SpotLight = null;
    ChromaKeyManager m_ChromaKeyManager;
    PresentsManager m_PresentsManger;
    bool canSpotLight = true;

    void GetEmoticonInput()
    {

    }

    public void SetOptionUI(GameObject obj)
    {
        optionUI = obj;
    }

    override protected void OnStart()
    {
        base.OnStart();
        OptionKey = KeyCode.O;
        ResetKey = KeyCode.F5;
        LightKey = KeyCode.L;
        CromakeyMode = KeyCode.Semicolon;
        GetEmoticonInput();
        SpotLight.SetActive(false);
        character = GameObject.Find("Character").GetComponent<Character>();
        m_ChromaKeyManager = GameObject.Find("ChromakeyManager").GetComponent<ChromaKeyManager>();
        m_PresentsManger = GameObject.Find("PresentsManager").GetComponent<PresentsManager>();
    }

    public static void ResetApplication()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    void CloseOption()
    {
        optionUI.GetComponent<OptionUI>().DestroyOptions();
        optionUI.SetActive(false);

        character.ReplayMovement();
        SpotLight.SetActive(false);
        m_PresentsManger.testDropPresent = true;
    }

    void OpenOption()
    {
        optionUI.GetComponent<OptionUI>().SetDefaultPannel();
        optionUI.SetActive(true);

        character.StopMovement();
        m_PresentsManger.testDropPresent = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(OptionKey) && !m_ChromaKeyManager.GetisHide())
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
            if (SpotLight.activeSelf)
            {
                SpotLight.SetActive(false);
            }
            else
            {
                SpotLight.SetActive(true);
            }
        }
        if (Input.GetKeyDown(CromakeyMode))
        {
            m_ChromaKeyManager.SetChromaKeyMode();
            if(m_ChromaKeyManager.GetisHide())
            {
                CloseOption();
                canSpotLight = false;
                SpotLight.SetActive(false);
            }
            else
            {
                canSpotLight = true;
            }
        }
    }
}
