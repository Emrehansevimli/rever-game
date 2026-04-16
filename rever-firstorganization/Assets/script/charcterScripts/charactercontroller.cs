using UnityEngine;

public class charactercontroller : MonoBehaviour
{
    [Header("Karakter Verisi (KÝMLÝK KARTI)")]
    public KarakterVerisiSO karakterVerisi;

    // Mevcut deðiþkenlerin
    public float yürümeHizi;
    public float zýplamaKuvveti;
    public float kosmaHizi;

    [Header("Kamera Ayarlarý (FPS/TPS)")]
    public Transform kameraTransform;
    public float fareHassasiyeti = 100f;
    public float maxBakýþAçýsý = 90f;
    public float minBakýþAçýsý = -90f;

    // YENÝ EKLENEN KAMERA DEÐÝÞKENLERÝ
    [Tooltip("Kameranýn FPS modundaki konumu (Karakterin göz hizasý)")]
    public Vector3 firstPersonPozisyon = new Vector3(0f, 0.7f, 0.2f);
    [Tooltip("Kameranýn TPS modundaki konumu (Karakterin arkasý)")]
    public Vector3 thirdPersonPozisyon = new Vector3(0f, 1f, -3f);
    [Tooltip("Geçiþin ne kadar hýzlý/yumuþak olacaðý")]
    public float kameraGecisHizi = 10f;
    private bool _isFirstPerson = false; // Baþlangýçta TPS varsayýyoruz

    private OyuncuEnvanter _envanter;
    private CharacterController _controller;
    private KarakterDurum _karakterDurum;
    public bool hareketEdebilir = true;
    public float yercekimi = 20.0f;
    private Vector3 _hizVektoru;

    private float _xRotasyon = 0f;
    private Vector2 _anlikGirdi = Vector2.zero;
    private bool _sprintBasildi = false;
    private bool _ziplamaBasildi = false;
    private bool _uiAcik = false;

    void Start()
    {
        _envanter = GetComponent<OyuncuEnvanter>();
        _controller = GetComponent<CharacterController>();
        _karakterDurum = GetComponent<KarakterDurum>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (kameraTransform == null)
        {
            kameraTransform = GetComponentInChildren<Camera>()?.transform;
        }

        // Baþlangýçta kamerayý direkt TPS noktasýna koyalým
        if (kameraTransform != null)
        {
            kameraTransform.localPosition = thirdPersonPozisyon;
        }

        if (karakterVerisi != null)
        {
            this.yürümeHizi = karakterVerisi.yurumeHizi;
            this.kosmaHizi = karakterVerisi.kosmaHizi;
            this.zýplamaKuvveti = karakterVerisi.ziplamaKuvveti;

            if (_karakterDurum != null)
            {
                _karakterDurum.KarakterOzellikleriniAyarla(karakterVerisi.maxCan, karakterVerisi.stamina);
            }

            Debug.Log($"Seçilen Karakter: {karakterVerisi.karakterAdi} yüklendi.");
        }
    }

    void Update()
    {
        // 1. UI Kontrolü
        bool craftingAcik = CraftingUIManager.Instance != null && CraftingUIManager.Instance.IsPanelOpen;
        bool ticaretAcik = TicaretUIManager.Instance != null && TicaretUIManager.Instance.IsOpen;
        bool zulaAcik = ZulaUIManager.Instance != null && ZulaUIManager.Instance.IsOpen;

        _uiAcik = craftingAcik || ticaretAcik || zulaAcik || !hareketEdebilir;

        if (_uiAcik)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _anlikGirdi = Vector2.zero;
            _sprintBasildi = false;
            return;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // 2. YÜRÜME INPUT'u
        // 2. YÜRÜME INPUT'u AL ve KAYDET (Raw kullanarak kaymayý engelledik)
        _anlikGirdi.x = Input.GetAxisRaw("Horizontal");
        _anlikGirdi.y = Input.GetAxisRaw("Vertical");
        _sprintBasildi = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetButtonDown("Jump"))
        {
            _ziplamaBasildi = true;
        }

