using UnityEngine;
using System.Collections.Generic;

public class CraftingManager : MonoBehaviour
{
    private InventoryManager inventoryManager;

    [Header("Mevcut Tarifler")]
    public List<Recipe> availableRecipes; // Inspector'dan buraya tarifleri sürükleyeceksin

    void Start()
    {
        // Manager'ý buluyoruz
        GameObject managerObject = GameObject.Find("GAME_MANAGER");
        if (managerObject != null)
        {
            inventoryManager = managerObject.GetComponent<InventoryManager>();
        }

        if (inventoryManager == null)
        {
            Debug.LogError("HATA: CraftingManager, InventoryManager'ý bulamadý!");
        }
    }

    void Update()
    {
        // 'C' tuþuna basýnca elimizdeki malzemelerle yapýlabilecek her þeyi yapar
        if (Input.GetKeyDown(KeyCode.C))
        {
            TryCraftAllRecipes();
        }
    }

    public void TryCraftAllRecipes()
    {
        if (inventoryManager == null) return;

        bool craftedAny = false;

        // Listendeki tüm tarifleri tek tek dene
        foreach (var recipe in availableRecipes)
        {
            // Eðer bu tarifi yapabiliyorsak (Malzeme varsa)
            if (CanCraft(recipe))
            {
                ExecuteCraft(recipe);
                craftedAny = true;
                // Bir tane üretince duralým mý, yoksa hepsini mi üretsin?
                // Þimdilik 'break' koyuyorum, tek seferde 1 iþlem yapsýn.
                break;
            }
        }

        if (!craftedAny)
        {
            Debug.LogWarning("Üretim Baþarýsýz: Yeterli malzeme yok veya tarif listesi boþ.");
        }
    }

    // YENÝLENDÝ: Yeni Recipe yapýsýna uygun kontrol
    private bool CanCraft(Recipe recipe)
    {
        // Tarifin içindeki "ingredients" listesine bakýyoruz
        foreach (Recipe.Ingredient ingredient in recipe.ingredients)
        {
            // Çantada bu eþyadan (ingredient.item) kaç tane var?
            int currentAmount = inventoryManager.GetItemCount(ingredient.item);

            // Eðer çantadaki, gerekenden azsa -> ÜRETEMEZSÝN
            if (currentAmount < ingredient.amount)
            {
                return false;
            }
        }
        return true;
    }

    // YENÝLENDÝ: Yeni Recipe yapýsýna uygun üretim
    private void ExecuteCraft(Recipe recipe)
    {
        // 1. Malzemeleri Çantadan Sil
        foreach (Recipe.Ingredient ingredient in recipe.ingredients)
        {
            inventoryManager.RemoveItem(ingredient.item, ingredient.amount);
        }

        // 2. Yeni Eþyayý Çantaya Ekle
        inventoryManager.AddItem(recipe.craftedItem, recipe.amountCrafted);

        Debug.Log("CRAFTING BAÞARILI: " + recipe.craftedItem.itemName + " oluþturuldu!");
    }
}