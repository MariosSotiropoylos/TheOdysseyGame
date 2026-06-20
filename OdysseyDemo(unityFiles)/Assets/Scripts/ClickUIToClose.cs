using UnityEngine;
using UnityEngine.EventSystems;

public class ClickUIToClose : MonoBehaviour, IPointerClickHandler
{
    public GameObject uiElementToClose;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (uiElementToClose != null)
        {
            uiElementToClose.SetActive(false);
        }
    }
}