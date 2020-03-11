using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (AudioSource))]
public class AudioPeer : MonoBehaviour
{
    AudioSource _audioSource;
    float[] _samples = new float[512];
    float[] _freqBand = new float[8];
    float[] _bandBuffer = new float[8];
    float[] _bufferDecrease = new float[8];

    float[] _freqBandHightest = new float[8];
    public static float[] _audioBand = new float[8];
    public static float[] _audioBandBuffer = new float[8];

    public static float _Amplitude, _AmplitudeBuffer;
    float _AmplitudeHighest;


    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        BandBuffer();
        CreateAudioBand();
        GetAmplitude();
    }

    void GetSpectrumAudioSource()
    {
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
    }

    void CreateAudioBand()
    {
        for (int i = 0; i < 8; i ++)
        {
            if(_freqBand[i] > _freqBandHightest [i])
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
        for (int i = 0; i < 8; i ++)
        {
            _CurrentAplitude += _audioBand[i];
            _CurrentAplitude += _audioBandBuffer[i];
        }
        if(_CurrentAplitude > _AmplitudeHighest)
        {
            _AmplitudeHighest = _CurrentAplitude;
        }
        _Amplitude = _CurrentAplitude / _AmplitudeHighest;
        _AmplitudeBuffer = _CurrentAplitudeBuffer / _AmplitudeHighest;
    }

    void BandBuffer()
    {
        for (int g = 0; g < 8; ++g)
        {
            if(_freqBand[g] > _bandBuffer[g])
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
        for (int  i = 0; i  < 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if(i ==  7)
            {
                sampleCount += 2;
            }
            for (int J = 0; J < 8; J++)
            {
                average += _samples[count] * (count +1);
                count++;
            }
            average /= count;

            _freqBand[i] = average * 10;
        }
    }
}
