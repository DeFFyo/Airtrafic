using UnityEngine;

public class Plane : Aircraft
{
    protected override void Configure()
    {
        type = AircraftType.Plane;
        maxFuel = 45f;
        speed = 0.5f;
    }
}
