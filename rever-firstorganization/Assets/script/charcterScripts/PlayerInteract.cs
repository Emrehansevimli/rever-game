using UnityEngine;
using TMPro; // UI Yazýsý için
using UnityEngine.UI;
public class PlayerInteract : MonoBehaviour
{
    [Header("Ayarlar")]
    public float rayDistance = 3f; // Ne kadar uzaktan etkileþime girebilirsin?
    public LayerMask interactLayer; // Hangi katmandaki objeleri görelim? (Player'ý görmesin diye)

    [Header("UI Referanslari")]
    public TextMeshProUGUI interactText; // Ekranda çýkan "E - Konuþ" yazýsý
    public GameObject interactUIObject; // Yazýnýn olduðu panel (Açýp kapatmak için)
    [Header("UI - Niþangah (Crosshair) Efekti")]
    public RectTransform crosshairRect;  // Niþangahýn boyutu için (Scale)
    public Image crosshairImage;         // Niþangahýn rengi için
    public Color normalRenk = Color.white;  // Boþluða bakarkenki renk
    public Color aktifRenk = Color.red;     // Eþyaya bakarkenki renk
    public float normalBoyut = 1f;          // Normal büyüklük
    public float aktifBoyut = 1.5f;
    private Camera _cam;

    void Start()
    {
        _cam = Camera.main; // Ana kamerayý bul
    }

    void Update()
    {
        Ray ray = new Ray(_cam.transform.position, _cam.transform.forward);
        RaycastHit hit;

        // Varsayýlan olarak etkileþim YOK kabul ediyoruz
        bool etkilesimVar = false;

        if (Physics.Raycast(ray, out hit, rayDistance, interactLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                // ETKÝLEÞÝM VAR!
                etkilesimVar = true;

                ShowInteractUI(true, interactable.GetInteractText());

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.Interact();
                }
            }
            else
            {
                ShowInteractUI(false, "");
            }
        }
        else
        {
            ShowInteractUI(false, "");
        }

        // --- NÝÞANGAH EFEKTÝNÝ GÜNCELLE ---
        CrosshairEfektiniYap(etkilesimVar);
    }

    void ShowInteractUI(bool isActive, string text)
    {
        if (interactUIObject != null)
        {
            interactUIObject.SetActive(isActive);
            if (interactText != null) interactText.text = text + " [E]";
        }
    }

    // YENÝ: Yumuþak geçiþ efekti
    void CrosshairEfektiniYap(bool aktifMi)
    {
        if (crosshairRect == null || crosshairImage == null) return;

        // Hedeflenen Deðerler
        float hedefBoyut = aktifMi ? aktifBoyut : normalBoyut;
        Color hedefRenk = aktifMi ? aktifRenk : normalRenk;

        // Lerp: Deðerleri aniden deðil, yavaþça deðiþtirir (Time.deltaTime * hýz)

        // 1. Boyutu deðiþtir
        crosshairRect.localScale = Vector3.Lerp(crosshairRect.localScale, Vector3.one * hedefBoyut, Time.deltaTime * 10f);

        // 2. Rengi deðiþtir
        crosshairImage.color = Color.Lerp(crosshairImage.color, hedefRenk, Time.deltaTime * 10f);
    }
}