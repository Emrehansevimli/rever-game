using UnityEngine;

public class NPCEtkilesim : MonoBehaviour, IInteractable
{
    ////[Header("Etkilesim Ayarlari")]
    ////public float etkilesimMesafesi = 3f; // Oyuncunun ne kadar yakýnda olmasý gerektiði
    ////public KeyCode etkilesimTusu = KeyCode.E; // Ticareti açacak tuþ

    //[Header("Referanslar")]
    //// TicaretUIManager'ý doðrudan sahneden bulacaðýz
    //public TicaretUIManager uiManagerReferans;
    //private Transform _oyuncu;
    [Header("NPC Ayarlari")]
    public string npcIsmi = "Tüccar";
    //void Start()
    //{
    //    // Manager'ý sahnede bul
    //    //uiManagerReferans = uiManagerReferans;
    //    if (uiManagerReferans == null)
    //    {
    //        Debug.LogError("NPCEtkilesim: UI Manager referansi Inspector'dan baglanmadi!");
    //        enabled = false;
    //    }
    //    // Oyuncu objesini Tag ile bul
    //    GameObject oyuncuObjesi = GameObject.FindGameObjectWithTag("Player");
    //    if (oyuncuObjesi != null)
    //    {
    //        Debug.LogError("Sahnede 'Player' tag'!");
    //        _oyuncu = oyuncuObjesi.transform;
    //    }
    //    else
    //    {
    //        Debug.LogError("Sahnede 'Player' tag'ine sahip obje bulunamadi!");
    //        enabled = false;
    //    }
    //    // Oyuncu objesini Tag ile bul
        
    //}

    //void Update()
    //{
    //    if (uiManagerReferans == null || _oyuncu == null)
    //    {
    //        // NPC veya UI Manager referansý alýnamadýysa bu log görünür.
    //        //Debug.LogError("NPCEtkilesim: UI Manager veya Oyuncu referansi alýnamadý!");
    //        return;
    //    }
    //    // Eðer UI Manager yoksa veya oyuncu yoksa iþlem yapma
    //    if (uiManagerReferans == null || _oyuncu == null) return;
    //    if (uiManagerReferans.IsOpen) return;
    //    // Oyuncu ile NPC arasýndaki mesafeyi hesapla
    //    float mesafe = Vector3.Distance(_oyuncu.position, transform.position);
    //    if (mesafe <= etkilesimMesafesi)
    //    {
    //        if (Input.GetKeyDown(etkilesimTusu))
    //        {
    //            Debug.Log("Etkileþim tuþuna basýldý. Paneli açýyorum.");
    //            uiManagerReferans.PaneliAc();
    //        }
    //        if (Input.GetKeyDown(etkilesimTusu))
    //        {
    //            uiManagerReferans.PaneliAc();
    //        }
    //    }
    //}

   
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