using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class ImageLoader
{
    public ImageLoader(string path, out Sprite[] spr)
    {
        
        string folderPath = Application.streamingAssetsPath + "/Image/";
        string[] filePaths = Directory.GetFiles(folderPath, "*.png");
        
        spr = null;
    }


}
