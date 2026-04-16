using UnityEngine;
using System.Collections.Generic;

public class ZulaUIManager : MonoBehaviour
{
    public static ZulaUIManager Instance;

    [Header("UI Bilesenleri")]
    public GameObject zulaPaneli; // Hem oyuncu hem de zula yuvalarýný tutan ana panel

    // YENÝ BÝR PREFAB GEREKECEK (Adým 3)
    public GameObject depoYuvaPrefab; // Týklanabilir depo yuvasý prefabý

    [Header("Konteynerler")]
    public Transform oyuncuYuvaKonteyneri; // Oyuncu envanterinin gösterileceði yer
    public Transform zulaYuvaKonteyneri;  // Zula envanterinin gösterileceði yer

    // O an açýk olan envanterler
    private OyuncuEnvanter _oyuncuEnvanteri;
    private Zula _aktifZula;
    private charactercontroller _karakterHareket;
    // Oluþturulan UI yuvalarýný tutan listeler
    private List<DepoYuvaUI> _oyuncuYuvaUIs = new List<DepoYuvaUI>();
    private List<DepoYuvaUI> _zulaYuvaUIs = new List<DepoYuvaUI>();
    //
    private bool _isOpen = false;
    public bool IsOpen => _isOpen;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (zulaPaneli != null)
            zulaPaneli.SetActive(false); // Baþlangýçta paneli gizle
    }

    // Zula.cs tarafýndan çaðrýlýr
    public void ZulaUI_Ac(OyuncuEnvanter oyuncu, Zula zula)
    {
        _oyuncuEnvanteri = oyuncu;
        _aktifZula = zula;
        _isOpen = true;
        zulaPaneli.SetActive(true);
        // Karakter hareketini durdur ve fareyi aç
        if (CraftingUIManager.Instance != null)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (_karakterHareket != null)
            {
                _karakterHareket.hareketEdebilir = true;

                // (Opsiyonel) Eðer karakter kaymaya devam ediyorsa hýzý sýfýrla:
                // _karakterHareket.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            }
        }

        // Yuvalarý oluþtur ve doldur
        EnvanterleriGuncelle();
    }

    public void ZulaUI_Kapat()
    {
        zulaPaneli.SetActive(false);
        // Karakter hareketine izin ver ve fareyi kilitle
        if (CraftingUIManager.Instance != null)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        _oyuncuEnvanteri = null;
        _aktifZula = null;
        _isOpen = false;
        if (InventoryUIManager.Instance != null)
        {
            InventoryUIManager.Instance.EnvanteriGuncelle();
        }
    }

    void Update()
    {
        if (zulaPaneli.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            ZulaUI_Kapat();
        }
    }

    // Yuvalarý doldurur
    private void EnvanterleriGuncelle()
    {
        // Oyuncu Yuvalarý (Hotbar)
        if (_oyuncuYuvaUIs.Count == 0) // Ýlk açýlýþta bir kez oluþtur
            YuvalariOlustur(_oyuncuEnvanteri.hotbarSlotlari.Length, oyuncuYuvaKonteyneri, _oyuncuYuvaUIs, DepoYuvaUI.EnvanterTipi.Oyuncu);

        // Zula Yuvalarý
        if (_zulaYuvaUIs.Count == 0) // Ýlk açýlýþta bir kez oluþtur
            YuvalariOlustur(_aktifZula.zulaSlotlari.Length, zulaYuvaKonteyneri, _zulaYuvaUIs, DepoYuvaUI.EnvanterTipi.Zula);

        // UI'larý Doldur
        for (int i = 0; i < _oyuncuYuvaUIs.Count; i++)
            GuncelleTekYuva(_oyuncuYuvaUIs[i], _oyuncuEnvanteri.hotbarSlotlari[i]);

        for (int i = 0; i < _zulaYuvaUIs.Count; i++)
            GuncelleTekYuva(_zulaYuvaUIs[i], _aktifZula.zulaSlotlari[i]);
    }

    // UI Yuvalarýný oluþturan yardýmcý metot
    private void YuvalariOlustur(int boyut, Transform konteyner, List<DepoYuvaUI> liste, DepoYuvaUI.EnvanterTipi tip)
    {
        foreach (Transform child in konteyner) Destroy(child.gameObject);
        liste.Clear();
        for (int i = 0; i < boyut; i++)
        {
            GameObject yuvaObj = Instantiate(depoYuvaPrefab, konteyner);
            DepoYuvaUI yuvaUI = yuvaObj.GetComponent<DepoYuvaUI>();
            yuvaUI.Initialize(tip, i); // Yuvaya kimliðini (index) ver
            liste.Add(yuvaUI);
        }
    }

    // Tek bir yuvayý dolduran yardýmcý metot
    private void GuncelleTekYuva(DepoYuvaUI yuvaUI, EnvanterYuvasi yuvaVeri)
    {
        if (yuvaVeri.miktar > 0)
        {
            EsyaVeriSO esyaVerisi = InventoryUIManager.Instance.VeriGetir(yuvaVeri.tip);
            yuvaUI.YuvayiGuncelle(esyaVerisi, yuvaVeri.miktar);
        }
        else
        {
            yuvaUI.Temizle();
        }
    }

    // ***************************************************************
    // TRANSFER MANTIÐI
    // ***************************************************************
    public void YuvaTiklandi(DepoYuvaUI.EnvanterTipi tip, int index)
    {
        if (tip == DepoYuvaUI.EnvanterTipi.Oyuncu)
        {
            // Oyuncudan Zula'ya Transfer
            TransferEt(_oyuncuEnvanteri.hotbarSlotlari, _aktifZula.zulaSlotlari, index);
        }
        else // tip == Zula
        {
            // Zula'dan Oyuncuya Transfer
            TransferEt(_aktifZula.zulaSlotlari, _oyuncuEnvanteri.hotbarSlotlari, index);
        }

        EnvanterleriGuncelle();
        if (InventoryUIManager.Instance != null)
        {
            InventoryUIManager.Instance.EnvanteriGuncelle();
        }
    }

    private void TransferEt(EnvanterYuvasi[] kaynak, EnvanterYuvasi[] hedef, int kaynakIndex)
    {
        EnvanterYuvasi kaynakYuva = kaynak[kaynakIndex];
        if (kaynakYuva.miktar == 0) return; // Boþ yuvaya týklandý

        // 1. Hedefte boþ bir slot ara
        int bosIndex = -1;
        for (int i = 0; i < hedef.Length; i++)
        {
            if (hedef[i].miktar == 0)
            {
                bosIndex = i;
                break;
            }
        }

        // 2. Boþ slot varsa transferi yap
        if (bosIndex != -1)
        {
            hedef[bosIndex] = kaynakYuva;
            kaynak[kaynakIndex] = new EnvanterYuvasi(); // Kaynak yuvayý boþalt
        }
        else
        {
            Debug.Log("Hedef envanter dolu!");
        }
        EnvanterleriGuncelle();
    }
}