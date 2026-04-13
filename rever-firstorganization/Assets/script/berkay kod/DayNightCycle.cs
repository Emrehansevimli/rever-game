using UnityEngine;
using System; // Action (Event) kullanmak için gerekli

public class DayNightCycle : MonoBehaviour
{
    [Header("Ayarlar")]
    public Light sun; // Güneş ışığı (Directional Light)
    [Tooltip("Bir oyun günü kaç saniye sürsün?")]
    public float dayDuration = 60f; // Varsayılan 60 saniye = 1 Gün

    // Bu satır, diğer scriptlerin (Bitkilerin) dinleyeceği "Duyuru" sistemidir.
    public static Action OnDayChanged;

    private float timeOfDay = 0f; // 0 ile 1 arasında ilerler (0:Sabah, 0.5:Akşam, 1:Gece sonu)

    void Update()
    {
        // Zamanı ilerlet
        timeOfDay += Time.deltaTime / dayDuration;

        // Güneşi döndür (X ekseninde 360 derece döner)
        if (sun != null)
        {
            sun.transform.localRotation = Quaternion.Euler((timeOfDay * 360f) - 90f, 170f, 0);
        }

        // Gün bitti mi? (1'e ulaştı mı?)
        if (timeOfDay >= 1f)
        {
            timeOfDay = 0f; // Zamanı başa sar

            // "GÜN DEĞİŞTİ" DUYURUSUNU YAP!
            if (OnDayChanged != null)
            {
                OnDayChanged.Invoke();
                Debug.Log("🌞 Gün doğdu! Bitkiler büyüyor...");
            }
        }
    }

    // Test amaçlı: Editörden sağ tıklayıp günü zorla bitirebilirsin
    [ContextMenu("Test: Günü Hızlıca Bitir")]
    public void ForceDayChange()
    {
        if (OnDayChanged != null)
        {
            OnDayChanged.Invoke();
            Debug.Log("🧪 Test: Gün zorla değiştirildi.");
        }
    }
}