using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodImage : RawImage
{
    private Slider slider;
    Rect rect = new Rect(0, 0, 1, 1);

    RawImage bgImage;
    Rect rectBg = new Rect(0, 0, 1, 1);

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();

        if (slider == null)
            slider = transform.parent.parent.GetComponent<Slider>();

        if (bgImage == null)
            bgImage = slider.transform.GetChild(0).GetComponent<RawImage>();

        if (slider != null)
        {
            // 填充血条
            if (rect.width != slider.value)
            {
                rect.width = slider.value;
                uvRect = rect;
            }

            // 填充血条背景
            if (bgImage != null && rectBg.width != slider.maxValue)
            {
                rectBg.width = slider.maxValue;
                bgImage.uvRect = rectBg;
            }

        }
    }
}
