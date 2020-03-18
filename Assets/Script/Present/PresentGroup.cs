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

#if UNITY_EDITOR
    public string name;
#endif
    public int m_price;     //가격
    public bool m_isSame;   //같은 가격에 내놓을지?
    public int[] m_presentList; //어떤 상품을 가지고 있는지
    public int m_extractCount; //몇개나 쏟을건지?
    public int m_addPrice;  //비용에 따라 추가적으로 소환할 것인가?
    public GroupData m_GroupData;   //그룹의 정보는?

   
}
