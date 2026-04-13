using UnityEngine;

public class TwoKeyInteractionManager : MonoBehaviour
{
    [Header("Ayarlar")]
    public float interactionDistance = 3f; // Ne kadar uzaktan basabilirsin?
    public KeyCode lootKey = KeyCode.F;    // Toplama Tuþu
    public KeyCode actionKey = KeyCode.E;  // Kapý/Etkileþim Tuþu

    [Header("UI")]
    // Buraya ilerde "E'ye bas" yazýsý ekleyebiliriz, þimdilik boþ kalsýn.
    public GameObject interactionUI;

    private Camera cam;

    void Start()
    {
        cam = Camera.main; // Ana kamerayý bul
    }

    void Update()
    {
        // Her karede önümüzü kontrol edelim
        CheckInteraction();
    }

    void CheckInteraction()
    {
        RaycastHit hit;

        Vector3 rayOrigin = cam.transform.position;
        Vector3 rayDirection = cam.transform.forward;

        // Debug Çizgisi: Scene ekranýnda kýrmýzý bir çizgi görürsün.
        Debug.DrawRay(rayOrigin, rayDirection * interactionDistance, Color.red);

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, interactionDistance))
        {
            // --- DEÐÝÞÝKLÝK BURADA BAÞLIYOR ---
            // 'GetComponent' yerine 'GetComponentInParent' kullanýyoruz.
            // Bu sayede nesnenin çocuklarýna (modeline) týklasan bile ana objeyi bulur.

            // 1. TARLA KONTROLÜ (F TUÞU)
            CropController crop = hit.collider.GetComponentInParent<CropController>();
            if (crop != null)
            {
                if (Input.GetKeyDown(lootKey))
                {
                   // crop.OnInteract(); // Ekmek veya toplamak için
                    Debug.Log("Tarlayla etkileþime geçildi!");
                }
            }

            // SoilController (Senin kodunda vardý, aynen korudum)
            SoilController soil = hit.collider.GetComponentInParent<SoilController>();
            if (soil != null)
            {
                if (Input.GetKeyDown(lootKey))
                {
                    // SoilController iþlemleri buraya...
                    Debug.Log("Toprak görüldü.");
                }
            }

            // 2. KAPI KONTROLÜ (E TUÞU)
            DoorController door = hit.collider.GetComponentInParent<DoorController>();
            if (door != null)
            {
                if (Input.GetKeyDown(actionKey))
                {
                    door.ToggleDoor(); // Kapýyý aç/kapa
                    Debug.Log("Kapý týklandý!");
                }
            }

            // 3. EÞYA (LOOT) KONTROLÜ
            LootableItem loot = hit.collider.GetComponentInParent<LootableItem>();
            if (loot != null)
            {
                if (Input.GetKeyDown(lootKey))
                {
                    loot.CollectItem();
                }
            }
        }
    }
}