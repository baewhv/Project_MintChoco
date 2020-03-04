using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicManager : SingletonMonoBehaviour<MicManager>
{
    public static float MicLoudness;
    public static float MicLoudnessinDecibels;

    float minValue;
    Dictionary<string, string> MicData;

    private string _device;

    public float GetMinValue()
    {
        return minValue;
    }

    public void SetMinValue(float min)
    {
        minValue = min;
    }

    public void SaveData()
    {
        MicData["Value"] = GetMinValue().ToString();
        string strDatas;
        Util.PackingDictionary(MicData, out strDatas);
        Debug.Log(strDatas);
        var dm = GameObject.Find("DataManager").GetComponent<DataManager>();
        dm.SaveValues(dm.m_OtherData, "MicMinValue", strDatas);
        dm.SaveDatas(dm.m_OtherData,dm.otherConfigTXT);
    }
    void ParseData()
    {
        var dm = GameObject.Find("DataManager").GetComponent<DataManager>();
        if (Util.ParseDictionary(dm.GetValues(dm.m_OtherData, "MicMinValue"), out MicData))
        {
            SetMinValue(float.Parse(MicData["Value"]));
        }
    }

    //mic initialization
    public void InitMic()
    {
        if (_device == null)
        {
            _device = Microphone.devices[0];
        }
        _clipRecord = Microphone.Start(_device, true, 999, 44100);
        //_isInitialized = true;
    }

    public void StopMicrophone()
    {
        Microphone.End(_device);
        //_isInitialized = false;
    }


    AudioClip _clipRecord;
    AudioClip _recordedClip;
    int _sampleWindow = 128;

    //get data from microphone into audioclip
    float MicrophoneLevelMax()
    {
        float levelMax = 0;
        float[] waveData = new float[_sampleWindow];
        int micPosition = Microphone.GetPosition(null) - (_sampleWindow + 1); // null means the first microphone
        if (micPosition < 0) return 0;
        _clipRecord.GetData(waveData, micPosition);
        // Getting a peak on the last 128 samples
        for (int i = 0; i < _sampleWindow; i++)
        {
            float wavePeak = waveData[i] * waveData[i];
            if (levelMax < wavePeak)
            {
                levelMax = wavePeak;
            }
        }
        return levelMax;
    }

    //get data from microphone into audioclip
    float MicrophoneLevelMaxDecibels()
    {

        float db = 20 * Mathf.Log10(Mathf.Abs(MicLoudness));

        return db;
    }

    public float FloatLinearOfClip(AudioClip clip)
    {
        StopMicrophone();

        _recordedClip = clip;

        float levelMax = 0;
        float[] waveData = new float[_recordedClip.samples];

        _recordedClip.GetData(waveData, 0);
        // Getting a peak on the last 128 samples
        for (int i = 0; i < _recordedClip.samples; i++)
        {
            float wavePeak = waveData[i] * waveData[i];
            if (levelMax < wavePeak)
            {
                levelMax = wavePeak;
            }
        }
        return levelMax;
    }

    public float DecibelsOfClip(AudioClip clip)
    {
        StopMicrophone();

        _recordedClip = clip;

        float levelMax = 0;
        float[] waveData = new float[_recordedClip.samples];

        _recordedClip.GetData(waveData, 0);
        // Getting a peak on the last 128 samples
        for (int i = 0; i < _recordedClip.samples; i++)
        {
            float wavePeak = waveData[i] * waveData[i];
            if (levelMax < wavePeak)
            {
                levelMax = wavePeak;
            }
        }

        float db = 20 * Mathf.Log10(Mathf.Abs(levelMax));

        return db;
    }

    public float GetDecibels()
    {
        float db = ((Mathf.Log10(Mathf.Abs(MicLoudness)) + 9) / 9) * 100;

        return db ;
    }



    void Update()
    {
        // levelMax equals to the highest normalized value power 2, a small number because < 1
        // pass the value to a static var so we can access it from anywhere
        MicLoudness = MicrophoneLevelMax();
        MicLoudnessinDecibels = MicrophoneLevelMaxDecibels();
    }

    //bool _isInitialized;
    // start mic when scene starts
    void OnEnable()
    {
        InitMic();
        //_isInitialized = true;
    }

    override protected void OnStart()
    {
        base.OnStart();
        ParseData();
    }

    

    void OnDestroy()
    {
        StopMicrophone();
    }



}

