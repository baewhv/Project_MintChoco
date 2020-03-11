using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionPanel_Check : MonoBehaviour
{
    [SerializeField] Text nameText = null;
    [SerializeField] Toggle boolBox = null;
    string KeyValue;



    public void SetData()
    {
        var Data = DataManager.Instance.GetValues(DataManager.Instance.m_OptionData, KeyValue);
        boolBox.isOn = bool.Parse(Data);
    }

    public void SaveData()
    {
        DataManager.Instance.SaveValues(DataManager.Instance.m_OptionData, KeyValue, boolBox.isOn.ToString());
    }

    public void SetPanel(string optionKey, string nameT, string placeholderT = "경로를 입력해주세요.")
    {
        KeyValue = optionKey;
        nameText.text = nameT;
    }
}
