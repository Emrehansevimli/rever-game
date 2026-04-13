using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // Singleton (İstediğin scriptten InventoryManager.instance diyerek ulaşabilirsin)
    public static InventoryManager instance;

    [Header("Envanter Slotları")]
    // Inspector'daki InventoryGrid altındaki slotları buraya sürüklemiş olmalısın
    public InventorySlot[] inventorySlots;

    void Awake()
    {
        // Singleton ayarı
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // -----------------------------------------------------------------
    // 1. EŞYA EKLEME (Hasat yapınca veya Crafting sonucu çalışır)
    // -----------------------------------------------------------------
    public bool AddItem(Item itemToAdd, int amount)
    {
        // A. Önce var olan yığınları kontrol et (Üstüne ekleme)
        foreach (InventorySlot slot in inventorySlots)
        {
            // Eğer slot doluysa VE aynı eşyaysa
            if (slot.item == itemToAdd)
            {
                // Slotun limiti dolmadıysa (Örn: 64)
                if (slot.count < 64)
                {
                    slot.count += amount;
                    slot.UpdateSlotUI(); // Görüntüyü yenile
                    return true;
                }
            }
        }

        // B. Yığın yoksa veya doluysa BOŞ slot bul
        foreach (InventorySlot slot in inventorySlots)
        {
            // Eğer slot boşsa (Item yoksa veya sayısı 0 ise)
            if (slot.item == null || slot.count <= 0)
            {
                slot.item = itemToAdd;
                slot.count = amount;
                slot.UpdateSlotUI(); // Görüntüyü yenile
                return true;
            }
        }

        Debug.Log("⚠️ Envanter Dolu! Eşya eklenemedi.");
        return false;
    }

    // -----------------------------------------------------------------
    // 2. EŞYA SAYMA (Crafting yaparken 'Yeterli malzeme var mı?' diye bakar)
    // -----------------------------------------------------------------
    public int GetItemCount(Item itemToCount)
    {
        int total = 0;
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.item == itemToCount)
            {
                total += slot.count;
            }
        }
        return total;
    }

    // -----------------------------------------------------------------
    // 3. EŞYA SİLME (Crafting yaparken malzemeleri harcar)
    // -----------------------------------------------------------------
    public void RemoveItem(Item itemToRemove, int amountToRemove)
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            // Aradığımız eşya bu slotta mı?
            if (slot.item == itemToRemove)
            {
                if (slot.count >= amountToRemove)
                {
                    // Bu slotta yeterince var, hepsini buradan düş
                    slot.count -= amountToRemove;
                    amountToRemove = 0;
                }
                else
                {
                    // Bu slotta az var (Örn: biz 5 istiyoruz, burada 2 var)
                    // Hepsini al, kalanı diğer slottan aramaya devam et
                    amountToRemove -= slot.count;
                    slot.count = 0;
                }

                // Slot boşaldıysa temizle (İkonu kaldır)
                if (slot.count <= 0)
                {
                    slot.item = null;
                    slot.count = 0;
                }

                // Her değişimde görüntüyü güncelle
                slot.UpdateSlotUI();

                // Eğer silmemiz gereken miktar bittiyse döngüden çık
                if (amountToRemove <= 0) break;
            }
        }
    }
}