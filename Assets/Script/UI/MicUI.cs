using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MicUI : MonoBehaviour
{
    bool isSetting = false;
    bool ControlSlider = false;
    bool ControlText = false;
    [SerializeField] Slider MicSlider = null;   //마이크 볼륨에 따라 표시되는 슬라이더
    [SerializeField] Slider CurrentSlider = null;   //현재 설정된 최저 볼륨 위치
    [SerializeField] InputField CurrentValueInput = null;

    [SerializeField] GameObject SampleCharacter = null;
    [SerializeField] GameObject SampleCharacter2 = null;


    public void SetPannel()
    {
        CurrentSlider.value = MicManager.Instance.GetMinValue();
        CurrentValueInput.text = MicManager.Instance.GetMinValue().ToString();
        isSetting = true;
    }
    public void OutPannel()
    {
        isSetting = false;
    }
    public void _SaveData()
    {
        MicManager.Instance.SetMinValue(float.Parse(CurrentValueInput.text));
        MicManager.Instance.SaveData();
    }

    public void _ChangeSliderValue()
    {
        if (!isSetting || ControlText)
        {
            return;
        }
        ControlSlider = true;
        float _value = CurrentSlider.value;
        CurrentValueInput.text = _value.ToString("N2");
        ControlSlider = false;
    }
    public void _ChangeFieldValue()
    {
        if (!isSetting || ControlSlider)
        {
            return;
        }
        ControlText = true;
        float _value = float.Parse(CurrentValueInput.text);
        if (_value > 100)
        {
            _value = 100;
            CurrentValueInput.text = "100";
        }
        else if(_value < 0)
        {
            _value = 0;
            CurrentValueInput.text = "0";
        }
        CurrentSlider.value = _value;
        ControlText = false;
    }


    void SetSampleCharacter(float _volume)
    {
        if(CurrentSlider.value < _volume)
        {
            SampleCharacter.SetActive(false);
            SampleCharacter2.SetActive(true);
        }
        else
        {
            SampleCharacter.SetActive(true);
            SampleCharacter2.SetActive(false);
        }
    }


    void Update()
    {
        if(gameObject.activeSelf)
        {
            float volume = MicManager.Instance.GetDecibels();
            if(volume < 0)
            {
                volume  = 0;
            }
            else if(volume > 100)
            {
                volume = 100;
            }
            MicSlider.value = volume;
            SetSampleCharacter(volume);
        }
    }
}
