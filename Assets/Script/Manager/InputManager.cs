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
    // Start is called before the first frame update

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
        SpotLight.SetActive(false);
        character = GameObject.Find("Character").GetComponent<Character>();
        m_ChromaKeyManager = GameObject.Find("ChromakeyManager").GetComponent<ChromaKeyManager>();
    }

    public static void ResetApplication()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(OptionKey))
        {
            if (optionUI.activeSelf)
            {
                optionUI.GetComponent<OptionUI>().DestroyOptions();
                optionUI.SetActive(false);
                
                character.ReplayMovement();
                SpotLight.SetActive(false);
                GameObject.Find("PresentsManager").GetComponent<PresentsManager>().testDropPresent = true;
            }
            else
            {
                optionUI.GetComponent<OptionUI>().SetDefaultPannel(); 
                optionUI.SetActive(true);

                character.StopMovement();
                GameObject.Find("PresentsManager").GetComponent<PresentsManager>().testDropPresent = false;
            }
        }
        if (Input.GetKeyDown(ResetKey))
        {
            ResetApplication();
        }
        if (Input.GetKeyDown(LightKey) && !optionUI.activeSelf)
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
        }
    }
}
