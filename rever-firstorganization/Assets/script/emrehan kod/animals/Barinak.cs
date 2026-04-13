// Barinak.cs

using UnityEngine;
using System.Collections.Generic;

public class Barinak : MonoBehaviour
{
    [Header("Ayarlar")]
    public int maksimumHayvanSayisi = 5;
    public float barinakIciAclikHizi = -500f;

    [Header("Mevcut Hayvanlar")]
    public List<HayvanAI> iceridekiHayvanlar = new List<HayvanAI>();

    public void HayvanEkle(HayvanAI hayvan)
    {
        HayvanDurum durum = hayvan.GetComponent<HayvanDurum>();

        if (durum != null && iceridekiHayvanlar.Count < maksimumHayvanSayisi && !iceridekiHayvanlar.Contains(hayvan))
        {
            iceridekiHayvanlar.Add(hayvan);

            // ÇÖZÜM 1: Açlık hızını negatif yaparak hayvanın doymasını sağla.
            durum.AclikTuketimHiziniAyarla(barinakIciAclikHizi);
        }
    }

    public void HayvanCikar(HayvanAI hayvan)
    {
        HayvanDurum durum = hayvan.GetComponent<HayvanDurum>();

        if (iceridekiHayvanlar.Contains(hayvan))
        {
            iceridekiHayvanlar.Remove(hayvan);

            // GÜNCELLEME: Açlık hızını normale döndür
            if (durum != null)
            {
                // Normal hızını tekrar alır
                durum.AclikTuketimHiziniAyarla(durum.aclikTuketimHizi);
            }
        }
    }
}
