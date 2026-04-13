using UnityEngine;

// Item türlerini tanýmlayan Enum (Artýk sýnýfýn dýþýnda, public ve görünür!)
public enum ItemType
{
    DEFAULT,
    FOOD,
    CRAFTING_MATERIAL,
}

// Bu, bir Item (Nesne) veri þablonudur
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
public class Item : ScriptableObject
{
    [Header("Eþya Bilgileri")]
    public string itemName = "New Item";
    public Sprite icon = null;
    public ItemType type = ItemType.DEFAULT; // Açýlýr menü kesinlikle burada görünecektir

    [Header("Yenilebilirlik")]
    public float healthRestoreValue = 0f;

    [Header("Büyü (Buff) Özelliði")]
    public bool hasBuff = false;
    public float buffAmount = 0f;
    public float buffDuration = 0f;

    // Bu metot Food/HealthController tarafýndan çaðrýlýr
    public void UseItem(HealthController healthController)
    {
        if (type == ItemType.FOOD)
        {
            healthController.ChangeHealth(healthRestoreValue);
            Debug.Log(itemName + " kullanýldý. Can yenileme deðeri: " + healthRestoreValue);
        }
    }
}