using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicParamCube : MonoBehaviour
{
    public int _band;
    public float _startScale, _scaleMultiplier;
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
            if (!float.IsNaN(MicManager.Instance._audioBandBuffer[_band]))
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, (MicManager.Instance._audioBandBuffer[_band] * _scaleMultiplier) + _startScale);
            }
        }
        else
        {
            if (!float.IsNaN(MicManager.Instance._audioBand[_band]))
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, (MicManager.Instance._audioBand[_band] * _scaleMultiplier) + _startScale);
            }
        }
    }
}
