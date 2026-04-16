using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    // Singleton eriþim noktasý
    public static UIManager Instance;

    // Tek bir UI metin objesini burada tutarýz
    public TextMeshProUGUI noticeText;

    private void Awake()
    {
        // Singleton yapýsý: Instance'ý bu obje olarak ayarla
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Harici scriptlerin metni göstermesi için metod
    public void UyariyiGoster(string mesaj)
    {
        if (noticeText != null)
        {
            noticeText.text = mesaj;
            noticeText.gameObject.SetActive(true);
        }
    }

    // Harici scriptlerin metni gizlemesi için metod
    public void UyariyiGizle()
    {
        if (noticeText != null)
        {
            noticeText.gameObject.SetActive(false);
        }
    }
}