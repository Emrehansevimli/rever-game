using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class InventoryUIManager : MonoBehaviour
{

    public static InventoryUIManager Instance;

    [Header("UI Bilesenleri")]
    // Tek bir envanter yuvasini temsil eden Prefab
    public GameObject yuvaPrefab;
    // Yuvalarin yerlestirilecegi konteyner (GridLayoutGroup olan obje)
    public Transform yuvaKonteyneri;

    // Envanterdeki yuvalari tutariz
    private List<GameObject> _yuvalar = new List<GameObject>();
    [Header("Esya Verileri")]
    public EsyaVeriSO[] tumEsyaVerileri;

    // Veriye hizli erisim icin Dictionary (Key: EsyaTipi, Value: EsyaVeriSO)
    private Dictionary<EsyaTipi, EsyaVeriSO> _veriSozluk;
    // OyuncuEnvanter scriptine erisim
    private OyuncuEnvanter _envanter;

    private List<EnvanterYuvaUI> _yuvaUIListesi = new List<EnvanterYuvaUI>();
    [Header("Panel Kontrolü")]
    public GameObject envanterPanel;
    void Awake()
    {
        // Singleton yapýsý: Instance'ý bu objeye atar.
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // Sahnedeki ikinci bir kopyayý yok et
            Destroy(gameObject);
        }
        _veriSozluk = tumEsyaVerileri.ToDictionary(veri => veri.esyaTipi, veri => veri);
        // Buradan Start() metoduna veya baþka bir yere taþýndýysa, 
        // Start metodunda da GetComponent<OyuncuEnvanter>() çaðrýsý yapýlabilir.
    }
    void Start()
    {

        // Oyuncunun envanterini bul
        GameObject oyuncu = GameObject.FindGameObjectWithTag("Player");

        // **** GÜVENLÝK KONTROLÜ 1 ****
        if (oyuncu == null)
        {
            Debug.LogError("HATA: Sahnedeki karakterinize 'Player' tag'ini vermeyi unuttunuz!");
            return; // Oyuncu bulunamazsa devam etme, hata vermesini engelle
        }

        _envanter = oyuncu.GetComponent<OyuncuEnvanter>();

        // **** GÜVENLÝK KONTROLÜ 2 ****
        if (_envanter == null)
        {
            Debug.LogError("HATA: 'Player' tag'li objenin uzerinde OyuncuEnvanter scripti bulunamadi!");
            return; // Script bulunamazsa devam etme
        }

        foreach (Transform child in yuvaKonteyneri)
        {
            Destroy(child.gameObject);
        }
        // Temizlik sonrasý listeyi de sýfýrla
        _yuvaUIListesi.Clear();
        // Baslangicta sabit sayida bos yuva olustur
        for (int i = 0; i < _envanter.hotbarBoyutu; i++)
        {
            GameObject yeniYuvaObjesi = Instantiate(yuvaPrefab, yuvaKonteyneri);
            EnvanterYuvaUI yuvaUI = yeniYuvaObjesi.GetComponent<EnvanterYuvaUI>();
            yuvaUI.Temizle();
            _yuvaUIListesi.Add(yuvaUI);
        }

        EnvanteriGuncelle();
    }

    public void EnvanteriGuncelle()
    {
        if (_envanter == null) return;

        for (int i = 0; i < _envanter.hotbarBoyutu; i++)
        {
            // DÜZELTME: Dizi artýk EsyaTipi deðil, EnvanterYuvasi tutuyor.
            EnvanterYuvasi esyaYuvasi = _envanter.hotbarSlotlari[i];
            EnvanterYuvaUI yuvaUI = _yuvaUIListesi[i];

            // Yuvada esya var mi (miktarina bakarak anlariz)
            if (esyaYuvasi.miktar > 0)
            {
                // DÜZELTME: VeriGetir'e yuvanýn içindeki .tip özelliðini gönderiyoruz.
                EsyaVeriSO esyaVerisi = VeriGetir(esyaYuvasi.tip);
                yuvaUI.YuvayiGuncelle(esyaVerisi, esyaYuvasi.miktar);
            }
            else
            {
                yuvaUI.Temizle();
            }
        }

        HighlightGuncelle();
    }

    // YENÝ: Secili yuvayi görsel olarak isaretleme metodu
    public void HighlightGuncelle()
    {
        for (int i = 0; i < _yuvaUIListesi.Count; i++)
        {
            // Her yuvanin uzerinde secili olup olmadigini gosteren bir metot olmali
            _yuvaUIListesi[i].SecimiAyarla(i == _envanter.seciliSlotIndex);
        }
    }
   
    public EsyaVeriSO VeriGetir(EsyaTipi tip)
    {
        if (_veriSozluk.ContainsKey(tip))
        {
            return _veriSozluk[tip];
        }
        return null;
    }
    public void EnvanterAcKapat(bool acilsinMi)
    {
       
        if (envanterPanel != null)
        {
            Debug.Log("Envanter Paneli Durumu Deðiþiyor: " + acilsinMi);
            envanterPanel.SetActive(acilsinMi);

            // Panel açýldýðýnda verilerin güncel olduðundan emin olalým
            if (acilsinMi)
            {
                EnvanteriGuncelle();
            }
        }
        else
        {
            Debug.LogWarning("InventoryUIManager: 'envanterPanel' Inspector'da atanmamýþ!");
        }
    }

}