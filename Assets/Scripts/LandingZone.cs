using UnityEngine;

public class LandingZone : MonoBehaviour
{
    public AircraftType acceptedType;

    void OnTriggerEnter2D(Collider2D other)
    {
        Aircraft a = other.GetComponent<Aircraft>();
        if (a != null && a.type == acceptedType)
            a.Land();
    }
}
