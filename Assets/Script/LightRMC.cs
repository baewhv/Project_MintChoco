using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRMC : MonoBehaviour
{
    float TargetPosition = 0;
    RectTransform rt;

    public void OnPointerEnter()
    {
        Debug.Log("Pointer Enter!");
        TargetPosition = 120.0f;
    }
    public void OnPointerExit()
    {
        Debug.Log("Pointer Exit!");
        TargetPosition = -80.0f;
    }

    public void OnNextLight()
    {
        LightManager.Instance.SelectNextLight();
    }

    public void OnPrevLight()
    {
        LightManager.Instance.SelectPrevLight();
    }

    private void Awake()
    {
        TargetPosition = -80.0f;
        rt = (RectTransform)transform;
    }

    void Update()
    {
        rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, new Vector2(rt.anchoredPosition.x, TargetPosition), Time.deltaTime * 2);
    }
}
