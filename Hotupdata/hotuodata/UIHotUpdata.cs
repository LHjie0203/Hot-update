using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHotUpdata : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        actionCancel?.Invoke();
        actionComfirm?.Invoke();
    }
    public Action actionCancel;
    public Action actionComfirm;
    // Update is called once per frame
    void Update()
    {
        
    }
}
