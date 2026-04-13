using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic; // Listeler için gerekli

public class TicaretUIManager : MonoBehaviour
{
    public static TicaretUIManager Instance;

    [Header("UI Referanslari")]
    public GameObject ticaretPaneli;
    public GameObject ticaretSlotPrefab; // Slot prefabý
    
    [Header("Liste Alanlarý")]
    public Transform saticiContent; // SOL Taraf (Satýcýnýn mallarý) -> Eskiden 'satisListesiIcerik'ti
    public Transform oyuncuContent; // SAÐ Taraf (Oyuncunun mallarý) -> YENÝ

    [Header("Veri Referanslari")]
    public List<EsyaTipi> satilacakEsyalar; // NPC'nin sattýðý eþyalar
    public TextMeshProUGUI oyuncuParaText;

    private bool _isOpen = false;
    public bool IsOpen => _isOpen; // Dýþarýdan okumak için property
    private charactercontroller _karakterHareket;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (ticaretPaneli != null) ticaretPaneli.SetActive(false);
    }

    // AÇMA - KAPAMA ÝÞLEMLERÝ
    public void PaneliAc()
    {
        if (ticaretPaneli == null)
        {
            Debug.LogError("HATA: Ticaret Paneli Inspector'da atanmamýþ!");
            return;
        }
        ticaretPaneli.SetActive(true);
        _isOpen = true;

        if (_karakterHareket != null)
        {
            _karakterHareket.hareketEdebilir = true;

            // (Opsiyonel) Eðer karakter kaymaya devam ediyorsa hýzý sýfýrla:
            // _karakterHareket.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        }
        // Mouse'u aç
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 1. Parayý Göster
        ParaGuncelle();

        // 2. Envanter Panelini de Aç
        if (InventoryUIManager.Instance != null)
        {
            InventoryUIManager.Instance.EnvanterAcKapat(true);
        }

        // 3. Ýki Listeyi de Doldur
        ListeleriYenile();
    }

    public void PaneliKapat()
    {
        _isOpen = false;
        ticaretPaneli.SetActive(false);

        // Mouse'u kilitle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    // LÝSTE DOLDURMA ÝÞLEMLERÝ
    public void ListeleriYenile()
    {
        SaticiListesiniDoldur();
        OyuncuListesiniDoldur();
    }

    void SaticiListesiniDoldur()
    {
        // Temizlik
        foreach (Transform child in saticiContent) Destroy(child.gameObject);

        // Satýcý Ürünlerini Diz
        foreach (EsyaTipi tip in satilacakEsyalar)
        {
            EsyaVeriSO veri = InventoryUIManager.Instance.VeriGetir(tip);
            if (veri != null)
            {
                GameObject slot = Instantiate(ticaretSlotPrefab, saticiContent);
                // FALSE = Satýn Alma Modu (Marketteyiz)
                slot.GetComponent<TicaretSlot>().SlotuKur(veri, false);
            }
        }
    }

    public void OyuncuListesiniDoldur()
    {
        // Temizlik
        foreach (Transform child in oyuncuContent) Destroy(child.gameObject);

        // Oyuncunun Envanterine Ulaþ
        OyuncuEnvanter envanter = FindObjectOfType<OyuncuEnvanter>();

        if (envanter != null)
        {
            foreach (var slotVerisi in envanter.hotbarSlotlari)
            {
                // Slot boþ deðilse listeye ekle
                if (slotVerisi.miktar > 0)
                {
                    EsyaVeriSO veri = InventoryUIManager.Instance.VeriGetir(slotVerisi.tip);
                    if (veri != null)
                    {
                        GameObject yeniSlot = Instantiate(ticaretSlotPrefab, oyuncuContent);
                        // TRUE = Satýþ Modu (Kendi çantamýzdayýz)
                        yeniSlot.GetComponent<TicaretSlot>().SlotuKur(veri, true);
                    }
                }
            }
        }
    }

    public void ParaGuncelle()
    {
        KarakterDurum durum = FindObjectOfType<KarakterDurum>();
        if (durum != null && oyuncuParaText != null)
        {
            // MevcutPara property'sini kullanýyoruz
            oyuncuParaText.text = "Para: " + durum.MevcutPara.ToString();
        }
    }
}