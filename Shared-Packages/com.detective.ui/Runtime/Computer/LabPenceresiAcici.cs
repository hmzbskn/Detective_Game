using UnityEngine;

public class LabPenceresiAcici : MonoBehaviour
{
    [Header("Pencere")]
    [SerializeField] private BilgisayarPenceresi labPenceresi;

    [Header("LAB Icerigi")]
    [SerializeField] private RectTransform uygulamaHavuzu;
    [SerializeField] private RectTransform dnaSystem;

    [Header("Reset")]
    [SerializeField] private GameObject dnaSorgulamaKoprusuObjesi;

    public void LabAc()
    {
        if (labPenceresi != null)
            labPenceresi.Ac();

        if (dnaSystem == null || uygulamaHavuzu == null)
            return;

        dnaSystem.SetParent(uygulamaHavuzu, false);

        dnaSystem.anchorMin = Vector2.zero;
        dnaSystem.anchorMax = Vector2.one;
        dnaSystem.pivot = new Vector2(0.5f, 0.5f);

        dnaSystem.offsetMin = Vector2.zero;
        dnaSystem.offsetMax = Vector2.zero;
        dnaSystem.localScale = Vector3.one;

        dnaSystem.gameObject.SetActive(true);

        EkraniSifirla();
    }

    private void EkraniSifirla()
    {
        if (dnaSorgulamaKoprusuObjesi != null)
        {
            dnaSorgulamaKoprusuObjesi.SendMessage(
                "EkraniSifirla",
                SendMessageOptions.DontRequireReceiver
            );

            return;
        }

        if (dnaSystem == null)
            return;

        MonoBehaviour[] butunScriptler = dnaSystem.GetComponentsInChildren<MonoBehaviour>(true);

        for (int i = 0; i < butunScriptler.Length; i++)
        {
            MonoBehaviour script = butunScriptler[i];

            if (script == null)
                continue;

            if (script.GetType().Name == "BilgisayarDNASorgulamaKoprusu")
            {
                script.gameObject.SendMessage(
                    "EkraniSifirla",
                    SendMessageOptions.DontRequireReceiver
                );

                return;
            }
        }
    }
}   