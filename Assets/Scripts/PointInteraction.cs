using UnityEngine;

public class PointInteraction : MonoBehaviour
{
    private Camera mainCamera;
    private SpriteRenderer pointRenderer;
    private bool clicking;
    private bool mouseOver;

    internal void Start()
    {
        mainCamera = Camera.main;
        pointRenderer = GetComponent<SpriteRenderer>();
    }

    public bool IsInteracting()
    {
        return mouseOver || clicking;
    }

    internal void Update()
    {
        if (clicking)
        {
            transform.position = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            // Lock Z
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            // Clamp to screen using the camera dimensions
            Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);
            screenPos.x = Mathf.Clamp(screenPos.x, 0, Screen.width);
            screenPos.y = Mathf.Clamp(screenPos.y, 0, Screen.height);
            transform.position = mainCamera.ScreenToWorldPoint(screenPos);
        }

        pointRenderer.color = IsInteracting() ? Color.green : Color.red;
    }

    internal void OnMouseDown()
    {
        clicking = true;
    }

    internal void OnMouseUp()
    {
        clicking = false;
    }

    internal void OnMouseOver()
    {
        mouseOver = true;
    }

    internal void OnMouseExit()
    {
        mouseOver = false;
    }
}