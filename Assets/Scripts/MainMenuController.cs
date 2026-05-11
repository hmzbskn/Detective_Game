using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string oyunSahnesiAdi = "SampleScene";

    public void YeniOyun()
    {
        SceneManager.LoadScene(oyunSahnesiAdi);
    }

    public void DevamEt()
    {
        SceneManager.LoadScene(oyunSahnesiAdi);
    }

    public void Cikis()
    {
        Application.Quit();
        Debug.Log("Oyun kapatıldı");
    }
}