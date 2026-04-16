using UnityEngine;

// DÝKKAT: Artýk IInteractable sözleþmesini kullanýyor!
public class trigerscripts : MonoBehaviour, IInteractable
{
    [Header("UI & Ayarlar")]
    public string iletisimMesaji = "Kullan"; // Mesajý buraya yaz (Örn: Kapýyý Aç)

    // Senin eski sistemindeki asýl iþi yapan arayüz (Kapý, NPC, Eþya scriptleri)
    private IIletisim _iletisimHedefi;

    private void Start()
    {
        // Bu objenin üzerindeki asýl yetenekli scripti bul (Örn: KapiAcmaScripti)
        _iletisimHedefi = GetComponent<IIletisim>();

        if (_iletisimHedefi == null)
        {
            Debug.LogError($"HATA: {gameObject.name} objesinde 'IIletisim' kullanan bir script yok!");
        }
    }

    // --- RAYCAST SÝSTEMÝNÝN ÇAÐIRDIÐI FONKSÝYONLAR ---

    // 1. Oyuncu 'E'ye bastýðýnda bu çalýþacak
    public void Interact()
    {
        if (_iletisimHedefi != null)
        {
            // Senin eski sistemin çalýþmak için "Player" objesini istiyor.
            // Raycast sisteminde 'other' olmadýðý için oyuncuyu Tag ile buluyoruz.
            GameObject oyuncu = GameObject.FindGameObjectWithTag("Player");

            if (oyuncu != null)
            {
                // ESKÝ SÝSTEMÝ TETÝKLE
                _iletisimHedefi.IletisimeGec(oyuncu);
            }
            else
            {
                Debug.LogError("HATA: Sahnede 'Player' tag'ine sahip bir oyuncu yok!");
            }
        }
    }

    // 2. Oyuncu buna baktýðýnda ekranda ne yazsýn?
    public string GetInteractText()
    {
        return iletisimMesaji;
    }
}