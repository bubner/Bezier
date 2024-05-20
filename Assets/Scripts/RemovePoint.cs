using UnityEngine;
using UnityEngine.EventSystems;

public class RemovePoint : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        foreach (GameObject point in GameObject.FindGameObjectsWithTag("GameController"))
        {
            if (point.GetComponent<PointInteraction>().IsInteracting())
                Destroy(point);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        foreach (GameObject point in GameObject.FindGameObjectsWithTag("GameController"))
            Destroy(point);
    }
}
