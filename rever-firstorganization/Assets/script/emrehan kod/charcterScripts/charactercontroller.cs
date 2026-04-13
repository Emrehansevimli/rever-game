
using UnityEngine;

public class charactercontroller : MonoBehaviour
{

    [Header("Karakter Verisi (KÝMLÝK KARTI)")]
    public KarakterVerisiSO karakterVerisi;

    // Mevcut deðiþkenlerin (Bunlar artýk veriden okunacak)
    public float yürümeHizi; // Inspector'dan deðiþtirmene gerek kalmayacak
    public float zýplamaKuvveti;
    public float kosmaHizi;

   


    private OyuncuEnvanter _envanter;
    private CharacterController _controller;
    private KarakterDurum _karakterDurum;
    public bool hareketEdebilir = true;   
    public float yercekimi = 20.0f;
    private Vector3 _hizVektoru;
    public float fareHassasiyeti = 100f;
    public Transform kameraTransform;

    private float _xRotasyon = 0f;
    private Vector2 _anlikGirdi = Vector2.zero;
    private bool _sprintBasildi = false;
    private bool _ziplamaBasildi = false;
    private bool _uiAcik = false;
    // Kameranýn dikey bakýþ açýsý sýnýrlarý
    public float maxBakýþAçýsý = 90f;
    public float minBakýþAçýsý = -90f;
    void Start()
    {
        _envanter = GetComponent<OyuncuEnvanter>();
        _controller = GetComponent<CharacterController>();
        _karakterDurum = GetComponent<KarakterDurum>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Eðer kameraTransform atanmamýþsa, alt objelerden ana kamerayý bulmaya çalýþ
        if (kameraTransform == null)
        {
            kameraTransform = GetComponentInChildren<Camera>()?.transform;
        }
        if (karakterVerisi != null)
        {
            // Hareket verilerini aktar
            this.yürümeHizi = karakterVerisi.yurumeHizi;
            this.kosmaHizi = karakterVerisi.kosmaHizi;
            this.zýplamaKuvveti = karakterVerisi.ziplamaKuvveti;

            // Can verisini aktar (Eðer KarakterDurum scriptin varsa)
            if (_karakterDurum != null)
            {
                _karakterDurum.KarakterOzellikleriniAyarla(karakterVerisi.maxCan, karakterVerisi.stamina);
            }

            Debug.Log($"Seçilen Karakter: {karakterVerisi.karakterAdi} yüklendi.");
        }
    }
    void Update()
    {
        // 1. UI Kontrolü (Hareketi hemen kes)
        //_uiAcik = CraftingUIManager.Instance != null && CraftingUIManager.Instance.IsPanelOpen;
        bool craftingAcik = CraftingUIManager.Instance != null && CraftingUIManager.Instance.IsPanelOpen;

        // YENÝ: Ticaret Açýk mý? (Eðer script sahnede varsa kontrol et)
        bool ticaretAcik = TicaretUIManager.Instance != null && TicaretUIManager.Instance.IsOpen;
        bool zulaAcik = ZulaUIManager.Instance != null && ZulaUIManager.Instance.IsOpen;
        // Herhangi biri açýksa VEYA hareket izni manuel olarak kapatýldýysa
        _uiAcik = craftingAcik || ticaretAcik || zulaAcik || !hareketEdebilir;
        if (_uiAcik)
        {
            // UI açýkken fare imlecini göster
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _anlikGirdi = Vector2.zero; // Hareketi sýfýrla
            _sprintBasildi = false;
            return;
        }
        else
        {
            // UI kapalýyken imleci kilitle
           // Cursor.lockState = CursorLockMode.Locked;
           // Cursor.visible = false;
        }

        // 2. YÜRÜME INPUT'u AL ve KAYDET
        _anlikGirdi.x = Input.GetAxis("Horizontal");
        _anlikGirdi.y = Input.GetAxis("Vertical");
        _sprintBasildi = Input.GetKey(KeyCode.LeftShift);

        // Zýplama input'unu sadece basýldýðý anda alýyoruz ve FixedUpdate'e býrakýyoruz
        if (Input.GetButtonDown("Jump"))
        {
            _ziplamaBasildi = true; // Sadece basýldýðý frame'de true yapar
        }

        // 3. FARE GÝRDÝSÝ VE KAMERA
        float fareX = Input.GetAxis("Mouse X") * fareHassasiyeti * Time.deltaTime;
        float fareY = Input.GetAxis("Mouse Y") * fareHassasiyeti * Time.deltaTime;

        transform.Rotate(Vector3.up * fareX);
        _xRotasyon -= fareY;

        _xRotasyon = Mathf.Clamp(_xRotasyon, minBakýþAçýsý, maxBakýþAçýsý);

        if (kameraTransform != null)
        {
            kameraTransform.localRotation = Quaternion.Euler(_xRotasyon, 0f, 0f);
        }

        // 4. HOTBAR INPUT
        HotbarInputKontrol();
    }

