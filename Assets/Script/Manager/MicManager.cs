using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MicManager : SingletonMonoBehaviour<MicManager>
{
    float minValue;
    Dictionary<string, string> MicData;
    public bool isSpotlightMode = false;

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
        //Debug.Log(strDatas);
        DataManager.Instance.SaveValues(DataManager.Instance.m_OtherData, "MicMinValue", strDatas);
        DataManager.Instance.SaveDatas(DataManager.Instance.m_OtherData, DataManager.Instance.otherConfigTXT);


    }
    void ParseData()
    {
        if (Util.ParseDictionary(DataManager.Instance.GetValues(DataManager.Instance.m_OtherData, "MicMinValue"), out MicData))
        {
            SetMinValue(float.Parse(MicData["Value"]));
        }
    }
    //-------------------------------------------------------------------------------------------------------------
    public float sensitivity = 100;
    public float loudness = 0;
    public float pitch = 0;
    AudioSource _audio;

    public float RmsValue;
    public float DBValue;
    public float PitchValue;

    private const int QSamples = 1024;
    private const float RefValue = 0.1f;
    private const float Threshold = 0.02f;

    float[] _samples;
    private float[] _spectrum;

    public bool startMicOnStartup = true;

    public bool stopMicrophoneListener = false;
    public bool startMicrophoneListener = false;

    private bool microphoneListenerOn = false;

    public bool disableOutputSound = false;

    AudioSource src;

    public AudioMixer masterMixer;

    float timeSinceRestart = 0;

    private int loopTime = 10;
    private int micFreq = 44100;
    //-------------------------------------------------------------------------------------------------------------

    float[] _freqBand = new float[8];
    float[] _bandBuffer = new float[8];
    float[] _bufferDecrease = new float[8];
    float[] _freqBandHightest = new float[8];
    public float[] _audioBand = new float[8];
    public float[] _audioBandBuffer = new float[8];
    public float _Amplitude, _AmplitudeBuffer;
    float _AmplitudeHighest;
    private float _fSample;

    private void Start()
    {
        ParseData();
        if (startMicOnStartup)
        {
            RestartMicrophoneListener();
            StartMicrophoneListener();

            _audio = GetComponent<AudioSource>();
            _audio.clip = Microphone.Start(null, true, loopTime, micFreq);
            _audio.loop = true;
            while (!(Microphone.GetPosition(null) > 0))
            {

            }
            _audio.Play();
            _samples = new float[QSamples];
            _spectrum = new float[QSamples];
            _fSample = AudioSettings.outputSampleRate;
        }
    }


    private void Update()
    {
        if (stopMicrophoneListener)
        {
            StopMicrophoneListener();
        }
        if (startMicrophoneListener)
        {
            StartMicrophoneListener();
        }

        stopMicrophoneListener = false;
        startMicrophoneListener = false;

        MicrophoneIntoAudioSource(microphoneListenerOn);

        DisableSound(!disableOutputSound);

        loudness = GetAveragedVolume() * sensitivity;
        GetPitch();

        CheckDelay();
        MakeFrequencyBands();
        BandBuffer();
        CreateAudioBand();
        GetAmplitude();
    }
    void CheckDelay()
    {
        int delay = Microphone.GetPosition(null) - src.timeSamples;
        if (delay < 0)
        {
            delay += 44100;

        }
        if (delay > 4410)
        {
            int setTime = Microphone.GetPosition(null) - 1500;
            if (setTime < 0)
            {
                setTime += loopTime * micFreq;
            }
            src.timeSamples = setTime;
        }
    }

    void BandBuffer()
    {
        for (int g = 0; g < 8; ++g)
        {
            if (_freqBand[g] > _bandBuffer[g])
            {
                _bandBuffer[g] = _freqBand[g];
                _bufferDecrease[g] = 0.005f;
            }
            if (_freqBand[g] < _bandBuffer[g])
            {
                _bandBuffer[g] -= _bufferDecrease[g];
                _bufferDecrease[g] *= 1.2f;
            }
        }
    }
    void MakeFrequencyBands()
    {
        int count = 0;
        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if (i == 7)
            {
                sampleCount += 2;
            }
            for (int J = 0; J < 8; J++)
            {
                average += _spectrum[count] * (count + 1);
                count++;
            }
            average /= count;

            _freqBand[i] = average * 10;
        }
    }
    void CreateAudioBand()
    {
        for (int i = 0; i < 8; i++)
        {
            if (_freqBand[i] > _freqBandHightest[i])
            {
                _freqBandHightest[i] = _freqBand[i];
            }
            _audioBand[i] = (_freqBand[i] / _freqBandHightest[i]);
            _audioBandBuffer[i] = (_bandBuffer[i] / _freqBandHightest[i]);
        }
    }
    void GetAmplitude()
    {
        float _CurrentAplitude = 0;
        float _CurrentAplitudeBuffer = 0;
        for (int i = 0; i < 8; i++)
        {
            _CurrentAplitude += _audioBand[i];
            _CurrentAplitudeBuffer += _audioBandBuffer[i];
        }
        if (_CurrentAplitude > _AmplitudeHighest)
        {
            _AmplitudeHighest = _CurrentAplitude;
        }
        _Amplitude = _CurrentAplitude / _AmplitudeHighest;
        _AmplitudeBuffer = _CurrentAplitudeBuffer / _AmplitudeHighest;
    }

    float GetAveragedVolume()
    {
        float[] data = new float[256];
        float a = 0;
        _audio.GetOutputData(data, 0);
        foreach (float s in data)
        {
            a += Mathf.Abs(s);
        }
        return a / 256;
    }

    void GetPitch()
    {
        GetComponent<AudioSource>().GetOutputData(_samples, 0);

        float sum = 0;
        for (int i = 0; i < QSamples; i++)
        {
            sum += _samples[i] * _samples[i];   //여기서도 제곱.
        }
        RmsValue = Mathf.Sqrt(sum / QSamples);
        DBValue = 20 * Mathf.Log10(RmsValue / RefValue);
        if (DBValue < -160)
        {
            DBValue = -160;
        }
        GetComponent<AudioSource>().GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);
        float maxV = 0;
        var maxN = 0;
        for (int i = 0; i < QSamples; i++)
        {
            if (!(_spectrum[i] > maxV) || !(_spectrum[i] > Threshold))
            {
                continue;
            }
            maxV = _spectrum[i];
            maxN = i;
        }
        float freqN = maxN;
        if (maxN > 0 && maxN < QSamples - 1)
        {
            var dL = _spectrum[maxN - 1] / _spectrum[maxN];
            var dR = _spectrum[maxN + 1] / _spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }
        PitchValue = freqN * (_fSample / 2) / QSamples;
    }

    public void StopMicrophoneListener()
    {
        microphoneListenerOn = false;
        disableOutputSound = false;
        src.Stop();
        src.clip = null;
        Microphone.End(null);
    }

    public void StartMicrophoneListener()
    {
        microphoneListenerOn = true;
        disableOutputSound = true;
        RestartMicrophoneListener();
    }


    public void DisableSound(bool SoundOn)
    {
        float volume = 0;
        if (SoundOn)
        {
            volume = 0.0f;
        }
        else
        {
            volume = -80.0f;
        }

        masterMixer.SetFloat("MasterVolume", volume);
    }

    public void RestartMicrophoneListener()
    {
        src = GetComponent<AudioSource>();
        src.clip = null;

        timeSinceRestart = Time.time;
    }

    void MicrophoneIntoAudioSource(bool MicrophoneListenerOn)
    {
        if (MicrophoneListenerOn)
        {
            if (Time.time - timeSinceRestart > 0.5f && !Microphone.IsRecording(null))
            {
                src.clip = Microphone.Start(null, true, 10, 44100);

                while (!(Microphone.GetPosition(null) > 0))
                {

                }
                src.Play();
            }
        }
    }

    public float GetDecibels()
    {
        return loudness * 100;
    }
}

