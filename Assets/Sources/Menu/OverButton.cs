using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public GameObject o;

    public void OnPointerEnter(PointerEventData eventData)
    {
        o.SetActive(true);
        Debug.Log("Mouse enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        o.SetActive(false);
        Debug.Log("Mouse exit");
    }
}
