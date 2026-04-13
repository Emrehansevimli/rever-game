using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Yeni Tarif", menuName = "Crafting/Tarif")]
public class Recipe : ScriptableObject
{
    [Header("Sonuç (Ne Üretilecek?)")]
    public Item craftedItem;      // Üretilen Eþya (Örn: Altýn Elma)
    public int amountCrafted = 1; // Kaç tane verilecek?

    [Header("Gerekli Malzemeler")]
    public List<Ingredient> ingredients; // Malzeme Listesi

    // Bu küçük yapý sayesinde Inspector'da Item ve Sayý yan yana duracak
    [System.Serializable]
    public struct Ingredient
    {
        public Item item;   // Hangi eþya? (Örn: Elma)
        public int amount;  // Kaç tane? (Örn: 2)
    }
}