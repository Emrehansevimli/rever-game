using UnityEngine;
using TMPro; // UI bilesenleri icin
using System.Linq;
using System.Collections.Generic;

public class CraftingUIManager : MonoBehaviour
{
    // Singleton (Tekil) eriþim noktasý
    public static CraftingUIManager Instance;

    [Header("UI Bilesenleri")]
    // Uretim Paneli (Parent obje, butun UI'i tutar)
    public GameObject craftingPanel;
    // Secili tarifi gosteren metin
    public TextMeshProUGUI tarifAdiText;
    // Gerekli malzemeleri gosteren metin (veya dinamik liste)
    public TextMeshProUGUI gereksinimlerText;

    [Header("Kontrol Degerleri")]
    private TarifSO[] _mevcutTarifler; // Gelen tarif listesini tutar
    private OyuncuEnvanter _oyuncuEnvanteri;
    private int _mevcutTarifIndex = 0;
    public bool IsPanelOpen => craftingPanel.activeSelf;
 
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Baslangicta paneli gizle
        if (craftingPanel != null)
            craftingPanel.SetActive(false);
    }
    public void CraftingUI_Ac(OyuncuEnvanter envanter, TarifSO[] tarifler)
    {
        if (craftingPanel == null) return;

        _oyuncuEnvanteri = envanter;
        _mevcutTarifler = tarifler;

        if (tarifler.Length == 0)
        {
            Debug.Log("Bu tezgahta uretilebilecek tarif yok.");
            return;
        }

        craftingPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None; // Fareyi serbest birak
        Cursor.visible = true;

        // Ilk tarifi goster
        _mevcutTarifIndex = 0;
        GuncelTarifiGoster();
    }

    public void CraftingUI_Kapat()
    {
        if (craftingPanel == null) return;

        craftingPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked; // Fareyi oyuna kilitle
        Cursor.visible = false;

        // Temizlik
        _oyuncuEnvanteri = null;
    }
    void Update()
    {
        // Yalnizca panel acikken tusa basilabilir
        if (craftingPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                IleriTarifeGec();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                GeriTarifeGec();
            }

            // Uretim tusu
            if (Input.GetKeyDown(KeyCode.Space)) // Ornek olarak Space tusu
            {
                SeciliTarifiUret();
            }

            // Kapatma tusu
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CraftingUI_Kapat();
            }
        }
    }

    private void IleriTarifeGec()
    {
        _mevcutTarifIndex = (_mevcutTarifIndex + 1) % _mevcutTarifler.Length;
        GuncelTarifiGoster();
    }

    private void GeriTarifeGec()
    {
        _mevcutTarifIndex--;
        if (_mevcutTarifIndex < 0)
        {
            _mevcutTarifIndex = _mevcutTarifler.Length - 1;
        }
        GuncelTarifiGoster();
    }
    private void GuncelTarifiGoster()
    {
        if (_mevcutTarifler.Length == 0) return;
        TarifSO seciliTarif = _mevcutTarifler[_mevcutTarifIndex];
        tarifAdiText.text = $"Tarif: {seciliTarif.name}";
        string gereksinimMetni = "Gerekenler:\n";
        bool uretilebilir = true;

        foreach (var gereksinim in seciliTarif.malzemeler)
        {
            // DÜZELTME: Burada da toplam miktarý say.
            int oyuncuMiktari = _oyuncuEnvanteri.hotbarSlotlari
                                   .Where(slot => slot.tip == gereksinim.tip)
                                   .Sum(slot => slot.miktar);

            if (oyuncuMiktari < gereksinim.miktar)
            {
                uretilebilir = false;
                gereksinimMetni += $"<color=red>{gereksinim.tip}: {oyuncuMiktari}/{gereksinim.miktar}</color>\n";
            }
            else
            {
                gereksinimMetni += $"<color=green>{gereksinim.tip}: {oyuncuMiktari}/{gereksinim.miktar}</color>\n";
            }
        }

        gereksinimlerText.text = gereksinimMetni;
        if (uretilebilir)
        {
            gereksinimlerText.text += "\n<color=yellow>Üretilebilir! [BOÞLUK]</color>";
        }
    }

    // TarifKontrolEt ve UretimiGerceklestir metotlarý, önceki adýmdaki gibi kalmalý (diziye uygun).
    private bool TarifKontrolEt(TarifSO tarif)
    {
        foreach (var gereksinim in tarif.malzemeler)
        {
            // DÜZELTME: Yuva sayýsýný deðil, yuvalardaki toplam miktarý say.
            // Önce 'Elma' tipindeki tüm yuvalarý bul (Where), sonra onlarýn miktarýný topla (Sum).
            int oyuncuMiktari = _oyuncuEnvanteri.hotbarSlotlari
                                   .Where(slot => slot.tip == gereksinim.tip)
                                   .Sum(slot => slot.miktar);

            if (oyuncuMiktari < gereksinim.miktar)
            {
                return false; // Malzeme yetersiz.
            }
        }
        return true; // Tüm malzemeler yeterli.
    }

    private void UretimiGerceklestir(TarifSO tarif)
    {
        // 1. Malzemeleri Tüket
        foreach (var gereksinim in tarif.malzemeler)
        {
            int silinecekMiktar = gereksinim.miktar; // Örn: 2 elma silinecek

            // Envanterdeki tüm yuvalarý kontrol et
            for (int i = 0; i < _oyuncuEnvanteri.hotbarSlotlari.Length; i++)
            {
                // Eðer silinecek miktar kalmadýysa döngüden çýk
                if (silinecekMiktar <= 0) break;

                // Yuva doðru tipte mi ve içinde eþya var mý?
                if (_oyuncuEnvanteri.hotbarSlotlari[i].tip == gereksinim.tip && _oyuncuEnvanteri.hotbarSlotlari[i].miktar > 0)
                {
                    // DÜZELTME: Bu yuvadan ne kadar alabileceðimizi hesapla
                    // (Ýhtiyacýmýz olan miktar ile yuvadaki miktar arasýndan küçük olaný seç)
                    int buYuvadanAlinacak = Mathf.Min(silinecekMiktar, _oyuncuEnvanteri.hotbarSlotlari[i].miktar);

                    // DÜZELTME: Yuvanýn miktarýný azalt
                    _oyuncuEnvanteri.hotbarSlotlari[i].miktar -= buYuvadanAlinacak;

                    // DÜZELTME: Silinecek toplam miktarý azalt
                    silinecekMiktar -= buYuvadanAlinacak;

                    // Eðer bu yuva tamamen boþaldýysa, tipini de sýfýrla
                    if (_oyuncuEnvanteri.hotbarSlotlari[i].miktar <= 0)
                    {
                        _oyuncuEnvanteri.hotbarSlotlari[i] = new EnvanterYuvasi(); // Yuvayý tamamen boþalt
                    }
                }
            }
        }

        // 2. Sonucu Envantere Ekle (Bu kýsým ayný kalýr)
        _oyuncuEnvanteri.EsyaEkle(tarif.sonucEsya, tarif.sonucMiktari);

        Debug.Log($"[{tarif.name}] tarifi basarili! Uretilen: {tarif.sonucEsya}");

        // Not: EsyaEkle metodu zaten EnvanteriGuncelle'yi çaðýrýyor, bu yüzden burada tekrar çaðýrmaya gerek yok.
    }
    public void SeciliTarifiUret()
    {
        // Gerekli kontroller
        if (_oyuncuEnvanteri == null || _mevcutTarifler.Length == 0) return;

        // O an secili olan tarifi al
        TarifSO seciliTarif = _mevcutTarifler[_mevcutTarifIndex];

        // Malzemelerin yeterli olup olmadigini kontrol et
        if (TarifKontrolEt(seciliTarif))
        {
            // Malzemeler yeterliyse uretimi gerceklestir
            UretimiGerceklestir(seciliTarif);

            // Uretimden sonra envanter degistigi icin UI'i tekrar guncelle
            GuncelTarifiGoster();
        }
        else
        {
            Debug.Log($"'{seciliTarif.name}' uretilemiyor. Yetersiz malzeme.");
            // Burada oyuncuya bir ses efekti veya görsel uyarý verilebilir.
        }
    }
}