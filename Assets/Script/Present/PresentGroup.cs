using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PresentGroup
{
    public enum GroupData
    {
        Default,
        AddObject
    }
    Dictionary<string, string> d_presentGroup = new Dictionary<string, string>();

    public string m_name;   //옵션용 네임.
    public int m_price;     //가격
    public bool m_isSame;   //같은 가격에 내놓을지?
    public int[] m_presentList; //어떤 상품을 가지고 있는지
    public int m_extractCount; //몇개나 쏟을건지?
    public int m_addPrice;  //비용에 따라 추가적으로 소환할 것인가?
    public GroupData m_GroupData;   //그룹의 정보는?
    public string m_key;

    public void InitData(string key, Dictionary<string, string> data)
    {
        m_key = key;
        m_name = data["name"];
        m_price = int.Parse(data["price"]);
        m_isSame = bool.Parse(data["isSame"]);
        m_extractCount = int.Parse(data["extractCount"]);
        m_addPrice = int.Parse(data["addPrice"]);
        m_GroupData = (GroupData)int.Parse(data["groupData"]);

        string[] listdata = data["presentList"].Split(',');
        m_presentList = new int[listdata.Length];
        for(int i = 0; i < m_presentList.Length; i++)
        {
            m_presentList[i] = int.Parse(listdata[i]);
        }
    }

    public PresentGroup(string key, Dictionary<string, string> data)
    {
        InitData(key, data);
    }

    public string ReturnValue()
    {
        string n = "&";
        string s = ":";

        string presents = "";
        for (int i = 0; i < m_presentList.Length; i++)
        {
            if(i != 0)
            {
                presents += ",";
            }
            presents += m_presentList[i].ToString();
        }

        string data = "name" + s + m_name + n +
                    "price" + s + m_price.ToString() + n +
                    "isSame" + s + m_isSame.ToString() + n +
                    "extractCount" + s + m_extractCount.ToString() + n +
                    "addPrice" + s + m_addPrice.ToString() + n +
                    "groupData" + s + ((int)m_GroupData).ToString() + n +
                    "presentList" + s + presents;
        return data;
    }
}
