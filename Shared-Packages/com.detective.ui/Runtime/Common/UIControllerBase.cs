using UnityEngine;

public abstract class UIControllerBase : MonoBehaviour
{
    [Header("UI Panel")]
    [SerializeField] protected GameObject panel;

    [Header("Ayarlar")]
    [SerializeField] protected bool oyunuDuraklatir = false;
    [SerializeField] protected bool cursorAcikOlmali = true;

    protected bool acikMi = false;

    protected virtual void Start()
    {
        if (panel != null)
            panel.SetActive(false);

        acikMi = false;
    }

    protected virtual void Update()
    {
        if (TetiklemeTusunaBasildiMi())
        {
            TogglePanel();
        }
    }

    protected abstract bool TetiklemeTusunaBasildiMi();

    protected void TogglePanel()
    {
        if (acikMi)
            Kapat();
        else
            Ac();
    }

    protected virtual void Ac()
    {
        acikMi = true;

        if (panel != null)
            panel.SetActive(true);

        Cursor.visible = cursorAcikOlmali;
        Cursor.lockState = cursorAcikOlmali ? CursorLockMode.None : CursorLockMode.Locked;

        if (oyunuDuraklatir)
            Time.timeScale = 0f;
    }

    protected virtual void Kapat()
    {
        acikMi = false;

        if (panel != null)
            panel.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (oyunuDuraklatir)
            Time.timeScale = 1f;
    }

    public bool AcikMi()
    {
        return acikMi;
    }
}