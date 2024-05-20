using UnityEngine;

public class ReferenceLines : MonoBehaviour
{
    private LineRenderer lineRenderer;

    internal void Start()
    {
        lineRenderer = new GameObject("Line").AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = new Color(0.5f, 0.5f, 0.5f, 0.15f);;
        lineRenderer.endColor = new Color(0.5f, 0.5f, 0.5f, 0.15f);;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.useWorldSpace = true;
    }

    internal void Update()
    {
        Vector2[] controlPoints = Bezier.instance.controlPoints;
        if (controlPoints.Length < 2)
        {
            lineRenderer.enabled = false;
            return;
        }

        lineRenderer.enabled = true;
        lineRenderer.positionCount = controlPoints.Length;
        for (int i = 0; i < controlPoints.Length; i++)
            lineRenderer.SetPosition(i, controlPoints[i]);
    }
}
