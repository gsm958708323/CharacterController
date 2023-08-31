using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[System.Serializable]
public class EntityInfo
{
    public int EntityType;
    public int Col = 50;
    public Transform EntityPos;
    public List<GameObject> PrefabList = new List<GameObject>();
    [HideInInspector]
    public Dictionary<Vector3, GameObject> EntityDict = new Dictionary<Vector3, GameObject>();
}

public class ModelTest : MonoBehaviour
{
    public InputField inputField;
    public Button exButton;
    public float spacing = 1f;
    public List<EntityInfo> cfgList = new List<EntityInfo>();

    void Start()
    {
        exButton.onClick.AddListener(delegate ()
        {
            ExecuteCommand(inputField.text);
        });
    }

    void ExecuteCommand(string command)
    {
        //将command用","分割,并打印数组
        string[] cmdArr = command.Split(',');
        foreach (string cmd in cmdArr)
        {
            Debug.Log(cmd);
        }

        int cmdType = int.Parse(cmdArr[0]);
        if (cmdType == 1)
        {
            int type = cmdArr.Length > 1 ? int.Parse(cmdArr[1]) : 1;
            var param = cmdArr.Length > 2 ? int.Parse(cmdArr[2]) : -1;
            CreateModel(type, param);
        }
        else if (cmdType == 2)
        {
            int type = cmdArr.Length > 1 ? int.Parse(cmdArr[1]) : 1;
            var param = cmdArr.Length > 2 ? int.Parse(cmdArr[2]) : -1;
            DestroyModel(type, param);
        }
        else if (cmdType == 3)
        {
            int type = cmdArr.Length > 2 ? int.Parse(cmdArr[1]) : 1;
            PlayRandomAnim(type);
        }
    }

    private void PlayRandomAnim(int type = 1)
    {
        var cfg = cfgList.Find(x => x.EntityType == type);
        if (cfg == null)
        {
            return;
        }

        foreach (var item in cfg.EntityDict)
        {
            Animator animator = item.Value.GetComponent<Animator>();
            if (animator == null)
            {
                continue;
            }
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            // 随机选择一个动画片段进行播放
            int randomIndex = Random.Range(0, clips.Length);
            animator.Play(clips[randomIndex].name);
        }
    }

    void CreateModel(int type, int args)
    {
        var count = args == -1 ? 100 : args;
        var cfg = cfgList.Find(x => x.EntityType == type);
        if (cfg == null)
        {
            return;
        }

        int row = cfg.Col;
        int col = 1;
        Vector3 startPos = new Vector3(-row / 2f * spacing, 0, -col / 2f * spacing);
        List<GameObject> prefabList = cfg.PrefabList;
        Transform parent = cfg.EntityPos;
        Dictionary<Vector3, GameObject> dict = cfg.EntityDict;

        for (int i = 0; i < count; i++)
        {
            var index = Random.Range(0, prefabList.Count);
            var prefab = prefabList[index];
            int x = i % row;
            int y = i / row;
            Vector3 pos = startPos + new Vector3(x * spacing, 0, y * spacing);
            if (!dict.ContainsKey(pos))
            {
                GameObject obj = Instantiate(prefab, parent);
                obj.name = "Model " + i;
                obj.SetActive(true);
                obj.transform.localPosition = pos;
                dict.Add(pos, obj);
            }
        }
    }

    void DestroyModel(int type, int args)
    {
        var cfg = cfgList.Find(x => x.EntityType == type);
        if (cfg == null)
        {
            return;
        }

        var dict = cfg.EntityDict;
        int count = args == -1 ? dict.Count: args;

        var waitDel = new List<Vector3>();
        foreach (var item in dict)
        {
            if(waitDel.Count < count)
            {
                waitDel.Add(item.Key);
            }
        }
        foreach (var key in waitDel)
        {
            GameObject obj = dict[key];
            Destroy(obj);
            dict.Remove(key);
        }
    }
}