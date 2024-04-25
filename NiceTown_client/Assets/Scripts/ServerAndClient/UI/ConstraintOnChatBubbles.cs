using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ConstraintOnChatBubbles : MonoBehaviour
{
    private RectTransform rectTransform;
    public float maxWidthPercentage = 0.7f; // ����Ȱٷֱ�
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
        // ��ȡ���չʾ���Ŀ��
        ScrollView = GameObject.Find("ChatOutput Panel");
        float parentWidth = ScrollView.GetComponent<RectTransform>().rect.width;
        Debug.Log("chat panel width is " + parentWidth);
        // ���������
        float maxWidth = parentWidth * maxWidthPercentage;
        // Ӧ�������
        rectTransform.sizeDelta = new Vector2(Mathf.Min(rectTransform.sizeDelta.x, maxWidth), rectTransform.sizeDelta.y);
        Debug.Log("now the panel width is "+rectTransform.sizeDelta);
    }
}

