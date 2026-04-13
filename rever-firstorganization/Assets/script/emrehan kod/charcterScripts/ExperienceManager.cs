using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager Instance; // Singleton (Her yerden ulaþalým)

    [Header("Mevcut Durum")]
    public int currentLevel = 1;
    public float currentXP = 0;
    public float targetXP = 100; // Ýlk seviye için gereken XP

    [Header("Ayarlar")]
    public float xpMultiplier = 1.2f; // Her levelde zorluk %20 artsýn

    [Header("UI Referanslarý")]
    public Slider xpSlider;
    public TextMeshProUGUI levelText;

    [Header("Efektler (Opsiyonel)")]
    public AudioClip levelUpSound;
    public AudioSource audioSource;

    private void Awake()
    {
        // Singleton ayarý
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI(); // Baþlangýçta barý ayarla
    }

    // --- DIÞARIDAN ÇAÐRILACAK FONKSÝYON ---
    public void AddXP(float amount)
    {
        currentXP += amount;

        // Level atlama kontrolü (Belki tek seferde 2 level atlar diye while döngüsü)
        while (currentXP >= targetXP)
        {
            LevelUp();
        }

        UpdateUI();
    }

    void LevelUp()
    {
        // XP'yi sýfýrlama, artaný sonraki levele aktar
        currentXP -= targetXP;

        // Leveli arttýr
        currentLevel++;

        // Hedef XP'yi zorlaþtýr (Örn: 100 -> 120 -> 144...)
        targetXP = targetXP * xpMultiplier;

        Debug.Log("TEBRÝKLER! Level " + currentLevel + " oldun!");

        // Ses çal
        if (audioSource != null && levelUpSound != null)
        {
            audioSource.PlayOneShot(levelUpSound);
        }

        // BURAYA ÝLERÝDE: Can doldurma, stat arttýrma vb. eklenecek.
    }

    void UpdateUI()
    {
        if (xpSlider != null)
        {
            xpSlider.maxValue = targetXP;
            xpSlider.value = currentXP;
        }

        if (levelText != null)
        {
            levelText.text = "Lvl " + currentLevel;
        }
    }
}