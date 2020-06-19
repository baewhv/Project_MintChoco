using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

public class DataManager : SingletonMonoBehaviour<DataManager>
{
    public Dictionary<string, string> m_OptionData = new Dictionary<string, string>();
    public Dictionary<string, string> m_FurnitureData = new Dictionary<string, string>();
    public Dictionary<string, string> m_PresentGroupData = new Dictionary<string, string>();
    public Dictionary<string, string> m_PresentInfoData = new Dictionary<string, string>();
    public Dictionary<string, string> m_OtherData = new Dictionary<string, string>();
    public string optionConfigTXT = @"\Config.txt";
    public string otherConfigTXT = @"\ETC.txt";
    public string furnitureConfigTXT = @"\Config_furniture.txt";
    public string PresentGroupConfigTXT = @"\Config_PresentGroup.txt";
    public string PresentInfoConfigTXT = @"\Config_PresentInfo.txt";
    LogUI m_massege;

    public void CreateDefaultData()
    {
        CheckData(m_OptionData, "TwippyLogAddress", @"C:\Users\YourName\AppData\Roaming\Twippy\userdata");
        CheckData(m_OptionData, "TwipWssAddress", "AAAAAAAAAA");
        CheckData(m_OptionData, "ToonationAddress", "wss://toonation.kr/");
        CheckData(m_OptionData, "SelectSpotLight", "False");
        CheckData(m_OptionData, "UseTwippyTXTDonate", "False");
        CheckData(m_OptionData, "UseTwipSocketDonate", "True");
        SaveDatas(m_OptionData, optionConfigTXT);
        CheckData(m_OtherData, "MicMinValue", "OptionCategory:Mic&Value:40");
        SaveDatas(m_OtherData, otherConfigTXT);

        CheckData(m_FurnitureData, "Furniture_Background", "Background&0&0&1920&1080&false&0&0&0&1920&1080&false&0&배경&0");
        CheckData(m_FurnitureData, "Furniture_Lamp", "Lamp&0&0&1920&1080&False&1&0&0&1920&1080&False&1&램프&1");
        CheckData(m_FurnitureData, "Furniture_Bed", "Bed&0&0&1920&1080&False&2&0&0&1920&1080&False&2&침대&1");
        CheckData(m_FurnitureData, "Furniture_Drawer", "Drawer&0&0&1920&1080&False&3&0&0&1920&1080&False&3&침대협탁&1");
        CheckData(m_FurnitureData, "Furniture_TV", "TV&553&280&775&474&false&4&553&280&775&474&false&4&모니터&0");
        CheckData(m_FurnitureData, "Furniture_Chair", "Chair&-379&-96&720&880&False&5&-379&-96&752&880&False&5&의자&1");
        CheckData(m_FurnitureData, "Furniture_Desk", "Desk&-406&-689&1099&764&true&0&-406&-689&1099&764&true&0&책상&1");
        CheckData(m_FurnitureData, "Furniture_ChatArea", "ChatArea&550&-225&780&540&true&1&550&-225&780&540&true&1&채팅창&1");
        CheckData(m_FurnitureData, "Furniture_StandMic", "StandMic&-746&-161&450&450&True&2&-746&-161&450&450&True&2&마이크&1");
        SaveDatas(m_FurnitureData, furnitureConfigTXT);

        CheckData(m_PresentGroupData, "presentGroup_0", "name:기본 도네1&price:1000&isSame:False&extractCount:1&addPrice:1000&groupData:0&presentList:0,1,2,3,4,5,6");
        CheckData(m_PresentGroupData, "presentGroup_1", "name:기본 도네2&price:5000&isSame:False&extractCount:1&addPrice:1000&groupData:0&presentList:7,8,9,10,11,12");
        CheckData(m_PresentGroupData, "presentGroup_2", "name:기본 도네3&price:10000&isSame:False&extractCount:1&addPrice:10000&groupData:0&presentList:13,14,15,16");
        CheckData(m_PresentGroupData, "presentGroup_3", "name:기본 도네4&price:50000&isSame:False&extractCount:1&addPrice:0&groupData:0&presentList:20");
        CheckData(m_PresentGroupData, "presentGroup_4", "name:기본 도네5&price:100000&isSame:False&extractCount:1&addPrice:0&groupData:0&presentList:21");
        CheckData(m_PresentGroupData, "presentGroup_5", "name:기본 도네4&price:1001&isSame:True&extractCount:1&addPrice:0&groupData:0&presentList:17,18");
        CheckData(m_PresentGroupData, "presentGroup_6", "name:기본 도네5&price:1002&isSame:True&extractCount:1&addPrice:0&groupData:0&presentList:19");
        CheckData(m_PresentGroupData, "presentGroup_7", "name:기본 도네5&price:1000000&isSame:True&extractCount:1&addPrice:0&groupData:0&presentList:22");

        SaveDatas(m_PresentGroupData, PresentGroupConfigTXT);
        //SaveDatas(m_PresentInfoData, PresentInfoConfigTXT);
    }

