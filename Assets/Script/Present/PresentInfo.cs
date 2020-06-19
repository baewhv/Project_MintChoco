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
    public enum PRESENT_DATA
    {
        Default,
        AddObject
    }


#if UNITY_EDITOR
    public string name;
#endif
    public PRESENT_TYPE m_PresentType;
    public Sprite m_sprite;
    public Vector3 m_SpritePos;
    public Vector3 m_SpriteRot;
    public Vector3 m_SpriteSize;
    public PRESENT_DATA m_PresentData;

    public void ReturnColliderSize(out Vector3 Size, out float Mass)
    {

        switch (m_PresentType)
        {
            case PRESENT_TYPE.CandyBall:
                Size = new Vector3(0.25f, 0.25f, 0.2f);
                Mass = 0.1f;
                break;
            case PRESENT_TYPE.Candy:
                Size = new Vector3(1f, 0.4f, 0.2f);
                Mass = 0.1f;
                break;
            case PRESENT_TYPE.ThinCandy:
                Size = new Vector3(1.1f, 0.3f, 0.2f);
                Mass = 0.1f;
                break;
            case PRESENT_TYPE.Small:
                Size = new Vector3(1f, 1f, 0.2f);
                Mass = 1.0f;
                break;
            case PRESENT_TYPE.Small_Macaroon:
                Size = new Vector3(1f, 0.7f, 0.2f);
                Mass = 1.0f;
                break;
            case PRESENT_TYPE.Long:
                Size = new Vector3(1f, 1.7f, 0.2f);
                Mass = 1.5f;
                break;
            case PRESENT_TYPE.Wide:
                Size = new Vector3(2f, 1.4f, 0.2f);
                Mass = 2.0f;
                break;
            case PRESENT_TYPE.Big:
                Size = new Vector3(2f, 2.5f, 0.2f);
                Mass = 3f;
                break;
            case PRESENT_TYPE.Huge:
                Size = new Vector3(1.3f, 1.3f, 0.2f);
                Mass = 5f;
                break;
            case PRESENT_TYPE.Long_Coke:
                Size = new Vector3(0.8f, 2.6f, 0.2f);
                Mass = 1.5f;
                break;
            default:
                Size = Vector3.zero;
                Mass = 0.01f;
                break;
        }
    }
}
