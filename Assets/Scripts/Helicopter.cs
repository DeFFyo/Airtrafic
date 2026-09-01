using UnityEngine;

public class Helicopter : Aircraft
{
    protected override void Configure()
    {
        type = AircraftType.Helicopter;
        maxFuel = 60f;
        speed = 0.5f;
    }
}
