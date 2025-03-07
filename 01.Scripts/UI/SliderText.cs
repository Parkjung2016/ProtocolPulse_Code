using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderText : MonoBehaviour
{
    private TextMeshProUGUI _tmp;
    private Slider _slider;

    public void OnSliderValueChanged(Single value)
    {
        if (_tmp == null)
        {
            _tmp = GetComponent<TextMeshProUGUI>();
            _slider = transform.parent.GetComponent<Slider>();
        }

        _tmp.SetText($"{value}/{_slider.maxValue}");
    }
}