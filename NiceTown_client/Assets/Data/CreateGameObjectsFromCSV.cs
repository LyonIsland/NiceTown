#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.IO;

[ExecuteInEditMode] // 特性：编辑模式下
public class CreateGameObjectsFromCSV : MonoBehaviour
{
    public string csvFilePath; // CSV 文件路径
    public GameObject bookTablePrefab; // BookTable 的预制体
    public GameObject parentObject; // 用于包含所有创建的对象的父对象

#if UNITY_EDITOR
    private void OnEnable(){
        CreateObjectsFromCSV();
    }
    void CreateObjectsFromCSV()
    {
        if (string.IsNullOrEmpty(csvFilePath) || bookTablePrefab == null || parentObject == null)
        {
            Debug.LogError("CSV 文件路径、预制体或父对象未指定！");
            return;
        }

        // 读取 CSV 文件的每一行
        string[] lines = File.ReadAllLines(csvFilePath);

        // 创建一个父对象来容纳所有创建的对象
        GameObject placeParent = new GameObject("place");

        // 跳过第一行标题行
        for (int i = 1; i < lines.Length; i++)
        {
            // 分割每行数据，假设格式为 ID,Position
            string[] values = lines[i].Split(',');

            if (values.Length >= 2)
            {
                // 解析 ID 和 Position
                string id = values[0];
                string[] position = values[1].Split('_');

                if (position.Length >= 2)
                {
                    float xOriginal = float.Parse(position[0]);
                    float yOriginal = float.Parse(position[1]);

                    // 转换坐标范围
                    float x = xOriginal / 10f; // 缩小10倍
                    float y = yOriginal / -10f; // 缩小10倍

                    // 根据坐标实例化 GameObject
                    GameObject newObj = Instantiate(bookTablePrefab, new Vector3(x, y, 0), Quaternion.identity);
                    newObj.name = id; // 设置 GameObject 名称为 ID

                    // 将新创建的 GameObject 设置为 placeParent 的子对象
                    newObj.transform.parent = placeParent.transform;
                }
                else
                {
                    Debug.LogWarning("Position 数据格式不正确：" + values[1]);
                }
            }
            else
            {
                Debug.LogWarning("CSV 数据格式不正确：" + lines[i]);
            }
        }
    }
#endif
}
