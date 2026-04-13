using UnityEngine;
using UnityEngine.AI;
using System.Linq; // LINQ metotlarý için

[RequireComponent(typeof(NavMeshAgent))]
public class HayvanAI : MonoBehaviour
{
    // Hayvanýn o anki ruh hali
    public enum HayvanDurumu { Dolasiyor, Kovaliyor, Kaciyor, YiyecekAriyor, BarinagaGit, Oluden }

    [Header("Ayarlar")]
    public HayvanDurumu mevcutDurum;
    public bool saldirganMi = false;
    public float gorusMesafesi = 10f;
    public float dolasmaYaricapi = 20f;
    private YiyecekKaynagi _hedefYiyecek;

    [Header("Referanslar")]
    private NavMeshAgent _agent;
    private Transform _oyuncu;
    private Animator _animator;
    private HayvanDurum _durum;
    private HayvanUretim _uretim;

    [Header("Barinak Ayarlari")]
    [HideInInspector]
    public bool barinaktaMi = false;
    public float barinagaDonmeAclikSeviyesi = 30f;
    private Barinak _hedefBarinak;

    [Header("Yiyecek Arama Ayarlari")]
    public float yiyecekAramaBeklemeSuresi = 5f;
    private float _sonAramaZamani;

    // Gerekirse _durum.mevcutAclik deðerini buradan okumak için
    public float mevcutAclikDegeri;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
        _uretim = GetComponent<HayvanUretim>();

        GameObject oyuncuObj = GameObject.FindGameObjectWithTag("Player");
        if (oyuncuObj != null) _oyuncu = oyuncuObj.transform;
        else Debug.LogError("Oyuncu Tag'i 'Player' olarak ayarlanmali!");

        _durum = GetComponent<HayvanDurum>();

