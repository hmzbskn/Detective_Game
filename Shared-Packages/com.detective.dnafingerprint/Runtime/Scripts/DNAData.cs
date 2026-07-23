using UnityEngine;

public enum DNAOrnekTuru
{
    Bilinmeyen,
    Ceset,
    Supheli
}

[CreateAssetMenu(fileName = "YeniDNA", menuName = "DNA/DNA Data")]
public class DNAData : ScriptableObject
{
    [Header("Kimlik")]
    public string dnaID;
    public string sahibiAdi;

    [Header("Örnek Türü")]
    public DNAOrnekTuru ornekTuru = DNAOrnekTuru.Bilinmeyen;

    [Header("Spektrum")]
    [Tooltip("0-4 arası yoğunluk")]
    public int[] spektrum = new int[10];
}