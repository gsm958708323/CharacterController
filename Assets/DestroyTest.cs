using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnDestroy() {
        Debug.Log(111);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
