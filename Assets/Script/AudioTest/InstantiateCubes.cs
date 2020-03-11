using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateCubes : MonoBehaviour
{
    public GameObject _sampleCubePrefab;
    GameObject[] _sampleCube = new GameObject[512];
    public int blockCount = 1;
    public float length = 1;

    void Start()
    {
        for (int i = 0; i < (16.0f * blockCount) +1; i++)
        {
            GameObject _instanceSampleCube = Instantiate(_sampleCubePrefab);
            _instanceSampleCube.transform.position = transform.position;
            _instanceSampleCube.transform.parent = transform;
            _instanceSampleCube.name = "SampleCube" + i;
            _instanceSampleCube.transform.position = Vector3.up * length;
            this.transform.eulerAngles = new Vector3(0, 0, (360.0f / (16.0f * blockCount)) * i);
            _sampleCube[i] = _instanceSampleCube;
            if (i > (blockCount * 8.0f) - 1)
            {
                _instanceSampleCube.GetComponent<MicParamCube>()._band = 7 - (i % 8);
            }
            else
            {
                _instanceSampleCube.GetComponent<MicParamCube>()._band = i % 8;
            }
        }
        transform.eulerAngles = Vector3.zero;
        transform.position = Vector3.forward * 2;
    }
}
