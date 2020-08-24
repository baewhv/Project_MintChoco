using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinker : SingletonMonoBehaviour<Blinker>
{
    public enum BLINK_TYPE
    {
        Idle = 0,
        Pong,
        Donate,
        Error
    }
    SpriteRenderer sprRenderer;
    BLINK_TYPE CurrentType;
    Color baseColor = new Color(0.33f, 0.33f, 0.33f);
    Color errorColor = new Color(0.78f, 0.01f, 0.01f);
    Color pongColor = new Color(0.01f, 0.01f, 0.78f);
    Color donateColor = new Color(0.01f, 0.78f, 0.01f);
    float startTime = 0.0f;
    float targetTime = 0.2f;

    override protected void OnAwake()
    {
        base.OnAwake();
        sprRenderer = GetComponent<SpriteRenderer>();
        SetIdle();
    }
    override protected void OnStart()
    {
        base.OnStart();
        SetIdle();
    }

    public void SetColor(BLINK_TYPE type)
    {
        startTime = 0.0f;
        CurrentType = type;
        sprRenderer.color = SetTargetColor(type);
    }

    void SetIdle()
    {
        startTime = 0.0f;
        CurrentType = BLINK_TYPE.Idle;
        sprRenderer.color = SetTargetColor(CurrentType);
    }

    Color SetTargetColor(BLINK_TYPE type)
    {
        switch(type)
        {
            case BLINK_TYPE.Donate:
                targetTime = 0.4f;
                return donateColor;
            case BLINK_TYPE.Error:
                targetTime = 0.5f;
                return errorColor;
            case BLINK_TYPE.Pong:
                targetTime = 0.2f;
                return pongColor;
            default:
                return baseColor;
        }
    }

    private void Update()
    {
        if(CurrentType != BLINK_TYPE.Idle)
        {
            startTime += Time.deltaTime;
            if(startTime >= targetTime)
            {
                SetIdle();
            }
        }
    }
}
