using UnityEngine;

public class HayvanUretim : MonoBehaviour, IIletisim
{
    [Header("Ayarlar")]
    public bool otomatikUretim = false; // Tavuk ise True, Ýnek/Koyun ise False
    public EsyaTipi uretimEsyasi = EsyaTipi.Elma;
    public int uretimMiktari = 1;
    public GameObject urunPrefab; // Otomatik üretim için yere düþecek prefab (Örn: Yumurta)

    private HayvanDurum _hayvanDurum;

    void Start()
    {
        _hayvanDurum = GetComponent<HayvanDurum>();
    }

    // ***************************************************************
    // 1. MANUEL ÜRETÝM (ÝNEK/KOYUN - E TUÞU ÝLE)
    // ***************************************************************
    public void IletisimeGec(GameObject etkilesen)
    {
        // Eðer otomatik üretimse, manuel etkileþimi yok say
        if (otomatikUretim) return;

        if (_hayvanDurum.urunVermeyeHazir)
        {
            OyuncuEnvanter envanter = etkilesen.GetComponent<OyuncuEnvanter>();
            if (envanter != null)
            {
                UrunuUretManuel(envanter);
            }
        }
        else
        {
            Debug.Log($"{gameObject.name} henüz ürün vermeye hazýr deðil. Önce beslenmeli.");
        }
    }

    // Envantere ekleyen metot
    public void UrunuUretManuel(OyuncuEnvanter envanter)
    {
        // ... (Miktar kadar eþya ekleme mantýðý ayný)
        bool basarili = true; // Basit kontrol, envanter doluysa bu false olur.

        for (int i = 0; i < uretimMiktari; i++)
        {
            if (!envanter.EsyaEkle(uretimEsyasi, uretimMiktari))
            {
                basarili = false;
                break;
            }
        }

        if (basarili)
        {
            _hayvanDurum.UrunVerildi(); // Durumu sýfýrla
            Debug.Log($"{gameObject.name} ürün verdi. Envantere {uretimMiktari} adet {uretimEsyasi} eklendi.");
        }
    }

    // ***************************************************************
    // 2. OTOMATÝK ÜRETÝM (TAVUK - Yere Býrakma)
    // ***************************************************************
    public void UrunuUretOtomomik()
    {
        if (urunPrefab == null)
        {
            Debug.LogError("Üretilecek prefab atanmamýþ!");
            return;
        }

        // Ürünü hayvanýn yanýna yere býrak
        Vector3 spawnPozisyonu = transform.position + Vector3.up * 0.5f + transform.forward * 0.5f;
        Instantiate(urunPrefab, spawnPozisyonu, Quaternion.identity);

        _hayvanDurum.UrunVerildi(); // Durumu sýfýrla
        Debug.Log($"{gameObject.name} yumurtladý ve yere býraktý.");
    }
}