using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChromaKeyManager : SingletonMonoBehaviour<ChromaKeyManager>
{
    GameObject BackGroundObject;
    GameObject ForeGroundObject;
    GameObject PresentsManager;
    GameObject Blinker;
    Character m_Character;
    bool isHide;

    public bool GetisHide()
    {
        return isHide;
    }

    public void SetChromaKeyMode()
    {
        if (!isHide)
        {
            BackGroundObject.SetActive(false);
            ForeGroundObject.SetActive(false);
            PresentsManager.SetActive(false);
            Blinker.SetActive(false);
            m_Character.SetChromaKeyMode();
            isHide = true;
        }
        else
        {
            BackGroundObject.SetActive(true);
            ForeGroundObject.SetActive(true);
            PresentsManager.SetActive(true);
            Blinker.SetActive(true);
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
        Blinker = GameObject.Find("Blinker");


    }
    
}
