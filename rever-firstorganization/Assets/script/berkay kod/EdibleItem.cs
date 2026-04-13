using UnityEngine;

// IInteractable arayüzünü uyguluyoruz
public class EdibleItem : MonoBehaviour, IInteractable
{
    [Header("Yiyecek Özellikleri")]
    // Bu yiyeceðin ne kadar can vereceðini Inspector'dan ayarlayabilirsiniz
    public int healAmount = 25;

    public void Interact()
    {
        // Yiyeceði yiyen objenin (Player'ýn) HealthController'ýný bul
        // FindObjectOfType, sahnede bu türden ilk bulunan bileþeni döner.
        HealthController playerHealth = FindObjectOfType<HealthController>();

        if (playerHealth != null)
        {
            // HealthController'ýn ChangeHealth metodunu çaðýr ve can miktarýný gönder
            playerHealth.ChangeHealth(healAmount);

            // Yiyecek yendikten sonra, yiyecek objesini sahneden yok et
            Destroy(gameObject);

            Debug.Log(gameObject.name + " yendi ve " + healAmount + " can yenilendi.");
        }
        else
        {
            // Eðer oyuncuda HealthController yoksa hata mesajý ver
            Debug.LogError("HealthController bulunamadý! Can yenilenemedi. Player objesinde HealthController script'i ekli mi?");
        }
    }
}