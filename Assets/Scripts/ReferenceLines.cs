using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReferenceLines : MonoBehaviour
{
    [SerializeField] private Slider tSlider;
    [SerializeField] private GameObject pointPrefab;

    private int lastLength;
    private readonly List<LineRenderer> lineRenderers = new();
    private readonly List<GameObject> points = new();

    internal void Start()
    {
        lineRenderers.Add(new GameObject("Line").AddComponent<LineRenderer>());
        lineRenderers.Add(new GameObject("SubLine").AddComponent<LineRenderer>());
        for (int i = 0; i < 2; i++)
        {
            lineRenderers[i].material = new Material(Shader.Find("Sprites/Default"));
            lineRenderers[i].startColor = new Color(0.5f, 0.5f, 0.5f, 0.15f);
            lineRenderers[i].endColor = new Color(0.5f, 0.5f, 0.5f, 0.15f);
            lineRenderers[i].startWidth = 0.05f;
            lineRenderers[i].endWidth = 0.05f;
            lineRenderers[i].useWorldSpace = true;
        }
    }

    internal void Update()
    {
        Vector2[] controlPoints = Bezier.instance.controlPoints;
        if (controlPoints.Length < 2 || !Mathf.Approximately(Bezier.instance.resolution, 0.002f))
        {
            for (int i = 0; i < 2; i++)
            {
                lineRenderers[i].enabled = false;
                foreach (GameObject point in points)
                {
                    point.SetActive(false);
                }
            }
            return;
        }

        if (lastLength != controlPoints.Length)
        {
            foreach (GameObject point in points)
                Destroy(point);

            points.Clear();
            for (int i = 0; i < controlPoints.Length - 1; i++)
                points.Add(Instantiate(pointPrefab));

            lineRenderers[1].positionCount = controlPoints.Length - 1;
        }

        for (int i = 0; i < controlPoints.Length - 1; i++)
        {
            Vector2 vertex = Vector2.Lerp(controlPoints[i], controlPoints[i + 1], tSlider.value);
            points[i].transform.position = vertex;
            lineRenderers[1].SetPosition(i, vertex);
        }

        for (int i = 0; i < 2; i++)
            lineRenderers[i].enabled = true;
        foreach (GameObject point in points)
            point.SetActive(true);

        lineRenderers[0].positionCount = controlPoints.Length;
        for (int i = 0; i < controlPoints.Length; i++)
            lineRenderers[0].SetPosition(i, controlPoints[i]);

        lastLength = controlPoints.Length;
    }
}