    void CheckData(Dictionary<string, string> data, string key, string value)
    {
        if (!data.ContainsKey(key))
        {
            data.Add(key, value);
        }
        else if(data[key] == "")
        {
            data[key] = value;
        }
    }

    void WriteData(string strData, string saveTXT)
    {
        FileStream f = new FileStream(Application.persistentDataPath + saveTXT, FileMode.Create, FileAccess.Write);
        StreamWriter writer = new StreamWriter(f /*System.Text.Encoding.Unicode*/);
        writer.WriteLine(strData);
        writer.Close();
        f.Close();
    }

    void ReadData(Dictionary<string, string> data, string saveTXT)
    {
        if (!File.Exists(Application.persistentDataPath + saveTXT))    //저장된 컨픽 파일이 없다면
        {
            return;
        }
        StreamReader sr = new StreamReader(Application.persistentDataPath + saveTXT);
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
            data.Add(keyS, valueS);
        }
        sr.Close();
    }

    public string GetValues(Dictionary<string, string> data, string key) //외부에서 값을 받아감
    {
        string str;
        if (data.TryGetValue(key, out str))
        {
            return str;
        }
        else
        {
            return "NotFound";
        }
    }

    public void SaveValues(Dictionary<string, string> data, string key, string value)    //외부에서 값을 변경
    {
        data[key] = value;
        //Debug.Log("키" + key + "의 값 저장 : " + value);
    }
    public void AddValues(Dictionary<string, string> data, string key, string value)    //외부에서 값을 변경
    {
        if (!data.ContainsKey(key))
        {
            data.Add(key, value);
        }
        //Debug.Log("키" + key + "의 값 저장 : " + value);
    }

    public void DeleteValue(Dictionary<string, string> data, string key)
    {
        if (data.ContainsKey(key))
        {
            data.Remove(key);
        }
    }

    public void SaveDatas(Dictionary<string, string> data, string saveTXT) //외부에서 데이터를 저장.
    {
        string strDatas = "";
        foreach (KeyValuePair<string, string> pair in data)
        {
            strDatas += "{" + pair.Key + "}";
            strDatas += "{" + pair.Value + "}\n";
        }
        WriteData(strDatas, saveTXT);
        //Debug.Log("데이터를 저장했습니다." + strDatas);
    }

    public Sprite SetStreamingImage(string fileName)   //이미 옮겨진 파일.
    {
        string path = Application.persistentDataPath + "/Image/" + fileName + ".png";
        if (File.Exists(path))
        {
            byte[] pngBytes = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(pngBytes);
            Sprite spr = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            spr.name = Path.GetFileNameWithoutExtension(path);
            //Debug.Log("Success to load sprite" + spr.name);
            return spr;
        }
        else
        {
            Debug.Log("fail to load sprite"  + path);
            return null;
        }
    }
    public Sprite[] GetAllImage()   //모든 스트리밍 에셋 이미지 파일 수집.
    {
        if (!Directory.Exists(Application.streamingAssetsPath + "/Image"))
        {
            Directory.CreateDirectory(Application.streamingAssetsPath + "/Image");
        }

        string[] filePaths = Directory.GetFiles(Application.streamingAssetsPath + "/Image", "*.png");

        Sprite[] spr = new Sprite[filePaths.Length];
        for (int i = 0; i < filePaths.Length; i++)
        {
            byte[] pngBytes = File.ReadAllBytes(filePaths[i]);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(pngBytes);
            spr[i] = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            spr[i].name = Path.GetFileNameWithoutExtension(filePaths[i]);
        }

        return spr;
    }
    public bool CopySprite(string fileName) //이미지 관련 - 스프라이트 복사
    {
        if (!Directory.Exists(Application.persistentDataPath + "/Image"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Image");
        }
        if (File.Exists(Application.persistentDataPath + "/Image/" + fileName + ".png"))
        {
            return false;
        }
        File.Copy(Application.streamingAssetsPath + "/Image/" + fileName + ".png", Application.persistentDataPath + "/Image/" + fileName + ".png", false);
        return true;
    }
    public bool DeleteSprite(string fileName)//이미지 관련 - 스프라이트 제거
    {
        if (File.Exists(Application.persistentDataPath + fileName + ".png"))
        {
            return false;
        }
        File.Delete(Application.persistentDataPath + "/Image/" + fileName + ".png");
            return true;
    }


    override protected void OnAwake()
    {
        base.OnAwake();
        ReadData(m_OptionData, optionConfigTXT);
        ReadData(m_FurnitureData, furnitureConfigTXT);
        ReadData(m_OtherData, otherConfigTXT);
        ReadData(m_PresentGroupData, PresentGroupConfigTXT);
        //ReadData(m_PresentInfoData, PresentInfoConfigTXT);

        CreateDefaultData();
    }
}
