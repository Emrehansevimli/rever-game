using UnityEngine;
using UnityEngine.UI; // UI elemanlarý (Resim, Yazý) için gerekli

public class InventorySlot : MonoBehaviour
{
    [Header("Veriler")]
    public Item item;   // Bu slotta hangi eþya var?
    public int count;   // Kaç tane var?

    [Header("UI Baðlantýlarý")]
    public Image icon;        // Eþyanýn resmi (Unity'de sürükleyeceksin)
    public Text countText;    // Sayý yazýsý (Unity'de sürükleyeceksin)

    // Bu fonksiyonu InventoryManager çaðýracak
    public void UpdateSlotUI()
    {
        if (item != null)
        {
            // Eþya varsa resmini koy ve aç
            icon.sprite = item.icon;
            icon.enabled = true;

            // Sayý 1'den büyükse göster, yoksa gizle
            if (count > 1)
            {
                countText.text = count.ToString();
                countText.enabled = true;
            }
            else
            {
                countText.enabled = false;
            }
        }
        else
        {
            // Eþya yoksa her þeyi gizle
            icon.sprite = null;
            icon.enabled = false;
            countText.enabled = false;
        }
    }
}