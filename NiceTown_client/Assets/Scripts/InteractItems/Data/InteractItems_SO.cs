using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "InteractItems_SO",menuName ="interact/objects")]
public class InteractItems_SO : ScriptableObject
{
    public static List<InteractItemsInfo> interactableObjects = new List<InteractItemsInfo>();
}

