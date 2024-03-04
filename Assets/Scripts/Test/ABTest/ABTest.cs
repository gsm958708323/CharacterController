using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ABTest : MonoBehaviour
{
    public GameObject Cube;

    void Start()
    {
    }

    // 设置颜色会生成多个材质球资源
    private void Test()
    {
        var go = Instantiate(Cube);
        // var mat = go.GetComponent<Renderer>().material;
        var color = new Color(Random.Range(0, 1), Random.Range(0, 1), Random.Range(0, 1));
        // mat.SetColor(Shader.PropertyToID("_Color"), color);
        // Destroy(mat);
        // Destroy(go);

        var render = go.GetComponent<Renderer>();
        var propBlock = new MaterialPropertyBlock();
        propBlock.SetColor("_Color", color);
        render.SetPropertyBlock(propBlock);
        Destroy(go);
    }

    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            for (int i = 0; i < 100; i++)
            {
                // Test();
                Test2();
            }
            Debug.Log("测试完成");
        }
    }

    private void Test2()
    {
        var abAsset = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "cube"));
        var prefab = abAsset.LoadAsset<GameObject>("cube");
        // Instantiate(prefab);
        abAsset.Unload(true);
    }
}
