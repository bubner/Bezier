using UnityEngine;

public class PointInteraction : MonoBehaviour
{
    private bool clicking;
    private bool mouseOver;

    public bool IsInteracting()
    {
        return mouseOver || clicking;
    }

    internal void Update()
    {
        if (clicking)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Lock Z
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            // Clamp to screen
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -8.5f, 8.5f),
                Mathf.Clamp(transform.position.y, -5f, 5f), 0);
        }

        GetComponent<SpriteRenderer>().color = IsInteracting() ? Color.green : Color.red;
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