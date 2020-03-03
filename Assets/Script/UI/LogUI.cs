using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LogUI : MonoBehaviour
{
    [SerializeField] Text m_Text = null;

    public void SetText(string message, int money)
    {
        m_Text.text += message + "님 께서 " + money.ToString() + "원을 보냈습니다." + "\n";
    }
    public void SetText(string message)
    {
        m_Text.text += message + "\n";
    }

    // Start is called before the first frame update
    void Start()
    {
        if (m_Text != null)
            m_Text.text += "안녕하세요 민초여왕님!" + "\n";
    }

    // Update is called once per frame
    void Update()
    {

    }


}
