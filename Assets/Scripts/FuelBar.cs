using UnityEngine;
using UnityEngine.UI;

public class FuelBar : MonoBehaviour
{
    public Image bg;
    public Image fill;
    public float width = 64f;

    void Awake()
    {
        if (bg) { bg.rectTransform.anchorMin = new Vector2(0.5f, 0.5f); bg.rectTransform.anchorMax = new Vector2(0.5f, 0.5f); bg.rectTransform.pivot = new Vector2(0.5f, 0.5f); }
        if (fill) { fill.rectTransform.anchorMin = new Vector2(0, 0.5f); fill.rectTransform.anchorMax = new Vector2(0, 0.5f); fill.rectTransform.pivot = new Vector2(0, 0.5f); }
    }

    public void Set(Vector3 worldPos, float ratio)
    {
        ratio = Mathf.Clamp01(ratio);
        Vector3 sp = Camera.main.WorldToScreenPoint(worldPos);
        sp.y += 42f;
        transform.position = sp;
        if (bg) bg.rectTransform.sizeDelta = new Vector2(width, 8f);
        if (fill)
        {
            fill.rectTransform.anchoredPosition = new Vector2(-width * 0.5f, 0f);
            fill.rectTransform.sizeDelta = new Vector2(width * ratio, 8f);
            fill.color = ratio < 0.3f ? Color.red : Color.green;
        }
    }
}
