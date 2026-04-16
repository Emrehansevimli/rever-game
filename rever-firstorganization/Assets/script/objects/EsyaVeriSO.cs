using UnityEngine;

// Her bir EsyaTipi icin ayri bir veri dosyasi olusturmamizi saglar.
[CreateAssetMenu(fileName = "Esya_Yeni", menuName = "Esya/Yeni Esya Verisi")]
public class EsyaVeriSO : ScriptableObject
{
    [Header("Temel Taným")]
    // Bu veri dosyasinin hangi EsyaTipi'ne ait oldugunu gosterir
    public EsyaTipi esyaTipi;

    public string gorunurAd = "Yeni Oge";

    [TextArea(3, 5)]
    public string aciklama = "Bu bir oyundaki esyadir.";

    [Header("Görsel & Ýstif")]
    // Her esyanin envanterdeki RESMI (ICON)
    public Sprite ikon;

    public int maxIstifBoyutu = 99; // Kac tanesinin ust uste binebilecegi (Stack size)

    [Header("Kullaným Etkisi")]
    public float canEtkisi = 0f; 
    public bool kullanilabilir = false; 

    [Header("Dünya Modeli")]
    // YENÝ EKLENEN: Esya atildiginda dunyada olusacak 3D model.
    public GameObject esyaModelPrefab;
    [Header("Ekonomi Degerleri")]
    [Tooltip("Bu eþyayi NPC'den satin alma fiyati.")]
    public int satinAlmaFiyati = 10;
    [Tooltip("Bu eþyayi NPC'ye satma fiyati (genellikle alis fiyatindan düsük).")]
    public int satmaFiyati = 8;
    public bool satilabilirMi = true;
}