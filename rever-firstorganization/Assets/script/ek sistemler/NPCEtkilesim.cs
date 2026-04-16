using UnityEngine;

public class NPCEtkilesim : MonoBehaviour, IInteractable
{ 
    [Header("NPC Ayarlari")]
    public string npcIsmi = "Tüccar";
    
    public void Interact()
    {
        Debug.Log("Tüccarla konuþuluyor...");

        // Ticaret Panelini Aç
        if (TicaretUIManager.Instance != null)
        {
            // Eðer kapalýysa aç, açýksa kapat mantýðý
            if (!TicaretUIManager.Instance.IsOpen)
                TicaretUIManager.Instance.PaneliAc();
        }
    }

    // Ekranda ne yazsýn?
    public string GetInteractText()
    {
        return npcIsmi + " ile Konuþ";
    }
}