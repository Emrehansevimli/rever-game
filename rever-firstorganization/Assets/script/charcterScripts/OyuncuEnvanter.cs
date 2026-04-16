using UnityEngine;

// Bu yapý, tek bir envanter yuvasýnýn verisini tutar.
[System.Serializable]
public struct EnvanterYuvasi
{
    public EsyaTipi tip;
    public int miktar;
}

public class OyuncuEnvanter : MonoBehaviour
{
    public int hotbarBoyutu = 8;
    public EnvanterYuvasi[] hotbarSlotlari;

    public int seciliSlotIndex = 0;
    private KarakterDurum _karakterDurum;

    void Awake()
    {
        hotbarSlotlari = new EnvanterYuvasi[hotbarBoyutu];
    }

    void Start()
    {
        _karakterDurum = GetComponent<KarakterDurum>();
    }

    //public bool EsyaEkle(EsyaTipi tip)
    //{
    //    EsyaVeriSO veri = InventoryUIManager.Instance.VeriGetir(tip);
    //    if (veri == null) return false;

    //    // 1. ADIM: Zaten var olan ve dolu olmayan bir yuvayý ara
    //    for (int i = 0; i < hotbarBoyutu; i++)
    //    {
    //        if (hotbarSlotlari[i].tip == tip && hotbarSlotlari[i].miktar < veri.maksIstifBoyutu)
    //        {
    //            hotbarSlotlari[i].miktar++;
    //            InventoryUIManager.Instance.EnvanteriGuncelle();
    //            return true;
    //        }
    //    }

    //    // 2. ADIM: Boþ bir yuva ara
    //    for (int i = 0; i < hotbarBoyutu; i++)
    //    {
    //        if (hotbarSlotlari[i].miktar == 0) // Boþ yuva = miktarý 0 olan yuva
    //        {
    //            hotbarSlotlari[i].tip = tip;
    //            hotbarSlotlari[i].miktar = 1;
    //            InventoryUIManager.Instance.EnvanteriGuncelle();
    //            return true;
    //        }
    //    }

    //    Debug.Log("Hotbar dolu, esya eklenemedi.");
    //    return false;
    //}
    public bool EsyaEkle(EsyaTipi tip, int miktar)
    {
        if (miktar <= 0) return true; // Eðer eklenen miktar 0 veya altýndaysa baþarýlý sayabiliriz.

        // 1. Mevcut Slotlarda Ýstifleme Kontrolü
        for (int i = 0; i < hotbarSlotlari.Length; i++)
        {
            // Ayný tipte eþya ve yuvada yer varsa
            if (hotbarSlotlari[i].tip == tip)
            {
                int maxIstif = InventoryUIManager.Instance.VeriGetir(tip).maxIstifBoyutu;
                int eklenebilecekBosluk = maxIstif - hotbarSlotlari[i].miktar;

                if (eklenebilecekBosluk > 0)
                {
                    int eklenecekMiktar = Mathf.Min(miktar, eklenebilecekBosluk);
                    hotbarSlotlari[i].miktar += eklenecekMiktar;
                    miktar -= eklenecekMiktar;
                    InventoryUIManager.Instance.EnvanteriGuncelle();
                    if (miktar == 0) return true; // Tüm eþyalar eklendi
                    
                }
                
            }
        }
        InventoryUIManager.Instance.EnvanteriGuncelle();
        // 2. Kalan Eþyayý Yeni Boþ Slotlara Ekleme
        for (int i = 0; i < hotbarSlotlari.Length && miktar > 0; i++)
        {
            if (hotbarSlotlari[i].tip == EsyaTipi.Bos)
            {
                int maxIstif = InventoryUIManager.Instance.VeriGetir(tip).maxIstifBoyutu;
                hotbarSlotlari[i].tip = tip;

                int eklenecekMiktar = Mathf.Min(miktar, maxIstif);
                hotbarSlotlari[i].miktar = eklenecekMiktar;
                miktar -= eklenecekMiktar;
                InventoryUIManager.Instance.EnvanteriGuncelle();
                if (miktar == 0) return true; // Tüm eþyalar eklendi
            }
        }

        // Eðer miktar > 0 ise, envanter doludur ve eþyanýn tamamý eklenememiþtir.
        // Baþarýlý saymak için ilk baþta eklenen eþyalarýn da kabul edilmesi gerekir,
        // ancak ticarette tam miktarýn alýnmasý beklenir.
        if (miktar > 0)
        {
            Debug.Log("Envanterde yeterli yer yok, satin alma iptal edildi.");
            
            return false;
        }

        InventoryUIManager.Instance.EnvanteriGuncelle();

        return true;
    }
    public bool EsyaCikar(EsyaTipi tip, int miktar)
    {
        if (miktar <= 0) return true;

        int toplamMiktar = 0;
        foreach (var slot in hotbarSlotlari)
        {
            if (slot.tip == tip)
            {
                toplamMiktar += slot.miktar;
            }
        }

        if (toplamMiktar < miktar)
        {
            return false; 
        }

        int kalanMiktar = miktar;
        for (int i = 0; i < hotbarSlotlari.Length && kalanMiktar > 0; i++)
        {
            if (hotbarSlotlari[i].tip == tip)
            {
                if (hotbarSlotlari[i].miktar >= kalanMiktar)
                {
                    hotbarSlotlari[i].miktar -= kalanMiktar;
                    if (hotbarSlotlari[i].miktar == 0)
                    {
                        hotbarSlotlari[i].tip = EsyaTipi.Bos;
                    }
                    kalanMiktar = 0;
                }
                else
                {
                    kalanMiktar -= hotbarSlotlari[i].miktar;
                    hotbarSlotlari[i].tip = EsyaTipi.Bos;
                    hotbarSlotlari[i].miktar = 0;
                }
            }
        }
        InventoryUIManager.Instance.EnvanteriGuncelle();
        return true;
    }
    public void SeciliEsyayiKullan()
    {
        EnvanterYuvasi seciliYuva = hotbarSlotlari[seciliSlotIndex];
        if (seciliYuva.miktar > 0)
        {
            EsyaVeriSO veri = InventoryUIManager.Instance.VeriGetir(seciliYuva.tip);
            if (veri != null && veri.kullanilabilir)
            {
                if (_karakterDurum != null) { _karakterDurum.CanArtir(veri.canEtkisi); }

                hotbarSlotlari[seciliSlotIndex].miktar--;
                if (hotbarSlotlari[seciliSlotIndex].miktar <= 0)
                {
                    hotbarSlotlari[seciliSlotIndex].tip = EsyaTipi.Bos;
                }
                InventoryUIManager.Instance.EnvanteriGuncelle();
            }
        }
    }

    public void SeciliEsyayiAt()
    {
        EnvanterYuvasi seciliYuva = hotbarSlotlari[seciliSlotIndex];
        if (seciliYuva.miktar > 0)
        {
            EsyaVeriSO veri = InventoryUIManager.Instance.VeriGetir(seciliYuva.tip);
            if (veri != null && veri.esyaModelPrefab != null)
            {
                Vector3 atmaPozisyonu = transform.position + transform.forward * 2f + Vector3.up * 0.5f;
                Instantiate(veri.esyaModelPrefab, atmaPozisyonu, Quaternion.identity);
            }

            hotbarSlotlari[seciliSlotIndex].miktar--;
            if (hotbarSlotlari[seciliSlotIndex].miktar <= 0)
            {
                hotbarSlotlari[seciliSlotIndex].tip = EsyaTipi.Bos;
            }
            InventoryUIManager.Instance.EnvanteriGuncelle();
        }
    }
}