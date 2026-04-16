using UnityEngine;

public class HayvanDurum : MonoBehaviour
{
    [Header("Saglik")]
    public float maksimumCan = 30f;
    private float _mevcutCan;

    [Header("Loot (Ganimet)")]
    public GameObject ganimetPrefab;

    [Header("Aclik Degerleri")]
    public float maksimumAclik = 100f;
    [SerializeField]
    public float mevcutAclik; // public olarak güncellendi!
    public float aclikTuketimHizi = 1f;
    private float _mevcutAclikTuketimHizi; // Anlık kullanılan hız
    public float aclikHasariHizi = 5f;
    public float aclikSiniri = 80f;

    [Header("Üretim Durumu")]
    [HideInInspector]
    public bool urunVermeyeHazir = false;

    private HayvanAI _yapayZeka;

    void Start()
    {
        _mevcutCan = maksimumCan;
        mevcutAclik = 0f;
        _mevcutAclikTuketimHizi = aclikTuketimHizi; // Başlangıçta normal hızı kullan
        _yapayZeka = GetComponent<HayvanAI>();
    }

    void Update()
    {
        // 1. Açlığı Artır / Azalt (Dinamik hız kullanılır)
        mevcutAclik += _mevcutAclikTuketimHizi * Time.deltaTime;
        mevcutAclik = Mathf.Clamp(mevcutAclik, 0f, maksimumAclik);

        // 2. Açlık Hasarını Uygula
        if (mevcutAclik >= aclikSiniri)
        {
            HasarAl(aclikHasariHizi * Time.deltaTime);
        }
    }

    // Barinak tarafından çağrılır
    public void AclikTuketimHiziniAyarla(float yeniHiz)
    {
        _mevcutAclikTuketimHizi = yeniHiz;
    }

    public void Beslen(float miktar)
    {
        mevcutAclik -= miktar;
        mevcutAclik = Mathf.Max(mevcutAclik, 0f);

        if (mevcutAclik <= 0f)
        {
            urunVermeyeHazir = true;
        }
    }

    public bool AcMi()
    {
        return mevcutAclik > (maksimumAclik / 2f);
    }

    // ... (HasarAl ve Olum metotları aynı kalsın)

    public void UrunVerildi()
    {
        urunVermeyeHazir = false;
    }

    public void HasarAl(float miktar)
    {
        if (_mevcutCan <= 0) return;

        _mevcutCan -= miktar;

        if (_mevcutCan <= 0)
        {
            Olum();
        }
    }

    void Olum()
    {
        if (_yapayZeka != null)
        {
            _yapayZeka.mevcutDurum = HayvanAI.HayvanDurumu.Oluden;
            _yapayZeka.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        }

        if (ganimetPrefab != null)
        {
            Instantiate(ganimetPrefab, transform.position + Vector3.up, Quaternion.identity);
        }

        Destroy(gameObject, 5f);
    }
}