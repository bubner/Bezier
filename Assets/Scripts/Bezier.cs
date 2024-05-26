using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Bezier : MonoBehaviour
{
    public static Bezier instance => FindObjectOfType<Bezier>();
    public float resolution = 0.004f;

    [HideInInspector] public Vector2[] controlPoints;
    private Vector2[] lastPoints;
    private LineRenderer bezierRenderer;

    internal void Awake()
    {
        bezierRenderer = GetComponent<LineRenderer>();
        bezierRenderer.startColor = Color.white;
        bezierRenderer.endColor = Color.white;
        bezierRenderer.startWidth = 0.1f;
        bezierRenderer.endWidth = 0.1f;
        bezierRenderer.useWorldSpace = true;
    }

    internal void Update()
    {
        // Fetch new points, "GameController" tag used for all control points
        controlPoints = GameObject.FindGameObjectsWithTag("GameController")
            .Select(o => new Vector2(o.transform.position.x, o.transform.position.y))
            .ToArray();

        // Performance optimisation: only generate the curve if required
        if (CompareVectorArrays(controlPoints, lastPoints))
            return;

        GenerateBezierCurve();
    }

    /// <summary>
    /// Render the Bézier curve based on the control points.
    /// </summary>
    public void GenerateBezierCurve()
    {
        // Progress from t 0->1 at resolution interval and plot Bézier curve points
        ArrayList newVertices = new();
        for (float t = 0; t < 1; t += resolution)
        {
            newVertices.Add(BezierVec(controlPoints, t));
        }
        // Always include t=1
        newVertices.Add(BezierVec(controlPoints, 1));

        // Update the line renderer with the new vertices
        bezierRenderer.positionCount = newVertices.Count;
        for (int i = 0; i < newVertices.Count; i++)
        {
            bezierRenderer.SetPosition(i, (Vector2)newVertices[i]);
        }

        lastPoints = controlPoints;
    }

    /// <summary>
    /// Generate a 2D Bezier function vector based on x,y coordinates.
    /// </summary>
    /// <param name="vectors">The control points</param>
    /// <param name="t">Interpolation ratio in [0, 1]</param>
    /// <returns>A vector at t along the control points</returns>
    public static Vector2 BezierVec(Vector2[] vectors, double t)
    {
        return new Vector2(
            (float)BezierFunc(vectors.Select(v => v.x).ToArray(), t),
            (float)BezierFunc(vectors.Select(v => v.y).ToArray(), t)
        );
    }

    /// <summary>
    /// Generate a Bezier function based on a dimensionless set of points.
    /// </summary>
    /// <param name="points">The points to evaluate</param>
    /// <param name="t">Interpolation ratio in [0, 1]</param>
    /// <returns></returns>
    private static double BezierFunc(float[] points, double t)
    {
        // Clamp t in [0,1]
        t = t switch
        {
            > 1 => 1,
            < 0 => 0,
            _ => t
        };

        double output = 0;
        // Don't create an extra invisible point at (0,0) by subtracting 1
        int n = points.Length - 1;
        // B(t)=\sum_{i=0}^{n}\binom{n}{i}(1-t)^{n-i}t^iP_i
        for (int i = 0; i <= n; i++)
        {
            // \binom{n}{k}=\frac{n!}{k!(n-k)!}
            double binomialCoefficient = Fac(n) / (Fac(i) * Fac(n - i));
            // Sum interpolation of B(t)
            output += binomialCoefficient * Math.Pow(1 - t, n - i) * Math.Pow(t, i) * points[i];
        }

        return output;
    }

    /// <summary>
    /// Take the factorial (!) of an integer.
    /// </summary>
    /// <param name="x">the number to take the factorial of, for all x >= 0</param>
    /// <returns>The factorial of x</returns>
    private static double Fac(int x)
    {
        double o = 1;
        // {x!}=\prod_{i=1}^{x}i
        for (int i = x; i >= 2; i--)
        {
            o *= i;
        }

        return o;
    }

    /// <summary>
    /// Compare two vector lists for equality.
    /// </summary>
    /// <param name="a">List of first vectors</param>
    /// <param name="b">List of second vectors</param>
    /// <returns>Whether the vectors inside the arrays are equal</returns>
    private static bool CompareVectorArrays(Vector2[] a, Vector2[] b)
    {
        if (a == null || b == null || a.Length != b.Length)
            return false;
        return !a.Where((t, i) => !t.Equals(b[i])).Any();
    }
}