using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleOnAmplitude : MonoBehaviour
{
    public float _startScale, _MaxScale;
    public bool _useBuffer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_useBuffer)
        {
            float volume = MicManager.Instance._AmplitudeBuffer;
            if (!float.IsNaN(volume))
            {
                if (volume > 3)
                {
                    volume = 3;
                }
                else if( volume <0)
                {
                    volume = 0;
                }
                volume /= 3;
                float Mix = (volume * _MaxScale) + _startScale;
                transform.localScale = new Vector3(Mix, Mix, 1);
            }
        }
        else
        {
            float volume = MicManager.Instance._Amplitude;
            if (!float.IsNaN(volume))
            {
                if (volume > 3)
                {
                    volume = 3;
                }
                else if (volume < 0)
                {
                    volume = 0;
                }
                volume /= 3;
                float Mix = (volume * _MaxScale) + _startScale;
                transform.localScale = new Vector3(Mix, Mix, 1);
            }
        }
    }
}
