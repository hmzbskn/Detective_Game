using UnityEngine;

[CreateAssetMenu(fileName = "YeniDNA", menuName = "DNA/DNA Data")]
public class DNAData : ScriptableObject
{
    public string dnaID;
    public string sahibiAdi;

    [Tooltip("0-4 arası yoğunluk")]
    public int[] spektrum = new int[10];

    public bool dogruEslesmeMi;
}