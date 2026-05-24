using UnityEngine;

public struct PlacementPose
{
    public bool YuzeyBulunduMu;
    public bool KonulabilirMi;
    public Vector3 Pozisyon;
    public Quaternion Rotasyon;

    public PlacementPose(bool yuzeyBulunduMu, bool konulabilirMi, Vector3 pozisyon, Quaternion rotasyon)
    {
        YuzeyBulunduMu = yuzeyBulunduMu;
        KonulabilirMi = konulabilirMi;
        Pozisyon = pozisyon;
        Rotasyon = rotasyon;
    }
}