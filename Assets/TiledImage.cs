using UnityEngine;
using UnityEngine.UI;

public class TiledImage : MonoBehaviour
{
    public RawImage rawImage; // 在Unity编辑器中关联RawImage组件

    public float originalTileWidth = 100f; // 贴图的原始长度
    public float blankPixelWidth = 10f; // 空白像素的宽度

    void Update()
    {
        // 获取材质
        Material material = rawImage.material;

        // 计算调整后的贴图长度，以保持比例
        float adjustedTileWidth = originalTileWidth + blankPixelWidth;

        // 计算Tiling的x值，以重复贴图
        float tilingX = rawImage.rectTransform.rect.width / adjustedTileWidth;

        // 设置Tiling
        material.mainTextureScale = new Vector2(tilingX, 1f);
    }
}
