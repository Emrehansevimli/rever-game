using UnityEngine;

public class KapiKontrol : MonoBehaviour, IIletisim
{
    private bool _kapiAcik = false;

    [Header("Kapi Ayarlari")]
    public float acikAci = 90f;
    public float kapamaAci = 0f;
    public float dönüsHizi = 2f;

    private Quaternion _hedefRotasyon;

    void Start()
    {
        _hedefRotasyon = Quaternion.Euler(0, kapamaAci, 0);
    }

    void Update()
    {
        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            _hedefRotasyon,
            Time.deltaTime * dönüsHizi
        );
    }

    // IIletisim arayuzunden gelen metod
    public void IletisimeGec(GameObject etkilesen)
    {
        _kapiAcik = !_kapiAcik;

        if (_kapiAcik)
        {
            Ac();
        }
        else
        {
            Kapat();
        }
    }

    private void Ac()
    {
        _hedefRotasyon = Quaternion.Euler(0, acikAci, 0);
    }

    private void Kapat()
    {
        _hedefRotasyon = Quaternion.Euler(0, kapamaAci, 0);
    }
}