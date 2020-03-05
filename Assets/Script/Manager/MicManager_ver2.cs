using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MicManager_ver2 : MonoBehaviour
{

    private string _device;
    float loudness;
    float mTimer = 0;       //녹음된 시간 -> 계속 리셋해준다.
    float mRefTime = 10;    //녹음 할 시간
    int minFreq;
    int maxFreq;
    int amountSamples = 128;
    float sensitivity = 100;
    [SerializeField]AudioSource source;

    AudioClip clip;

    void GetMicCaps()
    {
        Microphone.GetDeviceCaps(_device, out minFreq, out maxFreq);//Gets the frequency of the device
        if (maxFreq - minFreq < 1)
        {
            minFreq = 0;
            maxFreq = 44100;
        }
    }
    public void StartMicrophone()
    {
        clip = Microphone.Start(_device, true, 999, maxFreq);//Starts recording
        while (!(Microphone.GetPosition(_device) > 0)) { } // Wait until the recording has started
        source.Play(); // Play the audio source!
    }

    void Start()
    {
        source.loop = true; // Set the AudioClip to loop
        source.mute = false; // Mute the sound, we don't want the player to hear it
        _device = Microphone.devices[0];
        GetMicCaps();
    }

    void Update()
    {
        loudness = GetAveragedVolume() * sensitivity;

        if (!Microphone.IsRecording(_device))
        {
            StartMicrophone();
        }
    }

    public void StopMicrophone()
    {
        source.Stop();//Stops the audio
        Microphone.End(_device);//Stops the recording of the device
    }

    float GetAveragedVolume()
    {
        float[] data = new float[amountSamples];
        float a = 0;
        clip.GetData(data, 0);
        foreach (float s in data)
        {
            a += Mathf.Abs(s);
        }
        return a / amountSamples;
    }

}
