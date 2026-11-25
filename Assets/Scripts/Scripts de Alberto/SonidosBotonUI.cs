using UnityEngine;
using UnityEngine.EventSystems;

public class SonidosBotonUI : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("Configuración de Audio")]
    public AudioSource fuenteAudioUI;
    public AudioClip sonidoHover; // Al pasar el mouse por encima
    public AudioClip sonidoClic;  // Al presionar el botón

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (fuenteAudioUI != null && sonidoHover != null)
        {
            fuenteAudioUI.PlayOneShot(sonidoHover);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (fuenteAudioUI != null && sonidoClic != null)
        {
            fuenteAudioUI.PlayOneShot(sonidoClic);
        }
    }
}
