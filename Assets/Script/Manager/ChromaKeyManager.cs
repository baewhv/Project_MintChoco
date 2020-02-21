using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChromaKeyManager : SingletonMonoBehaviour<ChromaKeyManager>
{
    [SerializeField] GameObject[] HideTargetObject = null;
    Character m_Character;
    Resolution[] res;
    bool isHide;

    public void SetChromaKeyMode()
    {
        if (!isHide)
        {
            for (int i = 0; i < HideTargetObject.Length; i++)
            {
                HideTargetObject[i].SetActive(false);
                m_Character.SetChromaKeyMode();
                
            }
            isHide = true;
        }
        else
        {
            for (int i = 0; i < HideTargetObject.Length; i++)
            {
                HideTargetObject[i].SetActive(true);
                m_Character.DisableChromaKeyMode();
            }
            isHide = false;
        }
    }

    override protected void OnStart()
    {
        base.OnStart();
        isHide = false;
        GameObject GO = GameObject.Find("Character");
        m_Character = GO.GetComponent<Character>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Semicolon))
        {
            SetChromaKeyMode();
        }
    }
}
