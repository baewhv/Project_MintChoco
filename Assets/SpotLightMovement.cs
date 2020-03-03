using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLightMovement : MonoBehaviour
{
    [SerializeField] float startZ =0;
    float curTime;
    [SerializeField] float angle =0;
    [SerializeField] float rotateSpeed =0;
    // Start is called before the first frame update
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        curTime += Time.deltaTime * rotateSpeed;
        float a = startZ + (Mathf.Sin(curTime) * angle);
        transform.rotation = Quaternion.Euler(0,0,a);
    }
}
