/// <summary>
/// Hotbar ve envanterin ortak "global slot indeksi" adresleme şeması: 0-9 envanter slotlarını,
/// 100-104 hotbar slotlarını temsil eder. Bu şema önceden HotbarSistemi, HotbarSlotUI ve
/// EnvanterSlotUI içinde ayrı ayrı (bazen "100 + index", bazen çıplak index olarak) tekrarlanıyordu;
/// artık tek yerde tanımlı.
/// </summary>
public static class SlotAdresleme
{
    public const int EnvanterBaslangic = 0;
    public const int EnvanterAdet = 10;
    public const int HotbarBaslangic = 100;
    public const int HotbarAdet = 5;

    public static int HotbarGlobalIndex(int hotbarIndex)
    {
        return HotbarBaslangic + hotbarIndex;
    }

    public static int EnvanterGlobalIndex(int envanterIndex)
    {
        return EnvanterBaslangic + envanterIndex;
    }

    public static bool EnvanterIndexiMi(int globalIndex)
    {
        return globalIndex >= EnvanterBaslangic && globalIndex < EnvanterBaslangic + EnvanterAdet;
    }

    public static bool HotbarIndexiMi(int globalIndex)
    {
        return globalIndex >= HotbarBaslangic && globalIndex < HotbarBaslangic + HotbarAdet;
    }

    public static int HotbarLocalIndex(int globalIndex)
    {
        return globalIndex - HotbarBaslangic;
    }
}
