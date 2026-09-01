using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AircraftType { Plane, Helicopter }

public abstract class Aircraft : MonoBehaviour
{
    public AircraftType type;
    public float maxFuel = 45f;
    public float speed = 0.5f;

    [HideInInspector] public float fuel;
    [HideInInspector] public bool isVisible;
    [HideInInspector] public bool isLanded;
    [HideInInspector] public bool isCrashed;

    public FuelBar fuelBarUI;
    public OffScreenIndicator indicatorUI;

    protected List<Vector2> path = new List<Vector2>();
    protected int pathIndex = 1;
    protected bool hasPath = false;
    protected Vector2 lastDir;
    protected float facingOffset = -90f;
    protected bool hasEnteredView = false;
    protected bool wasInside = false;

    protected SpriteRenderer sr;
    protected Collider2D col;
    protected List<UnityEngine.GameObject> pathDots = new List<UnityEngine.GameObject>();
    protected static Sprite dotSprite;

    protected virtual void Awake()
    {
        Configure();
        fuel = maxFuel;
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb) rb.bodyType = RigidbodyType2D.Kinematic;
        SetInitialHeading();
        if (dotSprite == null)
        {
            Texture2D t = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            t.SetPixel(0, 0, Color.white);
            t.Apply();
            dotSprite = Sprite.Create(t, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        }
    }

    protected virtual void Configure() { }

    protected virtual void SetInitialHeading()
    {
        Vector2 toCenter = ((Vector2)Vector2.zero - (Vector2)transform.position).normalized;
        float scatter = Random.Range(-0.35f, 0.35f);
        lastDir = Quaternion.Euler(0f, 0f, scatter * Mathf.Rad2Deg) * toCenter;
    }

    public void BeginDrag()
    {
        path.Clear();
        path.Add(transform.position);
        pathIndex = 1;
        hasPath = true;
        UpdatePathLine();
    }

    public void AddPathPoint(Vector2 p)
    {
        if (path.Count == 0) path.Add(transform.position);
        if (Vector2.Distance(path[path.Count - 1], p) < 0.15f) return;
        path.Add(p);
        UpdatePathLine();
    }

    public void EndDrag() { }

    protected virtual void Update()
    {
        if (GameManager.IsPaused) return;
        if (isLanded || isCrashed) return;
        ConsumeFuel();
        Move();
        UpdateVisibility();
        UpdatePathLine();
        UpdateUI();
    }

    protected void Move()
    {
        if (hasPath && pathIndex < path.Count)
        {
            Vector2 pos = transform.position;
            Vector2 target = path[pathIndex];
            Vector2 step = target - pos;
            float dist = step.magnitude;
            float move = speed * Time.deltaTime;
            if (move >= dist)
            {
                transform.position = target;
                pathIndex++;
                if (pathIndex < path.Count) lastDir = (path[pathIndex] - (Vector2)transform.position).normalized;
                else if (dist > 0.0001f) lastDir = step / dist;
            }
            else
            {
                Vector2 dir = step / dist;
                transform.position = pos + dir * move;
                lastDir = dir;
            }
        }
        else
        {
            transform.position = (Vector2)transform.position + lastDir * speed * Time.deltaTime;
        }
        float ang = Mathf.Atan2(lastDir.y, lastDir.x) * Mathf.Rad2Deg + facingOffset;
        transform.rotation = Quaternion.Euler(0f, 0f, ang);
    }

    protected void ConsumeFuel()
    {
        fuel -= Time.deltaTime;
        if (fuel <= 0f) { fuel = 0f; Crash(); }
    }

    protected void Crash()
    {
        if (isCrashed || isLanded) return;
        isCrashed = true;
        GameManager.Instance.GameOver("Топливо закончилось (" + (type == AircraftType.Plane ? "самолёт" : "вертолёт") + ")");
    }

    protected void UpdateVisibility()
    {
        Camera cam = Camera.main;
        if (cam == null) return;
        Vector3 vp = cam.WorldToViewportPoint(transform.position);
        bool inside = vp.x > 0f && vp.x < 1f && vp.y > 0f && vp.y < 1f && vp.z > 0f;
        isVisible = inside;
        if (inside)
        {
            hasEnteredView = true;
            wasInside = true;
        }
        else if (wasInside && hasEnteredView)
        {
            lastDir = -lastDir;
            hasPath = false;
            pathIndex = path.Count;
            wasInside = false;
        }
        if (indicatorUI) indicatorUI.SetVisible(!isVisible, transform.position);
    }

    protected void UpdateUI()
    {
        if (fuelBarUI) fuelBarUI.Set(transform.position, fuel / maxFuel);
    }

    protected void UpdatePathLine()
    {
        if (!hasPath || path.Count < 2)
        {
            for (int i = 0; i < pathDots.Count; i++) pathDots[i].SetActive(false);
            return;
        }
        while (pathDots.Count < path.Count)
        {
            GameObject g = new GameObject("PathDot");
            SpriteRenderer dsr = g.AddComponent<SpriteRenderer>();
            dsr.sprite = dotSprite;
            dsr.color = new Color(1f, 0.9f, 0.2f, 1f);
            dsr.sortingOrder = 3;
            pathDots.Add(g);
        }
        for (int i = 0; i < pathDots.Count; i++)
        {
            if (i < path.Count && i >= pathIndex)
            {
                pathDots[i].SetActive(true);
                pathDots[i].transform.position = (Vector3)path[i];
                pathDots[i].transform.localScale = new Vector3(0.05f, 0.05f, 1f);
            }
            else
            {
                pathDots[i].SetActive(false);
            }
        }
    }

    protected void OnDestroy()
    {
        for (int i = 0; i < pathDots.Count; i++) if (pathDots[i] != null) UnityEngine.Object.Destroy(pathDots[i]);
        pathDots.Clear();
    }

    public virtual void Land()
    {
        if (isLanded || isCrashed) return;
        isLanded = true;
        hasPath = false;
        for (int i = 0; i < pathDots.Count; i++) pathDots[i].SetActive(false);
        if (fuelBarUI) fuelBarUI.gameObject.SetActive(false);
        if (indicatorUI) indicatorUI.gameObject.SetActive(false);
        StartCoroutine(LandingAnimation());
    }

    protected IEnumerator LandingAnimation()
    {
        float dur = 1f;
        float t = 0f;
        Vector3 dir = (lastDir.magnitude > 0.0001f ? (Vector3)lastDir : Vector3.right).normalized;
        Vector3 startScale = transform.localScale;
        while (t < dur)
        {
            t += Time.deltaTime;
            float k = t / dur;
            transform.position += dir * speed * Time.deltaTime;
            transform.localScale = startScale * (1f - k);
            yield return null;
        }
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        Aircraft a = other.GetComponent<Aircraft>();
        if (a != null && a != this)
        {
            if (isLanded || isCrashed || a.isLanded || a.isCrashed) return;
            if (isVisible && a.isVisible)
                GameManager.Instance.GameOver("Столкновение летательных аппаратов");
        }
    }
}
