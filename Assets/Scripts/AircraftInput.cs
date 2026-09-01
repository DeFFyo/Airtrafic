using UnityEngine;

public class AircraftInput : MonoBehaviour
{
    private Aircraft dragging;
    private int btn;

    void Update()
    {
        if (GameManager.IsPaused) return;
        bool l = Input.GetMouseButton(0);
        bool r = Input.GetMouseButton(1);
        if (dragging != null)
        {
            if (l == r)
            {
                dragging.EndDrag();
                dragging = null;
            }
            else
            {
                Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                wp.z = 0f;
                dragging.AddPathPoint(wp);
            }
        }
        else
        {
            if (l != r)
            {
                int b = l ? 0 : 1;
                if (Input.GetMouseButtonDown(b))
                {
                    Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    wp.z = 0f;
                    var hit = Physics2D.OverlapPoint(wp);
                    if (hit)
                    {
                        var a = hit.GetComponent<Aircraft>();
                        if (a && !a.isLanded && !a.isCrashed)
                        {
                            dragging = a;
                            btn = b;
                            a.BeginDrag();
                        }
                    }
                }
            }
        }
    }
}
