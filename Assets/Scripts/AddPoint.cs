using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AddPoint : MonoBehaviour
{
    // Maximum calculations can be done before we run out of double precision
    [SerializeField] private int maxPoints = 171;
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private Button addButton;

    private EventSystem events;
    private bool addMode;

    internal void Start()
    {
        events = FindObjectOfType<EventSystem>();
    }

    public void OnAddButtonClick()
    {
        addMode = GameObject.FindGameObjectsWithTag("GameController").Length < maxPoints && !addMode;
        addButton.GetComponent<Image>().color = addMode ? Color.green : new Color(127, 127, 127);
    }

    internal void Update()
    {
        if (!addMode || !Input.GetMouseButtonDown(0) || events.IsPointerOverGameObject()) return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Instantiate(pointPrefab, mousePos, Quaternion.identity);
        if (GameObject.FindGameObjectsWithTag("GameController").Length < maxPoints) return;
        // Call a button press to fully update state
        OnAddButtonClick();
    }
}