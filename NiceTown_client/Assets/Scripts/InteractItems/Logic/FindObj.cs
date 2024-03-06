using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindObj 
{
    public static void FindAndAddItems()
    {
        GameObject[] items = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject item in items)
        {
            InteractItemsInfo info = item.GetComponent<InteractItemsInfo>();
            if (info != null)
            {
                InteractItems_SO.interactableObjects.Add(info);
                Debug.Log("Add : " + item.name + " with ID : " + info.ID);
            }
        }
    }

}
