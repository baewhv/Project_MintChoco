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
    [SerializeField] GameObject optionUI = null;
    // Start is called before the first frame update
    override protected void OnStart()
    {
        base.OnStart();
        OptionKey = KeyCode.O;
        ResetKey = KeyCode.F5;
    }

    public static void ResetApplication()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(OptionKey))
        {
            if (optionUI.activeSelf)
            {
                optionUI.SetActive(false);
            }
            else
            {
                optionUI.SetActive(true);
            }
        }
        if (Input.GetKeyDown(ResetKey))
        {
            ResetApplication();
        }
    }
}
