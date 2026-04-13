using UnityEngine;

public class SoilController : MonoBehaviour, IInteractable
{
    [Header("Ekim Ayarlarý")]
    public GameObject cropPrefab; // Hangi bitkiyi ekeceðiz? (Havuç Prefab'ý)
    public Transform spawnPoint;  // Bitki tam nerede çýksýn?

    private bool isPlanted = false; // Þu an dolu mu?

    public void Interact()
    {
        if (!isPlanted)
        {
            PlantSeed();
        }
        else
        {
            Debug.Log("Bu toprak zaten ekili!");
        }
    }

    void PlantSeed()
    {
        if (cropPrefab != null)
        {
            // Bitkiyi oluþtur (Topraðýn pozisyonunda)
            // Quaternion.identity = Dönme açýsý sýfýr (düz) olsun demek.
            GameObject newPlant = Instantiate(cropPrefab, transform.position, Quaternion.identity);

            // Bitkiyi biraz yukarý kaldýralým ki topraðýn içine girmesin (veya spawnPoint kullanýrýz)
            // newPlant.transform.position += new Vector3(0, 0.2f, 0);

            isPlanted = true;
            Debug.Log("Tohum ekildi!");

            // NOT: Hasat edilince topraðýn tekrar 'isPlanted = false' olmasý lazým.
            // Bunu ileride 'Events' sistemiyle veya basit bir kontrolle çözeceðiz.
            // Þimdilik sadece ekim yapalým.
        }
    }

    // Bitki hasat edilince bu fonksiyonu çaðýrýp topraðý boþa çýkaracaðýz (Ýleride)
    public void ClearSoil()
    {
        isPlanted = false;
    }
}