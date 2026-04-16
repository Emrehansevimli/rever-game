using UnityEngine;
using UnityEngine.UI;

public class KarakterDurum : MonoBehaviour
{
    [Header("Durum Degerleri")]
    public float maksimumCan = 100f;
    public float mevcutIhtiyac = 0f;
    public float ihtiyacArtisHizi = 1f; // Saniye basina artis
    public float giderilmeMiktari = 100f; // Tek seferde giderilen miktar
    public Slider canBariSlider;
    public Slider staminaBariSlider;

    [Header("Maksimum Degerler")]
    private float _mevcutCan;
    public float maksimumIhtiyac = 100f;
    public float kritikSeviye = 80f;

    [Header("Stamina Degerleri")]
    public float maksimumStamina = 100f;
    [SerializeField]
    private float _mevcutStamina;
    public float staminaTuketimHizi = 20f; // Saniyede ne kadar harcayacaðý
    public float staminaYenilenmeHizi = 15f;
    [HideInInspector]
    public bool staminaKullaniliyor = false;

    [Header("Ekonomi")]
    [SerializeField]
    private int _mevcutPara = 100;
    public int MevcutPara => _mevcutPara;

    void Start()
    {
        // BURAYI UFAK REVÝZE ETTÝM: Elle 50 yazmak yerine maksimum neyse o baþlasýn.
        // Böylece Tank seçersen 200, Hýzlý seçersen 70 baþlar.
        _mevcutCan = maksimumCan;
        _mevcutStamina = maksimumStamina;

        GuncelleCanBari();
        GuncelleStaminaBari();
    }

    void Update()
    {
        mevcutIhtiyac += ihtiyacArtisHizi * Time.deltaTime;
        mevcutIhtiyac = Mathf.Min(mevcutIhtiyac, maksimumIhtiyac);
        StaminaYonetimi();
    }

    // --- YENÝ EKLENEN KISIM (Sadece burasý yeni) ---
    // Diðer script (CharacterController) burayý çaðýrýp özellikleri deðiþtirecek
    public void KarakterOzellikleriniAyarla(float yeniMaxCan, float yeniMaxStamina)
    {
        maksimumCan = yeniMaxCan;
        maksimumStamina = yeniMaxStamina;

        // Yeni özelliklere göre caný ve staminayý fulle
        _mevcutCan = maksimumCan;
        _mevcutStamina = maksimumStamina;

        // Barlarý güncelle ki oyuncu hemen görsün
        GuncelleCanBari();
        GuncelleStaminaBari();
    }
    // ------------------------------------------------

    public void ParaEkle(int miktar)
    {
        if (miktar <= 0) return;
        _mevcutPara += miktar;
        Debug.Log($"{miktar} para eklendi. Yeni miktar: {_mevcutPara}");
        if (EkonomiUIManager.Instance != null)
        {
            EkonomiUIManager.Instance.GuncelParayiGoster();
        }
    }

    public bool ParaCikar(int miktar)
    {
        if (miktar <= 0) return false;

        if (_mevcutPara >= miktar)
        {
            _mevcutPara -= miktar;
            Debug.Log($"{miktar} para harcandi. Kalan miktar: {_mevcutPara}");
            if (EkonomiUIManager.Instance != null)
            {
                EkonomiUIManager.Instance.GuncelParayiGoster();
            }
            return true;
        }

        Debug.Log("Yeterli paraniz yok!");
        return false;
    }

    private void GuncelleCanBari()
    {
        if (canBariSlider != null)
        {
            float canYuzdesi = _mevcutCan / maksimumCan;
            canBariSlider.value = canYuzdesi;
        }
    }

    private void StaminaYonetimi()
    {
        if (staminaKullaniliyor)
        {
            _mevcutStamina -= staminaTuketimHizi * Time.deltaTime;
        }
        else
        {
            _mevcutStamina += staminaYenilenmeHizi * Time.deltaTime;
        }
        _mevcutStamina = Mathf.Clamp(_mevcutStamina, 0f, maksimumStamina);

        GuncelleStaminaBari();
    }

    public bool StaminaVarMi()
    {
        return _mevcutStamina > 0.1f;
    }

    private void GuncelleStaminaBari()
    {
        if (staminaBariSlider != null)
        {
            float staminaYuzdesi = _mevcutStamina / maksimumStamina;
            staminaBariSlider.value = staminaYuzdesi;
        }
    }

    public bool IhtiyaciGider()
    {
        if (mevcutIhtiyac >= giderilmeMiktari * 0.1f)
        {
            mevcutIhtiyac = 0f;
            Debug.Log("Tuvalet ihtiyaci giderildi ve sifirlandi.");
            return true;
        }
        Debug.Log("Henüz tuvalet yapmaya yeterli ihtiyac yok.");
        return false;
    }

    public void CanArtir(float miktar)
    {
        if (miktar <= 0)
        {
            Debug.LogWarning("Can artirma miktari pozitif olmalidir.");
            return;
        }
        _mevcutCan += miktar;
        _mevcutCan = Mathf.Min(_mevcutCan, maksimumCan);
        Debug.Log($"{miktar} can kazanildi. Yeni can degeri: {_mevcutCan} / {maksimumCan}");
        GuncelleCanBari();
    }

    public void HasarAl(float hasar)
    {
        if (hasar <= 0) return;

        _mevcutCan -= hasar;

        GuncelleCanBari();

        if (_mevcutCan <= 0)
        {
            _mevcutCan = 0;
            Olüm();
        }

        Debug.Log($"{hasar} hasar alindi. Kalan can: {_mevcutCan}");
    }

    private void Olüm()
    {
        Debug.Log($"{gameObject.name} hayatini kaybetti!");
        GuncelleCanBari();
    }
}