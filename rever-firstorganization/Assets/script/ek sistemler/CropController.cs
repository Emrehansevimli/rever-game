using UnityEngine;

public class CropController : MonoBehaviour, IIletisim
{
    // Durumları tanımlıyoruz (Senin istediğin aşamalar)
    public enum TarlaDurumu { HamToprak, SurulmusToprak, Ekili, Olgun }

    [Header("Durum Takibi")]
    public TarlaDurumu suankiDurum = TarlaDurumu.HamToprak;

    [Header("Modeller (Görünüm)")]
    public GameObject modelHamToprak;      // Dümdüz toprak (Varsa ata, yoksa boş kalabilir)
    public GameObject modelSurulmusToprak; // Çapalanmış toprak görüntüsü (Varsa ata)
    public GameObject modelFide;           // Küçük yeşil filiz
    public GameObject modelOlgun;          // Kocaman havuç

   // [Header("Ayarlar")]
  //  public Item urunItem; // Envantere gelecek havuç
   // private InventoryManager inventoryManager;

    // --- ZAMAN SİSTEMİ BAĞLANTISI ---
    
    void OnEnable() { OyunZamani.YeniGunBasladi += Grow; }
    void OnDisable() { OyunZamani.YeniGunBasladi -= Grow; }

    void Start()
    {
        // Envanter yöneticisini bul
        //inventoryManager = InventoryManager.instance;
        //if (inventoryManager == null)
        //{
        //    GameObject manager = GameObject.Find("GAME_MANAGER");
        //    if (manager != null) inventoryManager = manager.GetComponent<InventoryManager>();
        //}

        // Oyuna başladığımızda görüntüleri duruma göre ayarla
        GorumunuGuncelle();
    }
    public void IletisimeGec(GameObject etkilesen)
    {
        switch (suankiDurum)
        {
            case TarlaDurumu.HamToprak:
                // 1. AŞAMA: Toprağı Sür
                suankiDurum = TarlaDurumu.SurulmusToprak;
                Debug.Log("🚜 Toprak sürüldü!");
                GorumunuGuncelle();
                break;

            case TarlaDurumu.SurulmusToprak:
                // 2. AŞAMA: Tohum Ek
                suankiDurum = TarlaDurumu.Ekili;
                Debug.Log("🌱 Tohum ekildi/Fide dikildi!");
                GorumunuGuncelle();
                break;

            case TarlaDurumu.Ekili:
                // Henüz büyümedi, kullanıcıya bilgi ver
                Debug.Log("⏳ Bitki henüz büyümedi. Uyu veya bekle.");
                break;

            case TarlaDurumu.Olgun:
                // 4. AŞAMA: Hasat Et
                HasatEt();
                break;
        }
    }
    // --- BÜYÜME (GECE GÜNDÜZ SİSTEMİ ÇAĞIRIR) ---
    public void Grow()
    {
        // Sadece ekili olanlar büyür
        if (suankiDurum == TarlaDurumu.Ekili)
        {
            suankiDurum = TarlaDurumu.Olgun;
            Debug.Log("🌞 Gün doğdu! Bitki olgunlaştı.");
            GorumunuGuncelle();
        }
    }

    // --- HASAT FONKSİYONU ---
    void HasatEt()
    {
        //if (inventoryManager != null && urunItem != null)
        //{
        //    bool eklendi = inventoryManager.AddItem(urunItem, 1);

        //    if (eklendi)
        //    {
        //        Debug.Log("🥕 Hasat yapıldı! Envantere eklendi.");

        //        // İsteğine göre burayı değiştirebilirsin:
        //        // Seçenek A: Toprak tekrar "Sürülmüş" hale döner (Tohum ekmeye hazır)
        //        // suankiDurum = TarlaDurumu.SurulmusToprak; 

        //        // Seçenek B: Toprak tamamen sıfırlanır "Ham" hale döner (Tekrar sürmen gerekir)
        //        suankiDurum = TarlaDurumu.HamToprak;

        //        GorumunuGuncelle(); // Havuç modelini gizle, toprağı göster
        //    }
        //    else
        //    {
        //        Debug.Log("❌ Envanter Dolu!");
        //    }
        //}
    }

    // --- GÖRÜNTÜ YÖNETİMİ ---
    void GorumunuGuncelle()
    {
        // Önce hepsini kapat
        if (modelHamToprak != null) modelHamToprak.SetActive(false);
        if (modelSurulmusToprak != null) modelSurulmusToprak.SetActive(false);
        if (modelFide != null) modelFide.SetActive(false);
        if (modelOlgun != null) modelOlgun.SetActive(false);

        // Duruma uygun olanı aç
        switch (suankiDurum)
        {
            case TarlaDurumu.HamToprak:
                if (modelHamToprak != null) modelHamToprak.SetActive(true);
                break;
            case TarlaDurumu.SurulmusToprak:
                if (modelSurulmusToprak != null) modelSurulmusToprak.SetActive(true);
                // Eğer özel sürülmüş toprak modelin yoksa, ham toprak açık kalsın:
                else if (modelHamToprak != null) modelHamToprak.SetActive(true);
                break;
            case TarlaDurumu.Ekili:
                if (modelFide != null) modelFide.SetActive(true);
                break;
            case TarlaDurumu.Olgun:
                if (modelOlgun != null) modelOlgun.SetActive(true);
                break;
        }
    }
}