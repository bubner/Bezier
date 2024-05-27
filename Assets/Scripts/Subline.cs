using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Render lines between a series of Bezier points.
/// </summary>
public class Subline
{
    private readonly GameObject pointPrefab;
    private readonly LineRenderer[] renderers;
    private readonly List<GameObject>[] renderPoints = {new(), new()};
    private readonly Color color;

    public Subline(GameObject pointPrefab, Color color)
    {
        // Two renderers which will render the control points and the interpolated line
        renderers = new LineRenderer[2];
        renderers[0] = new GameObject("Root").AddComponent<LineRenderer>();
        renderers[1] = new GameObject("Line").AddComponent<LineRenderer>();
        for (int i = 0; i < 2; i++)
        {
            renderers[i].material = new Material(Shader.Find("Sprites/Default"));
            renderers[i].startColor = new Color(0.5f, 0.5f, 0.5f, 0.07f);
            renderers[i].endColor = new Color(0.5f, 0.5f, 0.5f, 0.07f);
            renderers[i].startWidth = 0.07f;
            renderers[i].endWidth = 0.07f;
            renderers[i].useWorldSpace = true;
        }

        this.color = color;
        this.pointPrefab = pointPrefab;
    }

    public void Render(Vector2[] controlPoints, float t)
    {
        // Cannot draw a line
        if (controlPoints.Length < 2)
            return;

        // Clear all existing render points
        for (int i = 0; i < 2; i++)
        {
            renderPoints[i].ForEach(Object.Destroy);
            renderPoints[i].Clear();
        }

        // Draw the root control points
        renderers[0].positionCount = controlPoints.Length;
        renderers[0].SetPositions(controlPoints.Select(v => (Vector3)v).ToArray());
        foreach (Vector2 point in controlPoints)
            renderPoints[0].Add(Object.Instantiate(pointPrefab, point, Quaternion.identity));

        // Set the color of the points at the current iteration depth
        renderPoints[0].ForEach(o => o.GetComponent<SpriteRenderer>().color = color);

        // Should not draw an interpolated line if we have only two points (otherwise we will
        // overlap with the main Bézier draw point that is being rendered externally)
        if (controlPoints.Length == 2)
            return;

        // Draw the interpolated line with the same method but instead linear interpolating between
        // the control points. These control points will increase in depth as the iterations progress
        renderers[1].positionCount = controlPoints.Length - 1;
        for (int i = 0; i < controlPoints.Length - 1; i++)
        {
            renderPoints[1].Add(Object.Instantiate(pointPrefab));
            // (1-t)P0 + tP1
            Vector2 vertex = Vector2.Lerp(controlPoints[i], controlPoints[i + 1], t);
            renderPoints[1][i].transform.position = vertex;
            renderers[1].SetPosition(i, vertex);
        }
        renderPoints[1].ForEach(o => o.GetComponent<SpriteRenderer>().color = color);
    }

    public void Disable()
    {
        for (int i = 0; i < 2; i++)
        {
            renderers[i].enabled = false;
            foreach (GameObject point in renderPoints[i])
                point.SetActive(false);
        }
    }
    
    public void Enable()
    {
        for (int i = 0; i < 2; i++)
        {
            renderers[i].enabled = true;
            foreach (GameObject point in renderPoints[i])
                point.SetActive(true);
        }
    }

    public void Destroy()
    {
        foreach (LineRenderer renderer in renderers)
            Object.Destroy(renderer.gameObject);
        for (int i = 0; i < 2; i++)
        {
            foreach (GameObject point in renderPoints[i])
                Object.Destroy(point);
        }
    }

    public Vector2[] GetNewVertices(float t)
    {
        List<Vector2> newVertices = new();
        for (int i = 0; i < renderPoints[1].Count - 1; i++)
        {
            // Interpolate based on the current t value and the new vertices from the last line
            Vector2 vertex = Vector2.Lerp(renderPoints[1][i].transform.position, renderPoints[1][i + 1].transform.position, t);
            newVertices.Add(vertex);
        }

        return newVertices.ToArray();
    }
}