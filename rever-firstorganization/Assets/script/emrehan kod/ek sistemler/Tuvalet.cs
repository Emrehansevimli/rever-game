using UnityEngine;

// IIletisim arayuzunu uyguluyoruz
public class Tuvalet : MonoBehaviour, IIletisim
{
    // Arayuzden gelen zorunlu metot
    public void IletisimeGec(GameObject etkilesen)
    {
        // Tuvalet objesine giren karakterin KarakterDurum scriptini al
        KarakterDurum durum = etkilesen.GetComponent<KarakterDurum>();

        if (durum != null)
        {
            // Ihtiyaci giderme islemini baslat (KarakterDurum scriptine komut gonder)
            bool basarili = durum.IhtiyaciGider();

            if (basarili)
            {
                Debug.Log($"{etkilesen.name} tuvaletini yapti.");
                // Burada bir animasyon/ses efekti baslatilabilir.
            }
            else
            {
                Debug.Log($"{etkilesen.name}: Henuz tuvalet yapmaya gerek yok.");
            }
        }
        else
        {
            Debug.LogError("Etkilesen objede 'KarakterDurum' scripti bulunamadi!");
        }
    }
}