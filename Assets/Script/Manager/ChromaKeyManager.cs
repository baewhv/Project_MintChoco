using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChromaKeyManager : SingletonMonoBehaviour<ChromaKeyManager>
{
    [SerializeField] GameObject BackGroundObject;
    [SerializeField] GameObject ForeGroundObject;
    [SerializeField] GameObject PresentsManager;
    Character m_Character;
    bool isHide;

    public void SetChromaKeyMode()
    {
        if (!isHide)
        {
            BackGroundObject.SetActive(false);
            ForeGroundObject.SetActive(false);
            PresentsManager.SetActive(false);
            m_Character.SetChromaKeyMode();
            isHide = true;
        }
        else
        {
            BackGroundObject.SetActive(true);
            ForeGroundObject.SetActive(true);
            PresentsManager.SetActive(true);
            m_Character.DisableChromaKeyMode();
            isHide = false;
        }
    }
    


    override protected void OnStart()
    {
        base.OnStart();
        isHide = false;
        m_Character = GameObject.Find("Character").GetComponent<Character>();
        BackGroundObject = GameObject.Find("BackgroundCanvas");
        ForeGroundObject = GameObject.Find("ForwardCanvas");
        PresentsManager = GameObject.Find("PresentsManager");


    }
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Semicolon))
        {
            SetChromaKeyMode();
        }
    }
}
