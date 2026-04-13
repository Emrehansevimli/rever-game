using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnvanterYuvaUI : MonoBehaviour
{
    // Inspector'da bu alanlara, Prefab icindeki dogru bilesenleri sürükleyip birakacagiz.
    public Image ikonResmi;
    public TextMeshProUGUI miktarMetni;

    // YENÝ EKLENEN: Esya ismini gosterecek metin
    public TextMeshProUGUI isimMetni;
    public GameObject secimCercevesi;
 
    public void YuvayiGuncelle(EsyaVeriSO esyaVerisi, int miktar)
    {
        if (esyaVerisi != null)
        {
            // ÝKONU AYARLA
            if (ikonResmi != null && esyaVerisi.ikon != null)
            {
                ikonResmi.sprite = esyaVerisi.ikon;
                ikonResmi.color = Color.white;
            }

            // MÝKTARI AYARLA
            if (miktarMetni != null)
            {
                miktarMetni.text = miktar > 1 ? miktar.ToString() : "";
            }

            // YENÝ EKLENEN: ÝSMÝ AYARLA
            if (isimMetni != null)
            {
                // EsyaVeriSO icindeki 'gorunurAd' degiskenini kullaniriz.
                isimMetni.text = esyaVerisi.gorunurAd;
            }
        }
        else
        {
            Temizle();
        }
    }
    public void SecimiAyarla(bool seciliMi)
    {
        if (secimCercevesi != null)
        {
            secimCercevesi.SetActive(seciliMi);
        }
    }
    /// <summary>
    /// Yuvayi bosaltir (ikonu ve metinleri gizler).
    /// </summary>
    public void Temizle()
    {
        if (ikonResmi != null)
        {
            ikonResmi.sprite = null;
            ikonResmi.color = new Color(1, 1, 1, 0);
        }

        if (miktarMetni != null)
        {
            miktarMetni.text = "";
        }

        // YENÝ EKLENEN: Isim metnini de temizle
        if (isimMetni != null)
        {
            isimMetni.text = "";
        }
    }
}