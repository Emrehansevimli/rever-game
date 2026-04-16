using UnityEngine;
using TMPro; // TextMeshPro kullanýyorsanýz GEREKLÝ!

public class EkonomiUIManager : MonoBehaviour
{
    // Singleton (Tekil) Yöntemi: Diðer script'lerin kolay eriþimi için
    public static EkonomiUIManager Instance;

    [Header("UI Bilesenleri")]
    public TextMeshProUGUI paraMetni; // Inspector'dan baglanacak

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Oyun baþladýðýnda güncel parayý hemen ekrana yaz
        GuncelParayiGoster();
    }

    // KarakterDurum.cs tarafýndan çaðrýlacak metot
    public void GuncelParayiGoster()
    {
        // KarakterDurum script'ine eriþim
        GameObject oyuncu = GameObject.FindGameObjectWithTag("Player");
        if (oyuncu == null) return;

        KarakterDurum durum = oyuncu.GetComponent<KarakterDurum>();

        if (durum != null && paraMetni != null)
        {
            paraMetni.text = $"Para: {durum.MevcutPara}";
        }
    }
}