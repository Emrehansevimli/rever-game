using UnityEngine;

public class TicaretYonetici : MonoBehaviour
{
    // 1. HATA ÇÖZÜMÜ: Singleton Instance Tanýmý
    public static TicaretYonetici Instance;

    // Oyuncu referansla
    private OyuncuEnvanter _oyuncuEnvanter;
    private KarakterDurum _karakterDurum;

    void Awake()
    {
        // Singleton yapýsý (Instance hatasýný çözer)
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Oyuncuyu ve gerekli scriptleri bul
        GameObject oyuncu = GameObject.FindGameObjectWithTag("Player");
        if (oyuncu != null)
        {
            _oyuncuEnvanter = oyuncu.GetComponent<OyuncuEnvanter>();
            _karakterDurum = oyuncu.GetComponent<KarakterDurum>();
        }
        else
        {
            Debug.LogError("TicaretYonetici: 'Player' tag'ine sahip oyuncu bulunamadý!");
        }
    }

    // --- OYUNCU MARKETEN EÞYA ALIR ---
    public bool SatinAl(EsyaTipi esyaTipi, int miktar)
    {
        // Gerekli referanslar yoksa iþlem yapma
        if (_oyuncuEnvanter == null || _karakterDurum == null) return false;

        EsyaVeriSO esyaVerisi = InventoryUIManager.Instance.VeriGetir(esyaTipi);
        if (esyaVerisi == null) return false;

        int toplamTutar = esyaVerisi.satinAlmaFiyati * miktar;

        // Parasý yetiyor mu?
        if (_karakterDurum.MevcutPara >= toplamTutar)
        {
            // Parayý düþ
            bool paraOdendi = _karakterDurum.ParaCikar(toplamTutar);

            if (paraOdendi)
            {
                // Eþyayý envantere ekle
                _oyuncuEnvanter.EsyaEkle(esyaTipi, miktar);

                // UI Güncellemeleri
                TicaretUIManager.Instance.ParaGuncelle();
                if (InventoryUIManager.Instance != null) InventoryUIManager.Instance.EnvanteriGuncelle();

                return true; // Ýþlem baþarýlý
            }
        }
        else
        {
            Debug.Log("Yetersiz Bakiye!");
        }
        return false; // Ýþlem baþarýsýz
    }

    // --- 2. HATA ÇÖZÜMÜ: OYUNCU MARKETE EÞYA SATAR ---
    public void OyuncudanEsyaSatinAl(EsyaTipi esyaTipi)
    {
        if (_oyuncuEnvanter == null || _karakterDurum == null) return;

        EsyaVeriSO esyaVerisi = InventoryUIManager.Instance.VeriGetir(esyaTipi);
        if (esyaVerisi == null) return;

        // Satýþ fiyatý (Alýþ fiyatýnýn yarýsý)
        int satisFiyati = esyaVerisi.satmaFiyati;

        // Envanterden 1 tane sil
        bool silindi = _oyuncuEnvanter.EsyaCikar(esyaTipi, 1);

        if (silindi)
        {
            // Parayý ekle
            _karakterDurum.ParaEkle(satisFiyati);

            // UI Güncellemeleri
            TicaretUIManager.Instance.ParaGuncelle(); // Para yazýsýný güncelle
            TicaretUIManager.Instance.OyuncuListesiniDoldur(); // Listeyi yenile ki sayý düþsün

            Debug.Log($"{esyaVerisi.gorunurAd} satýldý. Kazanýlan: {satisFiyati}");
        }
    }
}