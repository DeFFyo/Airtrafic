using UnityEngine;
using UnityEngine.UI;

public class OffScreenIndicator : MonoBehaviour
{
    private Image img;

    void Awake() { img = GetComponent<Image>(); }

    void Update()
    {
        if (img && img.enabled)
        {
            float a = 0.55f + 0.45f * Mathf.Sin(Time.time * 6f);
            img.color = new Color(1f, 0f, 0f, a);
        }
    }

    public void SetVisible(bool v, Vector3 worldPos)
    {
        if (img) img.enabled = v;
        if (!v) return;
        Vector3 sp = Camera.main.WorldToScreenPoint(worldPos);
        sp.x = Mathf.Clamp(sp.x, 40f, Screen.width - 40f);
        sp.y = Mathf.Clamp(sp.y, 40f, Screen.height - 40f);
        transform.position = sp;
    }
}
