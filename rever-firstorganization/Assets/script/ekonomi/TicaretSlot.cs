using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TicaretSlot : MonoBehaviour
{
    [Header("UI Referanslari")]
    public TextMeshProUGUI adText;
    public TextMeshProUGUI fiyatText;
    public Image ikonImage;
    public Button islemButonu; // Adýný genel 'islemButonu' olarak düþünelim (Eski adý: satinAlButonu)

    [Header("Miktar ve Buton Yazisi")]
    public TMP_InputField miktarInput; // Miktar giriþi
    public TextMeshProUGUI butonYazisi; // Butonun üzerindeki "AL" veya "SAT" yazýsý

    private EsyaVeriSO _esyaVeri;
    private bool _satisModu; // Bu slot bir satýþ slotu mu (oyuncunun envanteri) yoksa alýþ mý?

    void Start()
    {
        // Singleton Instance kontrolü (Daha performanslý)
        if (TicaretYonetici.Instance == null)
        {
            Debug.LogError("Sahnede TicaretYonetici bulunamadi!");
            islemButonu.interactable = false;
        }
    }

    // Bu fonksiyonu TicaretUIManager çaðýracak
    // satisModu = TRUE ise -> Oyuncu Satýyor (Sað liste)
    // satisModu = FALSE ise -> Oyuncu Alýyor (Sol liste)
    public void SlotuKur(EsyaVeriSO veri, bool satisModu)
    {
        _esyaVeri = veri;
        _satisModu = satisModu;

        // 1. Temel Görselleri Ayarla
        adText.text = veri.gorunurAd; // Senin deðiþken adýn
        ikonImage.sprite = veri.ikon;

        // Varsayýlan miktar 1 olsun
        if (miktarInput != null) miktarInput.text = "1";

        // 2. Moduna Göre Fiyat ve Buton Ayarla
        islemButonu.onClick.RemoveAllListeners(); // Eski týklama olaylarýný temizle

        if (_satisModu)
        {
            // --- SATIÞ MODU (Oyuncunun Eþyasý) ---
            int satisFiyati = veri.satmaFiyati ; // Yarý fiyatýna satalým
            fiyatText.text = $"{satisFiyati} G";

            if (butonYazisi != null) butonYazisi.text = "SAT";

            // Butona basýnca 'Sat' fonksiyonu çalýþsýn
            islemButonu.onClick.AddListener(SatisIslemi);
        }
        else
        {
            // --- ALIÞ MODU (Satýcýnýn Eþyasý) ---
            fiyatText.text = $"{veri.satinAlmaFiyati} G";

            if (butonYazisi != null) butonYazisi.text = "AL";

            // Butona basýnca 'SatinAl' fonksiyonu çalýþsýn
            islemButonu.onClick.AddListener(SatinAlmaIslemi);
        }
    }

    // MARKET -> OYUNCU (Eþya Al)
    void SatinAlmaIslemi()
    {
        if (_esyaVeri == null) return;

        int miktar = MiktariAl();

        // Yöneticiye gönder
        bool basarili = TicaretYonetici.Instance.SatinAl(_esyaVeri.esyaTipi, miktar);

        if (basarili)
        {
            Debug.Log($"Baþarýyla {miktar} adet {_esyaVeri.gorunurAd} satýn alýndý.");
        }
    }

    // OYUNCU -> MARKET (Eþya Sat)
    void SatisIslemi()
    {
        if (_esyaVeri == null) return;

        int miktar = MiktariAl();

       
        for (int i = 0; i < miktar; i++)
        {
            TicaretYonetici.Instance.OyuncudanEsyaSatinAl(_esyaVeri.esyaTipi);
        }   
    }

    // Input alanýndan sayýyý güvenli þekilde çeken yardýmcý fonksiyon
    int MiktariAl()
    {
        if (miktarInput == null) return 1;

        if (int.TryParse(miktarInput.text, out int miktar))
        {
            return miktar > 0 ? miktar : 1;
        }
        return 1;
    }
}