using UnityEngine;

[CreateAssetMenu(fileName = "YeniKarakter", menuName = "Karakter/Karakter Verisi")]
public class KarakterVerisiSO : ScriptableObject
{
    [Header("Genel Bilgiler")]
    public string karakterAdi = "Ýsimsiz Kahraman";
    public string aciklama = "Dengeli bir karakter.";

    [Header("Hareket Özellikleri")]
    public float yurumeHizi = 6.0f;
    public float kosmaHizi = 10.0f;
    public float ziplamaKuvveti = 8.0f;

    [Header("Savaþ Özellikleri")]
    public float maxCan = 100f;
    public float stamina = 100f;
}