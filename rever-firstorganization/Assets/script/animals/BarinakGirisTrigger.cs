using UnityEngine;

public class BarinakGirisTrigger : MonoBehaviour
{
    private Barinak _barinakYonetici;

    void Start()
    {
        // Barinak.cs scripti bu objenin parent'ýnda olmalý (Ahýr objesinde)
        _barinakYonetici = GetComponentInParent<Barinak>();
        if (_barinakYonetici == null)
            Debug.LogError("BarinakGirisTrigger, Barinak.cs script'ine ulasamadi!");
    }

    void OnTriggerEnter(Collider other)
    {
        HayvanAI hayvan = other.GetComponent<HayvanAI>();
        if (hayvan != null)
        {
            // HayvanAI'a "Barýnaktasýn" bilgisini ver
            hayvan.barinaktaMi = true;

            // Barinak listesine ekle
            _barinakYonetici.HayvanEkle(hayvan);

            Debug.Log($"{hayvan.gameObject.name} barinaga girdi.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        HayvanAI hayvan = other.GetComponent<HayvanAI>();
        if (hayvan != null)
        {
            // HayvanAI'a "Dýþarýdasýn" bilgisini ver
            hayvan.barinaktaMi = false;

            // Barinak listesinden çýkar
            _barinakYonetici.HayvanCikar(hayvan);

            // Barýnaktan çýktýysa hemen dolaþmaya dönsün
            hayvan.mevcutDurum = HayvanAI.HayvanDurumu.Dolasiyor;

            Debug.Log($"{hayvan.gameObject.name} barinaktan cikti.");
        }
    }
}