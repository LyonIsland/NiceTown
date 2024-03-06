using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//这个脚本要挂载到每一个可以互动的gameObject上
public class InteractItemsInfo : MonoBehaviour
{
    public GameObject obj;
    bool canInteract;
    public string ID;
}




