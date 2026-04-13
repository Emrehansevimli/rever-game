using UnityEngine;

// Bu script, sahnede duran Zula (Sandýk/Depo) objesine takýlacak.
public class Zula : MonoBehaviour, IIletisim
{
    [Header("Depo Ayarlari")]
    public int zulaBoyutu = 16; // Zulanýn kaç slotu olacaðý
    public EnvanterYuvasi[] zulaSlotlari; // Eþyalarý tutan dizi

    void Awake()
    {
        // Zula envanterini baþlat
        zulaSlotlari = new EnvanterYuvasi[zulaBoyutu];
    }

    // IIletisim arayüzünden gelen metot (trigerscirpts.cs bunu çaðýracak)
    public void IletisimeGec(GameObject etkilesen)
    {
        // Karakterin envanterini al
        OyuncuEnvanter oyuncuEnvanteri = etkilesen.GetComponent<OyuncuEnvanter>();
        if (oyuncuEnvanteri == null)
        {
            Debug.LogError("Etkilesen objede OyuncuEnvanter scripti bulunamadi!");
            return;
        }

        // Zula UI Yöneticisine haber ver: UI'ý aç!
        // Hem oyuncunun envanterini hem de bu Zula'nýn envanterini göndeririz.
        if (ZulaUIManager.Instance != null)
        {
            ZulaUIManager.Instance.ZulaUI_Ac(oyuncuEnvanteri, this);
        }
    }
}