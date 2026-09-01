using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Plane planePrefab;
    public Helicopter heliPrefab;
    public Canvas uiCanvas;
    public OffScreenIndicator indicatorPrefab;
    public FuelBar fuelBarPrefab;
    public float startInterval = 6f;
    public float minInterval = 3f;
    public int maxConcurrent = 6;

    private float timer = 2f;
    private float interval;

    void Awake() { interval = startInterval; }

    void Update()
    {
        if (GameManager.IsPaused) return;
        interval = Mathf.Max(minInterval, interval - Time.deltaTime * 0.03f);
        timer -= Time.deltaTime;
        if (timer <= 0f && CountAircraft() < maxConcurrent)
        {
            Spawn();
            timer = interval;
        }
    }

    int CountAircraft()
    {
        int n = 0;
        var all = FindObjectsOfType<Aircraft>();
        for (int i = 0; i < all.Length; i++) if (!all[i].isLanded && !all[i].isCrashed) n++;
        return n;
    }

    void Spawn()
    {
        bool isPlane = Random.value < 0.5f;
        Aircraft prefab = isPlane ? (Aircraft)planePrefab : (Aircraft)heliPrefab;
        Vector2 spawn = RandomOffScreenPoint();
        Aircraft a = Instantiate(prefab, spawn, Quaternion.identity);
        if (fuelBarPrefab && uiCanvas)
        {
            var fb = Instantiate(fuelBarPrefab, uiCanvas.transform);
            a.fuelBarUI = fb;
        }
        if (indicatorPrefab && uiCanvas)
        {
            var ind = Instantiate(indicatorPrefab, uiCanvas.transform);
            a.indicatorUI = ind;
        }
    }

    Vector2 RandomOffScreenPoint()
    {
        Camera cam = Camera.main;
        float halfH = cam.orthographicSize + 1.5f;
        float halfW = cam.aspect * cam.orthographicSize + 1.5f;
        int edge = Random.Range(0, 4);
        if (edge == 0) return new Vector2(Random.Range(-halfW, halfW), -halfH);
        if (edge == 1) return new Vector2(Random.Range(-halfW, halfW), halfH);
        if (edge == 2) return new Vector2(-halfW, Random.Range(-halfH, halfH));
        return new Vector2(halfW, Random.Range(-halfH, halfH));
    }
}
