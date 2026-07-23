using UnityEngine;

[CreateAssetMenu(fileName = "Yeni Esya", menuName = "Envanter Sistemi/Yeni Eşya Oluştur")]
public class EsyaVerisi : ScriptableObject
{
    [Header("Eşya Detayları")]
    public string esyaAdi = "İsimsiz Eşya";

    // Grafiker resmi getirene kadar bu boş kalabilir
    public Sprite esyaIkonu;

    // İleride buraya: Eşya türü (Silah, İksir), Satış Fiyatı vs. eklenecek.
    // İŞTE YENİ EKLENEN SATIR: Ekranda (elde) belirecek 3D model
    public GameObject esyaPrefab;


    // YENİ EKLENEN SATIR: Bu eşya tahtaya asılabilir mi?
    [Tooltip("Eğer bu bir fotoğraf veya not ise işaretle. Su bardağı falan ise işaretleme!")]
    public bool delilMi = false;
}
