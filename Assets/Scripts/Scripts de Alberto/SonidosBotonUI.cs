using UnityEngine;
using UnityEngine.EventSystems;

public class SonidosBotonUI : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioSource fuenteAudioUI;
    public AudioClip sonidoHover;
    public AudioClip sonidoClic;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (fuenteAudioUI != null && sonidoHover != null)
            fuenteAudioUI.PlayOneShot(sonidoHover);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (fuenteAudioUI != null && sonidoClic != null)
            fuenteAudioUI.PlayOneShot(sonidoClic);
    }
}
