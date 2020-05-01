using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChromaKeyManager : SingletonMonoBehaviour<ChromaKeyManager>
{
    GameObject BackGroundObject;
    GameObject ForeGroundObject;
    GameObject TableObject;
    GameObject PresentsManager;
    GameObject Blinker;
    Character m_Character;
    int isHide;

    public int GetisHide()
    {
        return isHide;
    }

    public void SetChromaKeyMode()
    {
        if (isHide== 0)
        {
            BackGroundObject.SetActive(false);
            ForeGroundObject.SetActive(false);
            PresentsManager.SetActive(false);
            Blinker.SetActive(false);
            m_Character.SetChromaKeyMode();
            isHide++;
        }
        else if( isHide == 1)
        {
            BackGroundObject.SetActive(false);
            ForeGroundObject.SetActive(true);
            ObjectManager.Instance.SetCamMode();
            PresentsManager.SetActive(true);
            Blinker.SetActive(false);
            m_Character.SetCamBroadMode();
            isHide++;
        }
        else if (isHide == 2)
        {
            BackGroundObject.SetActive(true);
            ForeGroundObject.SetActive(true);
            ObjectManager.Instance.DisableCamMode();
            PresentsManager.SetActive(true);
            Blinker.SetActive(true);
            m_Character.DisableChromaKeyMode();
            isHide = 0;
        }
    }

    override protected void OnStart()
    {
        base.OnStart();
        m_Character = GameObject.Find("Character").GetComponent<Character>();
        BackGroundObject = GameObject.Find("BackgroundCanvas");
        ForeGroundObject = GameObject.Find("ForwardCanvas");
        PresentsManager = GameObject.Find("PresentsManager");
        Blinker = GameObject.Find("Blinker");
    }
}
