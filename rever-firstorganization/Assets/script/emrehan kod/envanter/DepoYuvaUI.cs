using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // Týklamayý algýlamak için GEREKLÝ!

public class DepoYuvaUI : MonoBehaviour, IPointerClickHandler
{
    // Hangi envantere ait olduðunu ve kaçýncý slot olduðunu tutar
    public enum EnvanterTipi { Oyuncu, Zula }
    private EnvanterTipi _tip;
    private int _index;

    [Header("Bilesenler")]
    public Image ikonResmi;
    public TextMeshProUGUI miktarMetni;

    // ZulaUIManager tarafýndan çaðrýlýr
    public void Initialize(EnvanterTipi tip, int index)
    {
        _tip = tip;
        _index = index;
    }

    // Týklandýðýnda ZulaUIManager'a haber verir
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ZulaUIManager.Instance.YuvaTiklandi(_tip, _index);
        }
    }

    // YuvayiGuncelle ve Temizle (EnvanterYuvaUI'dan kopyalandý)
    public void YuvayiGuncelle(EsyaVeriSO esyaVerisi, int miktar)
    {
        if (esyaVerisi != null)
        {
            if (ikonResmi != null && esyaVerisi.ikon != null)
            {
                ikonResmi.sprite = esyaVerisi.ikon;
                ikonResmi.color = Color.white;
            }
            if (miktarMetni != null)
            {
                miktarMetni.text = miktar > 1 ? miktar.ToString() : "";
            }
        }
    }

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
    }
}