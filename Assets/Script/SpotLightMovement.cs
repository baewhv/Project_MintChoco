using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLightMovement : MonoBehaviour
{
    [SerializeField] float startZ =0;
    float curTime;
    [SerializeField] float angle =0;
    [SerializeField] float rotateSpeed =0;
    [SerializeField] GameObject TargetObj = null;
    SpriteRenderer LightSpr;
    
    [SerializeField] bool SelectChangeColor = false;
    [SerializeField] bool SelectChangeRandomColor = false;
    [SerializeField] float TraceLefpTime = 1.0f;
    [SerializeField] Color lightColor = new Color( 1, 1, 1, 0.22f );
    [SerializeField] float ChangeColorTime = 1.0f;
    float ColorCurTime = 0;

    void Awake()
    {
        LightSpr = transform.GetComponentInChildren<SpriteRenderer>();
        LightSpr.color = lightColor;
    }

    int nextColor = 0;
    Color ChangeColor()
    {
        if(ColorCurTime > ChangeColorTime)
        {
            ColorCurTime -= 1;
            if(SelectChangeRandomColor)
            {
                nextColor = Util.Rand(0, 5);
            }
            if (nextColor+1 >= MyColor.colors.Length)
            {
                nextColor = 0;
            }
            else
            {
                nextColor++;
            }
        }
        else
        {
            ColorCurTime += Time.deltaTime;
        }
        
        return Color.Lerp(LightSpr.color,MyColor.colors[nextColor],Time.deltaTime * ChangeColorTime);
    }

    void Update()
    {
        curTime += Time.deltaTime * rotateSpeed;
        float a = startZ + (Mathf.Sin(curTime) * angle);
        transform.rotation = Quaternion.Euler(0,0,a);

        if(TargetObj != null)
        {
            transform.position = new Vector3(Mathf.Lerp(transform.position.x, TargetObj.transform.position.x, Time.deltaTime * TraceLefpTime), transform.position.y, transform.position.z);

        }
        if(SelectChangeColor)
        {
            LightSpr.color = ChangeColor();
            
        }
        else
        {
            LightSpr.color = lightColor;
        }
        
    }
}
