using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Kapý Ayarlarý")]
    public float openAngle = 90f;
    public float closeAngle = 0f;
    public float rotationSpeed = 10f; // Hýzlý açýlýþ

    private bool isOpen = false;
    private Collider doorCollider;
    private float targetRotationY;

    void Start()
    {
        doorCollider = GetComponent<Collider>();
        targetRotationY = closeAngle;
    }

    void Update()
    {
        // Senin yazdýðýn harika yumuþak geçiþ kodu (Lerp)
        Quaternion targetRotation = Quaternion.Euler(0, targetRotationY, 0);

        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            targetRotation,
            Time.deltaTime * rotationSpeed
        );
    }

    // ÝSMÝ DEÐÝÞTÝRDÝK: Interact -> ToggleDoor
    // Artýk InteractionManager bu ismi bulup çalýþtýracak.
    public void ToggleDoor()
    {
        isOpen = !isOpen; // Durumu tersine çevir

        if (isOpen)
        {
            targetRotationY = openAngle;
            // Ýçinden geçebilmek için collider'ý kapatýyoruz
            if (doorCollider != null) doorCollider.enabled = false;
            Debug.Log("Kapý açýldý!");
        }
        else
        {
            targetRotationY = closeAngle;
            // Kapanýnca tekrar katý olsun
            if (doorCollider != null) doorCollider.enabled = true;
            Debug.Log("Kapý kapandý!");
        }
    }
}