    // ***************************************************************
    // Fiziksel Hareketi ve Yerçekimini Ýþler (Sabit Zaman Döngüsü)
    // ***************************************************************
    void FixedUpdate()
    {
        if (_uiAcik) return; // UI açýksa FixedUpdate'te de hareket etme

        bool hareketEdiyor = _anlikGirdi.x != 0 || _anlikGirdi.y != 0;
        bool kosabilirMi = _sprintBasildi && hareketEdiyor && _karakterDurum.StaminaVarMi();

        float mevcutHiz;

        // HIZ VE STAMINA HESABI (FixedUpdate'te stamina kullanýmýný belirle)
        if (kosabilirMi)
        {
            mevcutHiz = kosmaHizi;
            _karakterDurum.staminaKullaniliyor = true; // Staminayý tüket
        }
        else
        {
            mevcutHiz = yürümeHizi;
            _karakterDurum.staminaKullaniliyor = false; // Staminayý yenile
        }

        // 1. YÜRÜME HESAPLAMASI
        // Input'u al ve hýzý uygula
        Vector3 hareket = transform.right * _anlikGirdi.x + transform.forward * _anlikGirdi.y;
        hareket = hareket.normalized * mevcutHiz;

        // 2. YERCEKÝMÝ & ZIPLAMA
        if (_controller.isGrounded)
        {
            _hizVektoru.y = 0f;

            // Sadece zýplama isteði varsa uygula
            if (_ziplamaBasildi)
            {
                _hizVektoru.y = zýplamaKuvveti;
                _ziplamaBasildi = false; // Zýplama input'unu HEMEN sýfýrla!
            }
        }

        // Yerçekimini uygulamaya devam et
        _hizVektoru.y -= yercekimi * Time.fixedDeltaTime;

        // 3. HAREKET UYGULA (Sonuç olarak gecikmesiz ve tutarlý hareket)
        Vector3 sonHareket = hareket + _hizVektoru;
        _controller.Move(sonHareket * Time.fixedDeltaTime);
    }
    private void HotbarInputKontrol()
    {
        if (_envanter == null) return;

        // 1'den 8'e kadar tuslari kontrol et
        for (int i = 0; i < _envanter.hotbarBoyutu; i++)
        {
            // Eðer bir sayý tuþuna basýlýrsa...
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                // ...oyuncunun seçtiði slotu güncelle...
                _envanter.seciliSlotIndex = i;

                // ...VE HEMEN ARDINDAN UI YÖNETÝCÝSÝNE GÖRSELÝ GÜNCELLEMESÝNÝ SÖYLE!
                if (InventoryUIManager.Instance != null)
                {
                    InventoryUIManager.Instance.HighlightGuncelle();
                }

                Debug.Log($"{i + 1}. yuva secildi.");
                return;
            }
        }
        // F tusu = Kullan
        if (Input.GetKeyDown(KeyCode.F))
        {
            _envanter.SeciliEsyayiKullan();
        }

        // Q tusu = At
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _envanter.SeciliEsyayiAt();
        }
    }
}