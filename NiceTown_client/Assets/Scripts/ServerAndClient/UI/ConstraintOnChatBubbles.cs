using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ConstraintOnChatBubbles : MonoBehaviour
{
    private RectTransform rectTransform;
    public float maxWidthPercentage = 0.7f; // 最大宽度百分比
    private GameObject ScrollView;

    private void Start()
    {
        ApplyConstraint();
    }
    public void ApplyConstraint()
    {
        rectTransform = GetComponent<RectTransform>();
        UpdateMaxWidth();
    }
    void UpdateMaxWidth()
    {
        if (rectTransform == null)
            return;
        // 获取输出展示区的宽度
        ScrollView = GameObject.Find("ChatOutput Panel");
        float parentWidth = ScrollView.GetComponent<RectTransform>().rect.width;
        Debug.Log("chat panel width is " + parentWidth);
        // 计算最大宽度
        float maxWidth = parentWidth * maxWidthPercentage;
        // 应用最大宽度
        rectTransform.sizeDelta = new Vector2(Mathf.Min(rectTransform.sizeDelta.x, maxWidth), rectTransform.sizeDelta.y);
        Debug.Log("now the panel width is "+rectTransform.sizeDelta);
    }
}

