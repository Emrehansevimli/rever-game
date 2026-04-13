using UnityEngine;

public class HealthController : MonoBehaviour
{
    [Header("Can Ayarlarý")]
    public float maxHealth = 100f; // Maksimum can deðeri
    public float currentHealth;    // Mevcut can deðeri

    void Start()
    {
        // Oyun baþladýðýnda caný maksimuma ayarla
        currentHealth = maxHealth;
        Debug.Log("Can sistemi baþlatýldý. Mevcut Can: " + currentHealth);

        // GEÇÝCÝ HASAR ALMA TESTÝ: 
        // Caný düþürelim ki elma yiyince yükseldiðini görelim.
        ChangeHealth(-50f); // 50 hasar al
    }

    // Dýþarýdan can eklemek veya hasar almak için kullanýlacak public metot
    public void ChangeHealth(float amount)
    {
        Debug.Log("Can deðiþimi geliyor: " + amount);

        // Mevcut caný deðiþtir
        currentHealth += amount;

        // Caný maksimum can deðeri ile 0 arasýnda tut
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Yeni Can: " + currentHealth);

        // Can 0'ýn altýna düþerse yapýlacaklar (Ölüm)
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " öldü!");
        // Karakteri yok et veya oyunu bitir
        // Destroy(gameObject);
        // Time.timeScale = 0; // Oyunu durdur
    }

    void Update()
    {
        // Gerekirse burasý boþ kalýr
    }
}