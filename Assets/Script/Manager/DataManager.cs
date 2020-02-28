using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

public class DataManager : SingletonMonoBehaviour<DataManager>
{
    Dictionary<string, string> m_Data = new Dictionary<string, string>();
    TVMassege m_massege;

    //public string FindDefaultPath()
    //{
    //    if (File.Exists(Application.persistentDataPath + "/Config.txt"))    //저장된 컨픽 파일이 있다면
    //    {
    //        return ReadDefaultPath();
    //    }
    //    else                                                                //저장된 컨픽 파일이 없다면
    //    {
    //        if (File.Exists(Application.streamingAssetsPath + "/Config.txt"))   //스트리밍 에셋에 저장된 텍스트 경로를 이동.
    //        {
    //            File.Copy(Application.streamingAssetsPath + "/Config.txt", Application.persistentDataPath + "/Config.txt");
    //            return ReadDefaultPath();
    //        }
    //    }
    //    return "PathNotFound";
    //}

    void CreateDefaultData()
    {
        CheckData("TwippyLogAddress", @"C\users");
        CheckData("TwippyLogAddress", @"C\users");
        CheckData("TwippyLogAddress", @"C\users");
        CheckData("TwippyLogAddress", @"C\users");
        CheckData("TwippyLogAddress", @"C\users");
        SaveDatas();
    }

    void CheckData(string key, string value)
    {
        if (!m_Data.ContainsKey(key))
        {
            m_Data.Add(key, value);
        }
    }

    void WriteData(string strData)
    {
        FileStream f = new FileStream(Application.persistentDataPath + "/Config.txt", FileMode.Create, FileAccess.Write);
        StreamWriter writer = new StreamWriter(f /*System.Text.Encoding.Unicode*/);
        writer.WriteLine(strData);
        writer.Close();
    }

    void ReadData()
    {
        if (!File.Exists(Application.persistentDataPath + "/Config.txt"))    //저장된 컨픽 파일이 있다면
        {
            return;
        }
        StreamReader sr = new StreamReader(Application.persistentDataPath + "/Config.txt");
        string[] lines = sr.ReadToEnd().Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            
            if (lines[i].Length <= 1) 
            {
                continue;
            }
            string[] cutString = lines[i].Split('}');
            string keyS = cutString[0].Substring(1, cutString[0].Length - 1);
            string valueS = cutString[1].Substring(1, cutString[1].Length - 1);
            m_Data.Add(keyS, valueS);
        }
    }

    public string GetValues(string key) //외부에서 값을 받아감
    {
        string str;
        if (m_Data.TryGetValue(key, out str))
        {
            return str;
        }
        else
        {
            Debug.Log("NotFound");
            return "NotFound";
        }
    }

    public void SaveValues(string key, string value)    //외부에서 값을 변경
    {
        m_Data[key] = value;
        Debug.Log("키" + key + "의 값 저장 : " + value);
    }

    public void SaveDatas() //외부에서 데이터를 저장.
    {
        string strDatas = "";
        foreach (KeyValuePair<string, string> pair in m_Data)
        {
            strDatas += "{" + pair.Key + "}";
            strDatas += "{" + pair.Value + "}\n";
        }
        WriteData(strDatas);
        Debug.Log("데이터를 저장했습니다." + strDatas);
    }

    override protected void OnAwake()
    {
        base.OnAwake();
        ReadData();

        if (m_Data.Count == 0)
        {
            CreateDefaultData();
        }
    }

    protected override void OnStart()
    {
        base.OnStart();
    }

    void Update()
    {
        
    }
}
