using UnityEngine;
using UnityEngine.EventSystems;

public class ClickUIToOpen : MonoBehaviour, IPointerClickHandler
{
    public GameObject uiElementToOpen;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (uiElementToOpen != null)
        {
            uiElementToOpen.SetActive(true);
        }
    }
}