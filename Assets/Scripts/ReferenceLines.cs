using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ReferenceLines : MonoBehaviour
{
    [SerializeField] private Slider tSlider;
    [SerializeField] private GameObject pointPrefab;

    private readonly List<Subline> renderers = new();
    private Vector2[] lastControlPoints = Array.Empty<Vector2>();
    private float lastT;

    internal void Start()
    {
        GenerateRefs();
    }

    private static Color GetRandomVariantOfGray()
    {
        float random = Random.Range(0.25f, 0.8f);
        return new Color(random, random, random, 1f);
    }

    private void GenerateRefs()
    {
        // Remove all existing renderers
        foreach (Subline r in renderers)
            r.Destroy();
        renderers.Clear();

        // For any Bezier, there will be n-1 lines
        for (float i = 0; i < Bezier.instance.controlPoints.Length - 1; i++)
            renderers.Add(new Subline(pointPrefab, GetRandomVariantOfGray()));
    }

    internal void Update()
    {
        Vector2[] controlPoints = Bezier.instance.controlPoints;
        // Cannot render
        if (controlPoints.Length < 2 || !Bezier.isAtMaxResolution)
        {
            foreach (Subline r in renderers)
                r.Disable();
            return;
        }

        foreach (Subline r in renderers)
            r.Enable();

        // Performance optimisation: only generate the curve if required
        if (Bezier.CompareVectorArrays(lastControlPoints, controlPoints) && Mathf.Approximately(tSlider.value, lastT))
            return;

        if (Bezier.instance.controlPoints.Length != lastControlPoints.Length)
        {
            // Control points have been physically modified, we need to scrap everything
            GenerateRefs();
        }

        // Rerender all lines
        renderers[0].Render(controlPoints, tSlider.value);
        for (int i = 1; i < renderers.Count; i++)
        {
            // Use the render points of the last line as the control points for the next
            renderers[i].Render(renderers[i - 1].GetNewVertices(tSlider.value), tSlider.value);
        }

        lastT = tSlider.value;
        lastControlPoints = controlPoints;
    }
}