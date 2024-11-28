using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UISound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] AudioClip onPointerClick;
    [SerializeField] AudioClip onPointerEnter;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onPointerClick != null)
            GameManager.Sound.PlaySFX(onPointerClick);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (onPointerEnter != null)
            GameManager.Sound.PlaySFX(onPointerEnter);
    }

}
