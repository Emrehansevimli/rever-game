using UnityEngine;

// Bu script, hayvanlarýn yiyeceði objelere (örneðin çim objesine) takýlmalýdýr.
public class YiyecekKaynagi : MonoBehaviour
{
    [Header("Besin Degerleri")]
    // Hayvanin ne kadar açlýðýný gidereceði
    public float besinDegeri = 100f;

    // Tüketimden sonra yiyecek kaynagi ne olacak?
    public bool yokOlsunMu = true;

    // Hayvan bu yiyecegi tükettikten sonra bu metot çaðrýlacak
    public float Tuket()
    {
        if (yokOlsunMu)
        {
            Destroy(gameObject); // Yiyeceði yok et (Tavuk yemi bitirdi)
        }

        return besinDegeri;
    }
}