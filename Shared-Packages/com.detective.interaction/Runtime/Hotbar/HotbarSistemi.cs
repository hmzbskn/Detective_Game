using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class HotbarSistemi : MonoBehaviour
{
    [Header("Envanter Açıkken Kapatılacak Oyuncu Kontrolleri")]
    [SerializeField] private PlayerInput oyuncuInput;
    [SerializeField] private MonoBehaviour[] envanterAcikkenKapatilacakScriptler;

    [Header("Envanter Açıkken Gizlenecek UI Objeleri")]
    [SerializeField] private GameObject crosshairUI;

    [Header("Referanslar")]
    [SerializeField] private OyuncuEsyaTutucu esyaTutucu;
    [SerializeField] private IncelemeSistemi incelemeSistemi;

    [Header("Hotbar UI (HB0-HB4)")]
    [SerializeField] private HotbarSlotUI[] hotbarSlotUIleri = new HotbarSlotUI[5];

    [Header("Envanter UI (Üst 2 satır = 10 slot)")]
    [SerializeField] private EnvanterSlotUI[] envanterSlotUIleri = new EnvanterSlotUI[10];

    [Header("UI Panelleri")]
    [SerializeField] private GameObject envanterMenusuUI;

    [Header("Hotbar Taşıma Yuvaları")]
    [SerializeField] private RectTransform hotbarKapsayici;
    [SerializeField] private RectTransform hotbarNormalSlot;
    [SerializeField] private RectTransform hotbarEnvanterSlot;

    [Header("İmleç Ayarları")]
    [SerializeField] private bool envanterAcikkenImlecAcilsin = true;

    [Header("Mouse Scroll Hotbar Ayarları")]
    [SerializeField] private bool mouseScrollIleHotbarSecimiAktif = true;
    [SerializeField] private bool envanterAcikkenScrollKapatilsin = true;

    [Tooltip("Scroll değerinin algılanması için gereken minimum eşik.")]
    [SerializeField] private float mouseScrollEsigi = 0.01f;

    [Tooltip("Scroll ile slot değiştirdikten sonra tekrar değiştirmek için beklenecek süre. Düşük değer = daha hızlı scroll.")]
    [SerializeField] private float mouseScrollBeklemeSuresi = 0.15f;

    [Tooltip("Scroll yönünü tersine çevirmek için aç.")]
    [SerializeField] private bool mouseScrollYonuTers = false;

    private EnvanterDeposu depo;
    private EnvanterPanelDurumu envanterPanel;

    private InventoryItemStack[] hotbarSlotlari;
    private InventoryItemStack[] envanterSlotlari;

    private int aktifHotbarIndex = 0;

    private float sonMouseScrollZamani = -999f;

    private int suruklenenGlobalSlotIndex = -1;

    private void Awake()
    {
        depo = new EnvanterDeposu(SlotAdresleme.HotbarAdet, SlotAdresleme.EnvanterAdet);
        hotbarSlotlari = depo.HotbarSlotlari;
        envanterSlotlari = depo.EnvanterSlotlari;

        envanterPanel = new EnvanterPanelDurumu(
            envanterMenusuUI,
            crosshairUI,
            hotbarKapsayici,
            hotbarNormalSlot,
            hotbarEnvanterSlot,
            oyuncuInput,
            envanterAcikkenKapatilacakScriptler,
            envanterAcikkenImlecAcilsin
        );

        for (int i = 0; i < envanterSlotUIleri.Length; i++)
        {
            if (envanterSlotUIleri[i] != null)
                envanterSlotUIleri[i].Baslat(this, i);
        }

        for (int i = 0; i < hotbarSlotUIleri.Length; i++)
        {
            if (hotbarSlotUIleri[i] != null)
                hotbarSlotUIleri[i].Baslat(this, i);
        }
    }

    private void OnEnable()
    {
        if (esyaTutucu != null)
            esyaTutucu.EsyaDunyayaBirakildi += EsyaDunyayaBirakildiginda;
    }

    private void OnDisable()
    {
        if (esyaTutucu != null)
            esyaTutucu.EsyaDunyayaBirakildi -= EsyaDunyayaBirakildiginda;
    }

    private void Start()
    {
        if (envanterMenusuUI != null)
            envanterMenusuUI.SetActive(false);

        envanterPanel.BaslangicYuvasinaTasi();

        SlotuSec(0);
        TumUIyiGuncelle();
    }

    private void Update()
    {
        GirdileriOku();
    }

    public List<ItemInstanceData> HotbarVeEnvanterdekiDNADelilInstancelariniGetir()
    {
        List<ItemInstanceData> sonuc = new List<ItemInstanceData>();

        DNADelilInstancelariniSlotlardanTopla(hotbarSlotlari, sonuc);
        DNADelilInstancelariniSlotlardanTopla(envanterSlotlari, sonuc);

        return sonuc;
    }

    public bool AktifHotbarIteminiDNAInstanceIleDegistir(ItemData beklenenEskiItem, ItemData yeniItem, DNAData dnaData)
    {
        if (beklenenEskiItem == null || yeniItem == null || dnaData == null)
            return false;

        if (hotbarSlotlari == null)
            return false;

        if (aktifHotbarIndex < 0 || aktifHotbarIndex >= hotbarSlotlari.Length)
            return false;

        InventoryItemStack aktifStack = hotbarSlotlari[aktifHotbarIndex];

        if (aktifStack == null || aktifStack.BosMu())
            return false;

        if (!aktifStack.AyniItemMi(beklenenEskiItem))
            return false;

        ItemInstanceData yeniDNAInstance = new ItemInstanceData(yeniItem, dnaData);

        bool aktifSlottaTekAdetVar = aktifStack.Adet <= 1;

        if (!aktifSlottaTekAdetVar)
        {
            bool bosYerVar =
                depo.IlkBosHotbarBul() != -1 ||
                depo.IlkBosEnvanterBul() != -1;

            if (!bosYerVar)
            {
                Debug.LogWarning("DNA alınamadı: DNA'lı kulak çöpünü koyacak boş hotbar/envanter slotu yok.");
                return false;
            }
        }

        aktifStack.BirAdetAzalt();

        bool dnaInstanceEklendi = ItemInstanceEkle(yeniDNAInstance);

        if (!dnaInstanceEklendi)
        {
            Debug.LogWarning("DNA alındı fakat DNA'lı kulak çöpü envantere eklenemedi.");
            return false;
        }

        if (yeniItem.DelilMi && EvidenceManager.Instance != null)
            EvidenceManager.Instance.Ekle(yeniItem);

        AktifEliGuncelle();
        TumUIyiGuncelle();

        return true;
    }

    private void DNADelilInstancelariniSlotlardanTopla(InventoryItemStack[] slotlar, List<ItemInstanceData> sonuc)
    {
        if (slotlar == null)
            return;

        for (int i = 0; i < slotlar.Length; i++)
        {
            InventoryItemStack stack = slotlar[i];

            if (stack == null || stack.BosMu())
                continue;

            ItemInstanceData instance = stack.TekAdetlikInstanceOlustur();

            if (instance == null || instance.ItemData == null)
                continue;

            if (!instance.ItemData.DelilMi)
                continue;

            if (instance.DNAData == null)
                continue;

            sonuc.Add(instance);
        }
    }

    public bool EsyayiHotbaraEkleVeSec(EldeTutulabilirObje dunyaObjesi)
    {
        if (dunyaObjesi == null)
            return false;

        ItemInstanceData instanceData = dunyaObjesi.ItemInstanceDataGetir();

        if (instanceData == null || instanceData.ItemData == null)
        {
            Debug.LogWarning("Pickup objesinde ItemData yok.");
            return false;
        }

        ItemData itemData = instanceData.ItemData;

        if (instanceData.RuntimeVerisiVarMi)
        {
            bool eklendi = ItemInstanceEkle(instanceData);

            if (eklendi)
            {
                Destroy(dunyaObjesi.gameObject);
                TumUIyiGuncelle();
                return true;
            }

            Debug.Log("Hotbar ve envanter dolu.");
            return false;
        }

        if (itemData.StacklenebilirMi)
        {
            int stackIndex = depo.StacklenebilirSlotBul(hotbarSlotlari, itemData);
            if (stackIndex != -1)
            {
                hotbarSlotlari[stackIndex].BirAdetEkle(itemData);
                Destroy(dunyaObjesi.gameObject);
                TumUIyiGuncelle();
                return true;
            }

            stackIndex = depo.StacklenebilirSlotBul(envanterSlotlari, itemData);
            if (stackIndex != -1)
            {
                envanterSlotlari[stackIndex].BirAdetEkle(itemData);
                Destroy(dunyaObjesi.gameObject);
                TumUIyiGuncelle();
                return true;
            }
        }

        if (hotbarSlotlari[aktifHotbarIndex].BosMu())
        {
            hotbarSlotlari[aktifHotbarIndex].Ayarla(itemData, 1);
            Destroy(dunyaObjesi.gameObject);
            SlotuSec(aktifHotbarIndex);
            TumUIyiGuncelle();
            return true;
        }

        int bosHotbarIndex = depo.IlkBosHotbarBul();
        if (bosHotbarIndex != -1)
        {
            hotbarSlotlari[bosHotbarIndex].Ayarla(itemData, 1);
            Destroy(dunyaObjesi.gameObject);
            SlotuSec(bosHotbarIndex);
            TumUIyiGuncelle();
            return true;
        }

        int bosEnvanterIndex = depo.IlkBosEnvanterBul();
        if (bosEnvanterIndex != -1)
        {
            envanterSlotlari[bosEnvanterIndex].Ayarla(itemData, 1);
            Destroy(dunyaObjesi.gameObject);
            TumUIyiGuncelle();
            return true;
        }

        Debug.Log("Hotbar ve envanter dolu.");
        return false;
    }

    public void SlotuSec(int index)
    {
        if (index < 0 || index >= hotbarSlotlari.Length)
            return;

        aktifHotbarIndex = index;

        ItemInstanceData secilenInstance = null;
        InventoryItemStack aktifStack = hotbarSlotlari[aktifHotbarIndex];

        if (aktifStack != null && !aktifStack.BosMu())
            secilenInstance = aktifStack.TekAdetlikInstanceOlustur();

        if (esyaTutucu != null)
        {
            if (secilenInstance == null)
                esyaTutucu.EldekiniGizle();
            else
                esyaTutucu.SlotEsyasiniEldeGoster(secilenInstance);
        }

        TumUIyiGuncelle();
    }

    private void SonrakiHotbarSlotunaGec()
    {
        if (hotbarSlotlari == null || hotbarSlotlari.Length == 0)
            return;

        int yeniIndex = aktifHotbarIndex + 1;

        if (yeniIndex >= hotbarSlotlari.Length)
            yeniIndex = 0;

        SlotuSec(yeniIndex);
    }

    private void OncekiHotbarSlotunaGec()
    {
        if (hotbarSlotlari == null || hotbarSlotlari.Length == 0)
            return;

        int yeniIndex = aktifHotbarIndex - 1;

        if (yeniIndex < 0)
            yeniIndex = hotbarSlotlari.Length - 1;

        SlotuSec(yeniIndex);
    }

    public void SlotlariYerDegistir(int kaynakGlobalIndex, int hedefGlobalIndex)
    {
        if (kaynakGlobalIndex == hedefGlobalIndex)
            return;

        InventoryItemStack kaynakStack = depo.GlobalStackGetir(kaynakGlobalIndex);
        InventoryItemStack hedefStack = depo.GlobalStackGetir(hedefGlobalIndex);

        if (kaynakStack == null || hedefStack == null)
            return;

        if (kaynakStack.BosMu())
            return;

        if (depo.StackleriBirlestir(kaynakStack, hedefStack))
        {
            AktifEliGuncelle();
            TumUIyiGuncelle();
            return;
        }

        depo.GlobalStackAta(kaynakGlobalIndex, hedefStack);
        depo.GlobalStackAta(hedefGlobalIndex, kaynakStack);

        AktifEliGuncelle();
        TumUIyiGuncelle();
    }

    public void GlobalSlottakiEsyayiDunyayaAt(int globalIndex)
    {
        InventoryItemStack stack = depo.GlobalStackGetir(globalIndex);
        if (stack == null || stack.BosMu())
            return;

        ItemInstanceData atilacakInstance = stack.TekAdetlikInstanceOlustur();
        ItemData itemData = stack.ItemData;

        bool aktifHotbarEsyasiMi = SlotAdresleme.HotbarIndexiMi(globalIndex) &&
            SlotAdresleme.HotbarLocalIndex(globalIndex) == aktifHotbarIndex;

        stack.BirAdetAzalt();

        if (aktifHotbarEsyasiMi)
            AktifEliGuncelle();

        if (esyaTutucu != null)
        {
            if (atilacakInstance != null)
                esyaTutucu.ItemInstanceDunyayaAt(atilacakInstance);
            else
                esyaTutucu.ItemDatayiDunyayaAt(itemData);
        }

        TumUIyiGuncelle();
    }

    /// <summary>
    /// Global slottaki eşyayı dünyaya atmadan, sadece kaynağı boşaltır. Delil tahtaya asıldığında
    /// (IDelilBirakmaHedefi.DeliliBirak başarılı döndüğünde) hotbar/envanter slot UI'ları bunu çağırır.
    /// </summary>
    public void GlobalSlotuBosalt(int globalIndex)
    {
        InventoryItemStack stack = depo.GlobalStackGetir(globalIndex);

        if (stack == null || stack.BosMu())
            return;

        bool aktifHotbarEsyasiMi = SlotAdresleme.HotbarIndexiMi(globalIndex) &&
            SlotAdresleme.HotbarLocalIndex(globalIndex) == aktifHotbarIndex;

        stack.Temizle();

        if (aktifHotbarEsyasiMi)
            AktifEliGuncelle();

        TumUIyiGuncelle();
    }

    /// <summary>
    /// Global slottaki eşyanın tam runtime instance verisini (DNA/fotoğraf verisi dahil) döndürür,
    /// slotu değiştirmez. Tahtaya delil bırakma gibi salt-okunur amaçlar için kullanılır.
    /// </summary>
    public ItemInstanceData GlobalSlottakiInstanceOlustur(int globalIndex)
    {
        InventoryItemStack stack = depo.GlobalStackGetir(globalIndex);

        if (stack == null || stack.BosMu())
            return null;

        return stack.TekAdetlikInstanceOlustur();
    }

    public bool SlottaEsyaVarMi(int envanterIndex)
    {
        if (envanterIndex < 0 || envanterIndex >= envanterSlotlari.Length)
            return false;

        return !envanterSlotlari[envanterIndex].BosMu();
    }

    public Sprite SlottakiIkonuGetir(int envanterIndex)
    {
        if (envanterIndex < 0 || envanterIndex >= envanterSlotlari.Length)
            return null;

        if (envanterSlotlari[envanterIndex].BosMu())
            return null;

        return envanterSlotlari[envanterIndex].IkonGetir();
    }

    public int SlottakiAdediGetir(int envanterIndex)
    {
        if (envanterIndex < 0 || envanterIndex >= envanterSlotlari.Length)
            return 0;

        return envanterSlotlari[envanterIndex].BosMu() ? 0 : envanterSlotlari[envanterIndex].Adet;
    }

    public bool HotbarSlottaEsyaVarMi(int hotbarIndex)
    {
        if (hotbarIndex < 0 || hotbarIndex >= hotbarSlotlari.Length)
            return false;

        return !hotbarSlotlari[hotbarIndex].BosMu();
    }

    public Sprite HotbarSlottakiIkonuGetir(int hotbarIndex)
    {
        if (hotbarIndex < 0 || hotbarIndex >= hotbarSlotlari.Length)
            return null;

        if (hotbarSlotlari[hotbarIndex].BosMu())
            return null;

        return hotbarSlotlari[hotbarIndex].IkonGetir();
    }

    public int HotbarSlottakiAdediGetir(int hotbarIndex)
    {
        if (hotbarIndex < 0 || hotbarIndex >= hotbarSlotlari.Length)
            return 0;

        return hotbarSlotlari[hotbarIndex].BosMu() ? 0 : hotbarSlotlari[hotbarIndex].Adet;
    }

    public bool EnvanterAcikMi()
    {
        return envanterPanel.Acik;
    }

    public void EnvanteriAc()
    {
        envanterPanel.Ac();
        TumUIyiGuncelle();
    }

    public void EnvanteriKapat()
    {
        envanterPanel.Kapat();
        TumUIyiGuncelle();
    }

    public void EnvanteriAcKapatDisaridan()
    {
        EnvanteriAcKapat();
    }

    public void SuruklemeBaslat(int globalIndex)
    {
        suruklenenGlobalSlotIndex = globalIndex;
    }

    public void SuruklemeBitir()
    {
        suruklenenGlobalSlotIndex = -1;
    }

    public int AktifSuruklenenSlotIndex()
    {
        return suruklenenGlobalSlotIndex;
    }

    public bool ItemVarMi(ItemData item)
    {
        if (item == null)
            return false;

        for (int i = 0; i < hotbarSlotlari.Length; i++)
        {
            if (hotbarSlotlari[i] != null &&
                !hotbarSlotlari[i].BosMu() &&
                hotbarSlotlari[i].AyniItemMi(item))
            {
                return true;
            }
        }

        for (int i = 0; i < envanterSlotlari.Length; i++)
        {
            if (envanterSlotlari[i] != null &&
                !envanterSlotlari[i].BosMu() &&
                envanterSlotlari[i].AyniItemMi(item))
            {
                return true;
            }
        }

        return false;
    }

    public bool ItemdenBirAdetAzalt(ItemData azaltilacakItem)
    {
        if (azaltilacakItem == null)
            return false;

        for (int i = 0; i < hotbarSlotlari.Length; i++)
        {
            InventoryItemStack stack = hotbarSlotlari[i];

            if (stack == null || stack.BosMu())
                continue;

            if (!stack.AyniItemMi(azaltilacakItem))
                continue;

            stack.BirAdetAzalt();

            if (i == aktifHotbarIndex)
                AktifEliGuncelle();

            TumUIyiGuncelle();
            return true;
        }

        for (int i = 0; i < envanterSlotlari.Length; i++)
        {
            InventoryItemStack stack = envanterSlotlari[i];

            if (stack == null || stack.BosMu())
                continue;

            if (!stack.AyniItemMi(azaltilacakItem))
                continue;

            stack.BirAdetAzalt();

            TumUIyiGuncelle();
            return true;
        }

        return false;
    }

    public bool ItemEkle(ItemData itemData, int adet = 1)
    {
        if (itemData == null || adet <= 0)
            return false;

        int kalanAdet = adet;

        if (itemData.StacklenebilirMi)
        {
            while (kalanAdet > 0)
            {
                int stackIndex = depo.StacklenebilirSlotBul(hotbarSlotlari, itemData);

                if (stackIndex != -1)
                {
                    hotbarSlotlari[stackIndex].BirAdetEkle(itemData);
                    kalanAdet--;
                    continue;
                }

                stackIndex = depo.StacklenebilirSlotBul(envanterSlotlari, itemData);

                if (stackIndex != -1)
                {
                    envanterSlotlari[stackIndex].BirAdetEkle(itemData);
                    kalanAdet--;
                    continue;
                }

                int bosHotbarIndex = depo.IlkBosHotbarBul();
                if (bosHotbarIndex != -1)
                {
                    int eklenecekMiktar = Mathf.Min(kalanAdet, itemData.MaxStack);
                    hotbarSlotlari[bosHotbarIndex].Ayarla(itemData, eklenecekMiktar);
                    kalanAdet -= eklenecekMiktar;
                    continue;
                }

                int bosEnvanterIndex = depo.IlkBosEnvanterBul();
                if (bosEnvanterIndex != -1)
                {
                    int eklenecekMiktar = Mathf.Min(kalanAdet, itemData.MaxStack);
                    envanterSlotlari[bosEnvanterIndex].Ayarla(itemData, eklenecekMiktar);
                    kalanAdet -= eklenecekMiktar;
                    continue;
                }

                TumUIyiGuncelle();
                return false;
            }

            TumUIyiGuncelle();
            return true;
        }

        for (int i = 0; i < kalanAdet; i++)
        {
            int bosHotbarIndex = depo.IlkBosHotbarBul();
            if (bosHotbarIndex != -1)
            {
                hotbarSlotlari[bosHotbarIndex].Ayarla(itemData, 1);
                continue;
            }

            int bosEnvanterIndex = depo.IlkBosEnvanterBul();
            if (bosEnvanterIndex != -1)
            {
                envanterSlotlari[bosEnvanterIndex].Ayarla(itemData, 1);
                continue;
            }

            TumUIyiGuncelle();
            return false;
        }

        TumUIyiGuncelle();
        return true;
    }

    public bool ItemInstanceEkle(ItemInstanceData instanceData)
    {
        if (instanceData == null || instanceData.ItemData == null)
            return false;

        if (!instanceData.RuntimeVerisiVarMi)
            return ItemEkle(instanceData.ItemData, 1);

        if (hotbarSlotlari[aktifHotbarIndex].BosMu())
        {
            hotbarSlotlari[aktifHotbarIndex].Ayarla(instanceData);
            SlotuSec(aktifHotbarIndex);
            TumUIyiGuncelle();
            return true;
        }

        int bosHotbarIndex = depo.IlkBosHotbarBul();
        if (bosHotbarIndex != -1)
        {
            hotbarSlotlari[bosHotbarIndex].Ayarla(instanceData);
            TumUIyiGuncelle();
            return true;
        }

        int bosEnvanterIndex = depo.IlkBosEnvanterBul();
        if (bosEnvanterIndex != -1)
        {
            envanterSlotlari[bosEnvanterIndex].Ayarla(instanceData);
            TumUIyiGuncelle();
            return true;
        }

        return false;
    }

    public bool AktifHotbarItemMi(ItemData kontrolEdilecekItem)
    {
        if (kontrolEdilecekItem == null)
            return false;

        if (hotbarSlotlari == null)
            return false;

        if (aktifHotbarIndex < 0 || aktifHotbarIndex >= hotbarSlotlari.Length)
            return false;

        InventoryItemStack aktifStack = hotbarSlotlari[aktifHotbarIndex];

        if (aktifStack == null || aktifStack.BosMu())
            return false;

        return aktifStack.AyniItemMi(kontrolEdilecekItem);
    }

    public bool AktifHotbarIteminiDegistir(ItemData beklenenEskiItem, ItemData yeniItem)
    {
        if (beklenenEskiItem == null || yeniItem == null)
            return false;

        if (hotbarSlotlari == null)
            return false;

        if (aktifHotbarIndex < 0 || aktifHotbarIndex >= hotbarSlotlari.Length)
            return false;

        InventoryItemStack aktifStack = hotbarSlotlari[aktifHotbarIndex];

        if (aktifStack == null || aktifStack.BosMu())
            return false;

        if (!aktifStack.AyniItemMi(beklenenEskiItem))
            return false;

        aktifStack.Ayarla(yeniItem, 1);

        if (yeniItem.DelilMi && EvidenceManager.Instance != null)
            EvidenceManager.Instance.Ekle(yeniItem);

        AktifEliGuncelle();
        TumUIyiGuncelle();

        return true;
    }

    private void GirdileriOku()
    {
        if (incelemeSistemi != null && incelemeSistemi.IncelemeAktifMi)
            return;

        KlavyeGirdileriniOku();
        MouseScrollGirdisiniOku();
    }

    private void KlavyeGirdileriniOku()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.tabKey.wasPressedThisFrame)
            EnvanteriAcKapat();

        if (Keyboard.current.digit1Key.wasPressedThisFrame) SlotuSec(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SlotuSec(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SlotuSec(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) SlotuSec(3);
        if (Keyboard.current.digit5Key.wasPressedThisFrame) SlotuSec(4);
    }

    private void MouseScrollGirdisiniOku()
    {
        if (!mouseScrollIleHotbarSecimiAktif)
            return;

        if (envanterAcikkenScrollKapatilsin && envanterPanel.Acik)
            return;

        if (Mouse.current == null)
            return;

        float scrollY = Mouse.current.scroll.ReadValue().y;

        if (Mathf.Abs(scrollY) <= mouseScrollEsigi)
            return;

        if (Time.unscaledTime - sonMouseScrollZamani < mouseScrollBeklemeSuresi)
            return;

        sonMouseScrollZamani = Time.unscaledTime;

        bool yukariKaydirildi = scrollY > 0f;

        if (mouseScrollYonuTers)
            yukariKaydirildi = !yukariKaydirildi;

        if (yukariKaydirildi)
            OncekiHotbarSlotunaGec();
        else
            SonrakiHotbarSlotunaGec();
    }

    private void EnvanteriAcKapat()
    {
        if (envanterPanel.Acik)
            EnvanteriKapat();
        else
            EnvanteriAc();
    }

    private void EsyaDunyayaBirakildiginda(ItemData itemData)
    {
        if (itemData == null)
            return;

        InventoryItemStack aktifStack = hotbarSlotlari[aktifHotbarIndex];

        if (aktifStack == null || aktifStack.BosMu())
            return;

        if (!aktifStack.AyniItemMi(itemData))
            return;

        aktifStack.BirAdetAzalt();

        AktifEliGuncelle();
        TumUIyiGuncelle();
    }

    private void AktifEliGuncelle()
    {
        if (esyaTutucu == null)
            return;

        InventoryItemStack aktifStack = hotbarSlotlari[aktifHotbarIndex];

        if (aktifStack == null || aktifStack.BosMu())
        {
            esyaTutucu.EldekiniGizle();
            return;
        }

        ItemInstanceData instance = aktifStack.TekAdetlikInstanceOlustur();

        if (instance == null)
            esyaTutucu.EldekiniGizle();
        else
            esyaTutucu.SlotEsyasiniEldeGoster(instance);
    }

    private void TumUIyiGuncelle()
    {
        for (int i = 0; i < hotbarSlotUIleri.Length; i++)
        {
            if (hotbarSlotUIleri[i] == null)
                continue;

            Sprite ikon = HotbarSlottakiIkonuGetir(i);
            int adet = HotbarSlottakiAdediGetir(i);

            hotbarSlotUIleri[i].GuncelleUI(ikon, i == aktifHotbarIndex, adet);
        }

        for (int i = 0; i < envanterSlotUIleri.Length; i++)
        {
            if (envanterSlotUIleri[i] == null)
                continue;

            Sprite ikon = SlottakiIkonuGetir(i);
            int adet = SlottakiAdediGetir(i);

            envanterSlotUIleri[i].GuncelleUI(ikon, adet);
        }
    }
}
