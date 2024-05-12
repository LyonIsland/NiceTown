using UnityEngine;

public class ClearHistory : MonoBehaviour
{
    // 调用这个方法来销毁所有子物体
    public void DestroyAllChildren()
    {
        // 创建一个临时数组来存储所有子物体
        GameObject[] children = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i).gameObject;
        }

        // 遍历所有子物体并销毁它们
        foreach (GameObject child in children)
        {
            Destroy(child);
        }
    }
}
