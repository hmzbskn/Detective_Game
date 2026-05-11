using UnityEngine;

public class DNABaslatButonu : MonoBehaviour
{
    [SerializeField] private GameObject baslatButonu;
    [SerializeField] private GameObject dnaSystem;

    public void DNABaslat()
    {
        Debug.Log("DNA BUTONUNA BASILDI");

        if (baslatButonu != null)
            baslatButonu.SetActive(false);

        if (dnaSystem != null)
        {
            dnaSystem.SetActive(true);
            dnaSystem.SendMessage("MiniGameBaslat", SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            Debug.LogError("DNA SYSTEM ATANMAMIS");
        }
    }
}