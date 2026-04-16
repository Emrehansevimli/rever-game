using UnityEngine;

// Bu ozellik, Unity Editor'de Assets menusu altinda yeni tarif olusturmamizi saglar.
[CreateAssetMenu(fileName = "YeniTarif", menuName = "Crafting/Yeni Tarif")]
public class TarifSO : ScriptableObject
{
    [Header("Üretilen Esya")]
    public EsyaTipi sonucEsya; // Üretim sonucunda elde edilecek esya
    public int sonucMiktari = 1;

    [Header("Gereken Malzemeler")]
    // Uretim icin gereken esya listesi
    public EsyaGereksinimi[] malzemeler;

    // Uretim icin gereken esya tipini ve miktarini tutan yapi
    [System.Serializable]
    public struct EsyaGereksinimi
    {
        public EsyaTipi tip;
        public int miktar;
    }
}