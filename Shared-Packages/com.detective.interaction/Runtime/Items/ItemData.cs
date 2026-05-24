using UnityEngine;

[CreateAssetMenu(fileName = "YeniItemData", menuName = "Items/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Temel Bilgiler")]
    [SerializeField] private string itemID;
    [SerializeField] private string itemAdi;

    [Header("Gorseller")]
    [SerializeField] private Sprite hotbarIkonu;

    [Header("Oyun Özellikleri")]
    public bool DelilMi;

    [Header("Yerleştirme")]
    [SerializeField] private bool yerlestirilebilirMi = true;

    [Header("Prefablar")]
    [Tooltip("Dunyaya atildiginda veya elde gosterildiginde kullanilacak world prefab.")]
    [SerializeField] private GameObject worldPrefab;

    [Header("Stack Ayarlari")]
    [SerializeField] private bool stacklenebilirMi = false;
    [SerializeField] private int maxStack = 1;

    public string ItemID => itemID;
    public string ItemAdi => itemAdi;
    public Sprite HotbarIkonu => hotbarIkonu;
    public GameObject WorldPrefab => worldPrefab;
    public bool StacklenebilirMi => stacklenebilirMi;
    public int MaxStack => Mathf.Max(1, maxStack);

    public bool YerlestirilebilirMi => yerlestirilebilirMi;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(itemID))
            itemID = name.Trim();

        if (string.IsNullOrWhiteSpace(itemAdi))
            itemAdi = name.Trim();

        if (!stacklenebilirMi)
            maxStack = 1;
        else if (maxStack < 1)
            maxStack = 1;
    }
#endif
}