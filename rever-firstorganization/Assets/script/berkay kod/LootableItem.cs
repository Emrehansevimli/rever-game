using UnityEngine;

public class LootableItem : MonoBehaviour
{
    public Item itemData; // Inspector'dan Elma/Havuç itemini sürükle
    public int amount = 1;

    // İSMİ DEĞİŞTİRDİK: Interact -> CollectItem
    public void CollectItem()
    {
        // Singleton yapısı sayesinde direkt instance'a ulaşıyoruz, aramaya gerek yok.
        if (InventoryManager.instance != null)
        {
            // Eşyayı envantere eklemeyi dene
            bool basarili = InventoryManager.instance.AddItem(itemData, amount);

            if (basarili)
            {
                Debug.Log($"🎒 {itemData.itemName} toplandı!");
                Destroy(gameObject); // Yerden sil
            }
            else
            {
                Debug.Log("❌ Envanter dolu, alınamadı!");
            }
        }
        else
        {
            Debug.LogError("HATA: InventoryManager sahnede bulunamadı!");
        }
    }
}