        // 3. FARE GÝRDÝSÝ VE KAMERA ROTASYONU
        float fareX = Input.GetAxis("Mouse X") * fareHassasiyeti * Time.deltaTime;
        float fareY = Input.GetAxis("Mouse Y") * fareHassasiyeti * Time.deltaTime;

        transform.Rotate(Vector3.up * fareX);
        _xRotasyon -= fareY;
        _xRotasyon = Mathf.Clamp(_xRotasyon, minBakýþAçýsý, maxBakýþAçýsý);

        // 4. C TUÞU ÝLE KAMERA MODU DEÐÝÞTÝRME (YENÝ)
        if (Input.GetKeyDown(KeyCode.C))
        {
            _isFirstPerson = !_isFirstPerson;
        }

        // 5. KAMERA POZÝSYONUNU VE ROTASYONUNU GÜNCELLE (YENÝ)
        if (kameraTransform != null)
        {
            // Pozisyonu yumuþakça (Lerp) kaydýr
            Vector3 hedefPozisyon = _isFirstPerson ? firstPersonPozisyon : thirdPersonPozisyon;
            kameraTransform.localPosition = Vector3.Lerp(kameraTransform.localPosition, hedefPozisyon, Time.deltaTime * kameraGecisHizi);

            // Dönüþü uygula
            kameraTransform.localRotation = Quaternion.Euler(_xRotasyon, 0f, 0f);
        }

        // 6. HOTBAR INPUT
        HotbarInputKontrol();
    }

    void FixedUpdate()
    {
        if (_uiAcik) return;

        bool hareketEdiyor = _anlikGirdi.x != 0 || _anlikGirdi.y != 0;
        bool kosabilirMi = _sprintBasildi && hareketEdiyor && _karakterDurum != null && _karakterDurum.StaminaVarMi();

        float mevcutHiz;

        if (kosabilirMi)
        {
            mevcutHiz = kosmaHizi;
            if (_karakterDurum != null) _karakterDurum.staminaKullaniliyor = true;
        }
        else
        {
            mevcutHiz = yürümeHizi;
            if (_karakterDurum != null) _karakterDurum.staminaKullaniliyor = false;
        }

        Vector3 hareket = transform.right * _anlikGirdi.x + transform.forward * _anlikGirdi.y;
        hareket = hareket.normalized * mevcutHiz;

        if (_controller.isGrounded)
        {
            if (_hizVektoru.y < 0)
            {
                _hizVektoru.y = -2f;
            }

            if (_ziplamaBasildi)
            {
                _hizVektoru.y = zýplamaKuvveti;
            }
        }

        _ziplamaBasildi = false;
        _hizVektoru.y -= yercekimi * Time.fixedDeltaTime;

        Vector3 sonHareket = hareket + _hizVektoru;
        _controller.Move(sonHareket * Time.fixedDeltaTime);
    }

    private void HotbarInputKontrol()
    {
        if (_envanter == null) return;

        bool slotDegisti = false;

        for (int i = 0; i < _envanter.hotbarBoyutu; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                _envanter.seciliSlotIndex = i;
                slotDegisti = true;
                break;
            }
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            if (scroll > 0f)
            {
                _envanter.seciliSlotIndex--;
                if (_envanter.seciliSlotIndex < 0)
                {
                    _envanter.seciliSlotIndex = _envanter.hotbarBoyutu - 1;
                }
            }
            else if (scroll < 0f)
            {
                _envanter.seciliSlotIndex++;
                if (_envanter.seciliSlotIndex >= _envanter.hotbarBoyutu)
                {
                    _envanter.seciliSlotIndex = 0;
                }
            }
            slotDegisti = true;
        }

        if (slotDegisti && InventoryUIManager.Instance != null)
        {
            InventoryUIManager.Instance.HighlightGuncelle();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            _envanter.SeciliEsyayiKullan();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            _envanter.SeciliEsyayiAt();
        }
    }
}