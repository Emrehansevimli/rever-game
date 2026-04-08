using UnityEngine;

public class GunDongusuYonetici : MonoBehaviour
{
    [Header("Isik Kaynagi")]
    // Sahnedeki Gunes'i (Directional Light) buraya sürükleyip birakacagiz.
    public Light gunes;

    [Header("Dongu Ayarlari")]
    // Bir oyun gununun gercek hayatta kac dakika sürecegini belirler.
    [Tooltip("Bir tam günün (24 saat) kaç dakika süreceði.")]
    public float gunSuresiDakikaCinsinden = 1.0f;

    // Mevcut oyun ici zamani 0-1 arasinda tutar (0 = Gece yarisi, 0.5 = Oglen)
    [Range(0, 1)]
    public float mevcutZaman = 0.25f; // Oyunu sabah baslatmak icin 0.25

    [Header("Ortam Renkleri")]
    public Color gunduzRengi = new Color(0.53f, 0.75f, 1.0f); // Açýk mavi
    public Color geceRengi = new Color(0.05f, 0.1f, 0.2f);   // Koyu lacivert

    void Start()
    {
        // Gunes objesinin atanip atanmadigini kontrol et
        if (gunes == null)
        {
            Debug.LogError("Gunes (Directional Light) atanmamis!");
            this.enabled = false; // Script'i devre disi birak
            return;
        }
    }

    void Update()
    {
        // 1. Zamani Guncelle
        // Zamaný, belirlenen gün süresine göre yavaþça artýr.
        // Time.deltaTime / (gunSuresi * 60) -> Bir saniyede ne kadar ilerlemesi gerektigini hesaplar.
        mevcutZaman += Time.deltaTime / (gunSuresiDakikaCinsinden * 60);

        // Zaman 1'i gectiginde, donguyu basa sar (yani yeni bir gune basla)
        if (mevcutZaman >= 1)
        {
            mevcutZaman -= 1;
        }

        // 2. Gunes'in Pozisyonunu Guncelle
        GunesiDondur();

        // 3. Ortam Isigini Guncelle
        OrtamIsiginiGuncelle();
    }

    private void GunesiDondur()
    {
        // Zamaný 360 derecelik bir açýya çevirir.
        // mevcutZaman * 360f -> 0 iken 0 derece, 0.5 iken 180 derece olur.
        float gunesAcisi = mevcutZaman * 360f;

        // Gunes'i X ekseni etrafýnda bu açýyla döndür.
        // Bu, Gunes'in ufukta doðup batmasýný simüle eder.
        gunes.transform.localRotation = Quaternion.Euler(new Vector3(gunesAcisi, -30f, 0));
    }

    private void OrtamIsiginiGuncelle()
    {
        // Gündüz ve gece renkleri arasýnda yumuþak bir geçiþ yap.
        // Dot product ile Gunes'in ne kadar yukarida oldugunu anlariz.
        float dotProduct = Vector3.Dot(gunes.transform.forward, Vector3.down);

        // Bu degeri 0-1 arasina sýkýþtýrarak gecis icin kullaniriz.
        float gecisFaktoru = Mathf.Clamp01((dotProduct + 1) / 2f);

        // RenderSettings.ambientLight, sahnedeki golgelerin ne kadar aydinlik olacagini belirler.
        RenderSettings.ambientLight = Color.Lerp(geceRengi, gunduzRengi, gecisFaktoru);
    }
}