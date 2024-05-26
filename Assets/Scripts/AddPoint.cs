using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AddPoint : MonoBehaviour
{
    // Maximum calculations can be done before we run out of double precision
    [SerializeField] private int maxPoints = 171;
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private Button addButton;
    private Camera mainCamera;
    private Image addButtonImage;

    private EventSystem events;
    private bool addMode;
    private int currentPoints;

    internal void Start()
    {
        mainCamera = Camera.main;
        events = FindObjectOfType<EventSystem>();
        addButtonImage = addButton.GetComponent<Image>();
    }

    public void OnAddButtonClick()
    {
        addMode = currentPoints < maxPoints && !addMode;
        addButtonImage.color = addMode ? Color.green : new Color(127, 127, 127);
    }

    internal void Update()
    {
        currentPoints = GameObject.FindGameObjectsWithTag("GameController").Length;
        if (!addMode || !Input.GetMouseButtonDown(0) || events.IsPointerOverGameObject()) return;
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Instantiate(pointPrefab, mousePos, Quaternion.identity);
        if (currentPoints < maxPoints) return;
        // Call a button press to fully update state
        OnAddButtonClick();
    }
}