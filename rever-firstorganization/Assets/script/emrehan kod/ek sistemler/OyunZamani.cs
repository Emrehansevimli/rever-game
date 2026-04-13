using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OyunZamani : MonoBehaviour
{
    [Header("Zaman Ayarlarý")]
    [Tooltip("Oyun içinde 1 gün kaç gerçek saniye sürsün?")]
    public float gunSuresi = 120f; // Örn: 2 dakika = 1 oyun günü

    [Header("Referanslar")]
    public Light oyunGunesi; // Directional Light'ý buraya at
    public Transform gokyuzuDiski; // UI'daki dönen resim (GokyuzuDiski)
    public TextMeshProUGUI saatText; // Dijital saat yazýsý

    [Header("Veri (Ýzleme)")]
    public float gecenSure;
    public int gunSayisi = 1;

    void Update()
    {
        ZamaniIlerlet();
        GorselleriGuncelle();
    }

    void ZamaniIlerlet()
    {
        // Zamaný akýt
        gecenSure += Time.deltaTime;

        // Gün bitti mi?
        if (gecenSure >= gunSuresi)
        {
            gecenSure = 0;
            gunSayisi++;
        }
    }

    void GorselleriGuncelle()
    {
        // 0 ile 1 arasýnda bir oran (Günün yüzde kaçý bitti?)
        float gunYuzdesi = gecenSure / gunSuresi;

        // 1. UI DÝSKÝNÝ DÖNDÜR (360 derece dönecek)
        if (gokyuzuDiski != null)
        {
            // Z ekseninde döndürüyoruz
            float rotasyon = gunYuzdesi * 360f;
            gokyuzuDiski.localRotation = Quaternion.Euler(0, 0, -rotasyon);
        }

        // 2. OYUN GÜNEÞÝNÝ DÖNDÜR (Gerçek gölge oluþsun!)
        if (oyunGunesi != null)
        {
            // Güneþ X ekseninde döner (Doðudan batýya)
            // -90 dereceden baþlayýp (Sabah), 270'e kadar dönsün
            float gunesAcisi = (gunYuzdesi * 360f) ;
            oyunGunesi.transform.localRotation = Quaternion.Euler(gunesAcisi, 170f, 0);

            // Gece olunca ýþýðý kapatabilir veya rengini deðiþtirebilirsin (Geliþtirilebilir)
            oyunGunesi.intensity = (gunYuzdesi > 0.25f && gunYuzdesi < 0.75f) ? 1f : 0.1f;
        }

        // 3. SAAT YAZISINI GÜNCELLE
        if (saatText != null)
        {
            // Yüzdeyi saate çevirme (0-24 arasý)
            float saat = Mathf.Floor(gunYuzdesi * 24);
            float dakika = Mathf.Floor(((gunYuzdesi * 24) - saat) * 60);

            // Örnek Çýktý: "Gün: 5 | 14:30"
            saatText.text = $"Gün: {gunSayisi} | {saat:00}:{dakika:00}";
        }
    }
}