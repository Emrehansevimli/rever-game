using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System; 

public class OyunZamani : MonoBehaviour
{
    // TARLAYA HABER VERECEK EVENT BURADA:
    public static event Action YeniGunBasladi;

    [Header("Zaman Ayarlarý")]
    [Tooltip("Oyun içinde 1 gün kaç gerçek saniye sürsün?")]
    public float gunSuresi = 120f;

    [Header("Referanslar")]
    public Light oyunGunesi;
    public Transform gokyuzuDiski;
    public TextMeshProUGUI saatText;

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
        gecenSure += Time.deltaTime;

        if (gecenSure >= gunSuresi)
        {
            gecenSure = 0;
            gunSayisi++;

            // GÜN DEÐÝÞTÝÐÝNDE DÝÐER KODLARA HABER SAL:
            YeniGunBasladi?.Invoke();
        }
    }

    void GorselleriGuncelle()
    {
        float gunYuzdesi = gecenSure / gunSuresi;

        if (gokyuzuDiski != null)
        {
            float rotasyon = gunYuzdesi * 360f;
            gokyuzuDiski.localRotation = Quaternion.Euler(0, 0, -rotasyon);
        }

        if (oyunGunesi != null)
        {
            float gunesAcisi = (gunYuzdesi * 360f);
            oyunGunesi.transform.localRotation = Quaternion.Euler(gunesAcisi, 170f, 0);
            oyunGunesi.intensity = (gunYuzdesi > 0.25f && gunYuzdesi < 0.75f) ? 1f : 0.1f;
        }

        if (saatText != null)
        {
            float saat = Mathf.Floor(gunYuzdesi * 24);
            float dakika = Mathf.Floor(((gunYuzdesi * 24) - saat) * 60);
            saatText.text = $"Gün: {gunSayisi} | {saat:00}:{dakika:00}";
        }
    }
}