using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CertainTimer : MonoBehaviour
{
    float startTime = 0;
    public float removeTime = 0;
    bool isOn;

    public void SetTimer(int i)
    {
        if (!isOn)
        {
            startTime = 0;
            removeTime = i;
            isOn = true;
        }
    }

    public void ResetTimer()
    {
        if (isOn)
        {
            startTime = 0;
        }
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

            if (removeTime <= startTime)
            {
                isOn = false;
                gameObject.SetActive(false);
            }
        }
    }
}
