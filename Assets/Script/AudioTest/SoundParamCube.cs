using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public class SoundParamCube : MonoBehaviour
{
    public int _band;
    public float _startScale, _scaleMultiplier;
    public bool _useBuffer;
    PostProcessProfile profile;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        profile = GetComponent<PostProcessVolume>().profile;
        if (_useBuffer)
        {
            if (!float.IsNaN(LoopbackAudio.Instance._audioBandBuffer[_band]))
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, (LoopbackAudio.Instance._audioBandBuffer[_band] * _scaleMultiplier) + _startScale);
            }
        }
        else
        {
            if (!float.IsNaN(LoopbackAudio.Instance._audioBand[_band]))
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, (LoopbackAudio.Instance._audioBand[_band] * _scaleMultiplier) + _startScale);
            }
        }
    }
}