        YeniHedefBelirle();
    }

    void Update()
    {
        if (_durum != null)
            mevcutAclikDegeri = _durum.mevcutAclik; // Açlýk deðerini güncel tut

        // GÜVENLÝK KONTROLÜ: NavMesh üzerinde deðilsek hareket emri verme
        if (!_agent.isActiveAndEnabled || !_agent.isOnNavMesh)
        {
            return;
        }

        if (mevcutDurum == HayvanDurumu.Oluden) return;

        DurumKontrolu();

        // OTOMATÝK ÜRETÝM KONTROLÜ (Tavuk/Ördek vb. için)
        if (_uretim != null && _uretim.otomatikUretim && _durum.urunVermeyeHazir)
        {
            if (mevcutDurum == HayvanDurumu.Dolasiyor || mevcutDurum == HayvanDurumu.YiyecekAriyor)
            {
                _uretim.UrunuUretOtomomik();
            }
        }

        AnimasyonlariGuncelle();
    }

    void DurumKontrolu()
    {
        if (_durum == null || _oyuncu == null) return;

        bool acMi = _durum.AcMi();
        float oyuncuyaMesafe = Vector3.Distance(transform.position, _oyuncu.position);

        // AÞAMA 1: ÖNCELÝK KONTROLÜ

        // 1. TEHDÝT KONTROLÜ (En Yüksek Öncelik)
        if (oyuncuyaMesafe < gorusMesafesi)
        {
            if (saldirganMi)
                mevcutDurum = HayvanDurumu.Kovaliyor;
            else
                mevcutDurum = HayvanDurumu.Kaciyor;
        }

        // 2. BARINAK ÝHTÝYACI
        else if (!barinaktaMi && acMi && _durum.mevcutAclik > barinagaDonmeAclikSeviyesi)
        {
            if (EnYakinBarinagiBul() != null)
                mevcutDurum = HayvanDurumu.BarinagaGit;
            else
                mevcutDurum = HayvanDurumu.YiyecekAriyor;
        }

        // 3. YÝYECEK ARAMA ÝHTÝYACI
        else if (acMi && mevcutDurum != HayvanDurumu.BarinagaGit)
        {
            mevcutDurum = HayvanDurumu.YiyecekAriyor;
        }

        // 4. NORMALLEÞME KONTROLÜ
        else if (oyuncuyaMesafe >= gorusMesafesi * 1.5f || !acMi)
        {
            if (!barinaktaMi)
            {
                mevcutDurum = HayvanDurumu.Dolasiyor;
            }
        }

        // AÞAMA 2: STATE'E GÖRE ÝÞLEM YAP (Eylem)

        switch (mevcutDurum)
        {
            case HayvanDurumu.Dolasiyor:
                // Sýkýþmayý engellemek için yeni hedefi sadece belirli durumlarda al.
                if (_agent.remainingDistance < 1f && _agent.velocity.magnitude < 0.1f)
                    YeniHedefBelirle();
                break;

            case HayvanDurumu.Kovaliyor:
                _agent.SetDestination(_oyuncu.position);
                break;

            case HayvanDurumu.Kaciyor:
                OyuncudanKac();
                break;

            case HayvanDurumu.YiyecekAriyor:
                YiyecekAraVeTuket();
                break;

            case HayvanDurumu.BarinagaGit:
                BarinagaGit();
                break;

            case HayvanDurumu.Oluden:
                break;
        }
    }

    void YeniHedefBelirle()
    {
        Vector3 rastgeleYon = Random.insideUnitSphere * dolasmaYaricapi;
        rastgeleYon += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(rastgeleYon, out hit, dolasmaYaricapi, 1))
        {
            _agent.SetDestination(hit.position);
        }
    }

    void OyuncudanKac()
    {
        Vector3 kacisYonu = transform.position - _oyuncu.position;
        Vector3 yeniHedef = transform.position + kacisYonu;

        _agent.SetDestination(yeniHedef);
    }

    void AnimasyonlariGuncelle()
    {
        if (_animator != null)
        {
            bool yuruyor = _agent.velocity.magnitude > 0.1f;
            _animator.SetBool("Yuruyor", yuruyor);
        }
    }

    // YÝYECEK MANTIK METOTLARI
    void YiyecekAraVeTuket()
    {
        if (_hedefYiyecek == null)
        {
            // Sadece bekleme süresi dolduysa yeni arama yap
            if (Time.time > _sonAramaZamani + yiyecekAramaBeklemeSuresi)
            {
                _hedefYiyecek = EnYakinYiyecegiBul();
                _sonAramaZamani = Time.time;

                if (_hedefYiyecek != null)
                    _agent.SetDestination(_hedefYiyecek.transform.position);
                else
                {
                    // Yiyecek yoksa, arama beklemesi baþlar ve dolaþmaya döner.
                    mevcutDurum = HayvanDurumu.Dolasiyor;
                    YeniHedefBelirle();
                }
            }
        }
        else
        {
            float yiyecegeMesafe = Vector3.Distance(transform.position, _hedefYiyecek.transform.position);

            if (yiyecegeMesafe < 1.5f) // Tüketim eþiði
            {
                // TÜKETÝM GERÇEKLEÞÝYOR
                float besin = _hedefYiyecek.Tuket();
                _durum.Beslen(besin);

                _hedefYiyecek = null;
                mevcutDurum = HayvanDurumu.Dolasiyor;
                YeniHedefBelirle();
            }
            else // Rota yenileme
            {
                if (!_agent.hasPath || _agent.remainingDistance > yiyecegeMesafe)
                {
                    _agent.SetDestination(_hedefYiyecek.transform.position);
                }
            }
        }
    }

    YiyecekKaynagi EnYakinYiyecegiBul()
    {
        GameObject[] yiyecekler = GameObject.FindGameObjectsWithTag("Yiyecek");
        if (yiyecekler.Length == 0) return null;

        GameObject enYakinObj = yiyecekler
            .OrderBy(y => Vector3.Distance(transform.position, y.transform.position))
            .FirstOrDefault();

        return enYakinObj?.GetComponent<YiyecekKaynagi>();
    }

    void BarinagaGit()
    {
        if (_hedefBarinak == null)
        {
            _hedefBarinak = EnYakinBarinagiBul();
            if (_hedefBarinak != null)
                _agent.SetDestination(_hedefBarinak.transform.position);
            else
                mevcutDurum = HayvanDurumu.YiyecekAriyor;
        }
    }

    Barinak EnYakinBarinagiBul()
    {
        GameObject[] barinaklar = GameObject.FindGameObjectsWithTag("Barinak");
        if (barinaklar.Length == 0) return null;

        // LINQ ile en yakýn objeyi bul
        GameObject enYakinObj = barinaklar
            .OrderBy(b => Vector3.Distance(transform.position, b.transform.position))
            .FirstOrDefault();

        return enYakinObj?.GetComponent<Barinak>();
    }
}