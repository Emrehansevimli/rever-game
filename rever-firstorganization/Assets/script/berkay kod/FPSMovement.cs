using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSMovement : MonoBehaviour
{
    [Header("Ayarlar")]
    public float walkSpeed = 6.0f;
    public float mouseSensitivity = 2.0f;
    public float gravity = -9.81f; // Gerçek yerçekimi değeri

    [Header("Bağlantılar")]
    public GameObject inventoryPanel;

    private CharacterController controller;
    private float xRotation = 0f;
    private Vector3 velocity; // Düşüş hızını hafızada tutmak için

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Mouse'u kilitle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Kamerayı sabitle
        if (Camera.main != null)
        {
            Camera.main.transform.SetParent(transform);
            Camera.main.transform.localPosition = new Vector3(0, 1.6f, 0);
            Camera.main.transform.localRotation = Quaternion.identity;
        }
    }

    void Update()
    {
        // --- 1. MOUSE (SADECE BAKIŞ) ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (Camera.main != null)
        {
            Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        transform.Rotate(Vector3.up * mouseX);


        // --- 2. KLAVYE (HER ZAMAN YÜRÜME) ---
        // Artık 'if(isGrounded)' şartı yok, havada da yürüyebilirsin.
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * walkSpeed * Time.deltaTime);


        // --- 3. YERÇEKİMİ (FİZİKSEL DÜŞÜŞ) ---
        // Yerdeysek düşüş hızını sıfırla (hafif bastır ki havada kalmasın)
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Hızı yerçekimi kadar artır (Hızlanarak düşersin)
        velocity.y += gravity * Time.deltaTime;

        // Düşüşü uygula
        controller.Move(velocity * Time.deltaTime);


        // --- 4. ENVANTER ---
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryPanel != null)
            {
                bool isActive = !inventoryPanel.activeSelf;
                inventoryPanel.SetActive(isActive);
                Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = isActive;
            }
        }
    }
}