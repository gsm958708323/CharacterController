using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodImageBg : RawImage
{
    private Slider _BloodSlider;
    Rect rect = new Rect(0, 0, 1, 1);

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();

        //获取血条
        if (_BloodSlider == null)
            _BloodSlider = transform.parent.GetComponent<Slider>();

        //获取血条的值
        if (_BloodSlider != null)
        {
            if (rect.width != _BloodSlider.maxValue)
            {
                //刷新血条的显示
                rect.width = _BloodSlider.maxValue;
                uvRect = rect;
            }
        }
    }
}
