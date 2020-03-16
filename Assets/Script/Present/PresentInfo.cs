using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PresentInfo
{
    public enum PRESENT_TYPE
    {
        CandyBall,
        Candy,
        ThinCandy,
        Small,
        Small_Macaroon,
        Long,   //
        Wide,   //?
        Big,    //cake
        Huge,
        Long_Coke
            
    }
#if UNITY_EDITOR
    public string name;
#endif
    public PRESENT_TYPE m_PresentType;
    public Sprite m_sprite;
    public Vector3 m_SpritePos;
    public Vector3 m_SpriteRot;
    public Vector3 m_SpriteSize;
}
