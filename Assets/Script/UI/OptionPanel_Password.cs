using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionPanel_Password : MonoBehaviour
{
    [SerializeField] Text placeholderText = null;
    [SerializeField] Text nameText = null;
    [SerializeField] InputField inputText = null;
    string KeyValue;

    public void ShowPassword()
    {
        inputText.contentType = InputField.ContentType.Standard;
        inputText.ForceLabelUpdate();
    }
    public void HidePassword()
    {
        inputText.contentType = InputField.ContentType.Password;
        inputText.ForceLabelUpdate();
    }

    public void SetData()
    {
        DataManager DM = GameObject.Find("DataManager").GetComponent<DataManager>();
        //Debug.Log(KeyValue);
        inputText.text = DM.GetValues(DM.m_OptionData, KeyValue);
    }

    public void SaveData()
    {
        DataManager DM = GameObject.Find("DataManager").GetComponent<DataManager>();
        Debug.Log(KeyValue);
        ;
        DM.SaveValues(DM.m_OptionData, KeyValue, inputText.text);
    }

    public void SetPanel(string optionKey, string nameT, string placeholderT = "경로를 입력해주세요.")
    {
        KeyValue = optionKey;
        nameText.text = nameT;
        placeholderText.text = placeholderT;
    }
}
