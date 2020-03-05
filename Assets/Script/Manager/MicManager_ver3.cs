using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MicManager_ver3 : MonoBehaviour
{
    [SerializeField] private Image _imageSound;
    [SerializeField] private Text TextVol;

    float maxVolume;
    float minVolume = 1;

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
    private float _fSample;


    public bool startMicOnStartup = true;

    public bool stopMicrophoneListener = false;
    public bool startMicrophoneListener = false;

    private bool microphoneListenerOn = false;

    public bool disableOutputSound = false;

    AudioSource src;

    public AudioMixer masterMixer;

    float timeSinceRestart = 0;

    private void Start()
    {
        if(startMicOnStartup)
        {
            RestartMicrophoneListener();
            StartMicrophoneListener();

            _audio = GetComponent<AudioSource>();
            _audio.clip = Microphone.Start(null, true, 10, 44100);
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
        if(stopMicrophoneListener)
        {
            StopMicrophoneListener();
        }
        if(startMicrophoneListener)
        {
            //StartMicrophoneListener();
        }

        stopMicrophoneListener = false;
        startMicrophoneListener = false;

        MicrophoneIntoAudioSource (microphoneListenerOn);

        DisableSound(!disableOutputSound);

        loudness = GetAveragedVolume() * sensitivity;
        GetPitch();

        if(loudness > 5f)
        {
            _imageSound.fillAmount = 1f;
        }
        else
        {
            _imageSound.fillAmount = 0.65f;
        }
        TextVol.text = "vol:" + loudness;

        if(loudness > maxVolume)
        {
            maxVolume = loudness;
        }
        if(loudness != 0)
        if(loudness < minVolume)
          {
            minVolume = loudness;
        }

    }

    float GetAveragedVolume()
    {
        float[] data = new float[256];
        float a = 0;
        _audio.GetOutputData(data, 0);
        foreach(float s in data)
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
            if(!(_spectrum[i] > maxV) || !(_spectrum[i] > Threshold))
            {
                continue;
            }
            maxV = _spectrum[i];
            maxN = i;
        }
        float freqN = maxN;
        if(maxN > 0 && maxN < QSamples -1)
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

    public void DisableSound (bool SoundOn)
    {
        float volume = 0;
        if (SoundOn)
        {
            volume = 0.0f;
        }
        else
        {
            volume = - 80.0f;
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
        if(MicrophoneListenerOn)
        {
            if(Time.time - timeSinceRestart > 0.5f && !Microphone.IsRecording(null))
            { 
                src.clip = Microphone.Start(null, true, 10, 44100);

                while (!(Microphone.GetPosition(null) > 0))
                {

                }
                src.Play();
            }
        }
    }
}

