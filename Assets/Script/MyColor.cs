using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyColor : MonoBehaviour
{
    public static Color MyRed = new Color(1.0f, 0.5f, 0.5f, 0.22f);
    public static Color MyYellow = new Color(1.0f, 1.0f, 0.5f, 0.22f);
    public static Color MyGreen = new Color(0.5f, 1.0f, 0.5f, 0.22f);
    public static Color MyCyan = new Color(1.0f, 1.0f, 0.5f, 0.22f);
    public static Color MyBlue = new Color(0.5f, 0.5f, 1.0f, 0.22f);
    public static Color MyMagenta = new Color(1.0f, 0.5f, 1.0f, 0.22f);
    public static Color[] colors = new Color[] { MyRed, MyYellow, MyGreen, MyCyan, MyBlue, MyMagenta };
}
