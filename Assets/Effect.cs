using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    float startTime = 0;
    public float removeTime = 0;
    bool isOn;

    public void SetTimer(int i)
    {
        startTime = 0;
        removeTime = i;
        isOn = true;
    }
    // Start is called before the first frame update
    void Awake()
    {
        isOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOn)
        {

            startTime += Time.deltaTime;

            if(removeTime <= startTime)
            {
                isOn = false;
                EffectManager.Instance.RemoveEffect(this);
            }
        }
    }
}
