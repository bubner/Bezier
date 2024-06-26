using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Bezier : MonoBehaviour
{
    private static Bezier _instance;

    public static Bezier instance
    {
        get
        {
            if (_instance == null)
            {
                // Cache the instance for future use
                _instance = FindObjectOfType<Bezier>();
            }
            return _instance;
        }
    }

    public static bool isAtMaxResolution => Mathf.Approximately(instance.resolution, MAX_RESOLUTION);
    private const float MAX_RESOLUTION = 0.002f;
    public float resolution = 0.002f;

    [SerializeField] private Slider tSlider;
    [SerializeField] private Toggle rtToggle;
    [HideInInspector] public Vector2[] controlPoints;
    private Vector2[] lastPoints;
    private LineRenderer bezierRenderer;
    private bool isRealtimeRender;

    public void OnRealtimeRenderPress(bool newState)
    {
        isRealtimeRender = newState;
        // Force a refresh
        GenerateBezierCurve();
    }

    internal void Awake()
    {
        bezierRenderer = GetComponent<LineRenderer>();
        gameObject.layer = 7;
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
            .Select(o => (Vector2)o.transform.position)
            .ToArray();

        // Performance optimisation: only generate the curve if required
        if (CompareVectorArrays(controlPoints, lastPoints) && !isRealtimeRender)
            return;

        if (isRealtimeRender && !Mathf.Approximately(resolution, MAX_RESOLUTION))
        {
            // Disable realtime rendering if we are not at max resolution as it won't mean anything
            rtToggle.isOn = false;
        }

        GenerateBezierCurve();
    }

    /// <summary>
    /// Render the Bézier curve based on the control points.
    /// </summary>
    public void GenerateBezierCurve()
    {
        // Progress from t 0->1 at resolution interval and plot Bézier curve points
        ArrayList newVertices = new();
        float maxRes = isRealtimeRender ? tSlider.value : 1f;
        for (float t = 0; t < maxRes; t += resolution)
        {
            newVertices.Add(BezierVec(controlPoints, t));
        }
        // Ensure the last point is always the last control point
        if (Mathf.Approximately(maxRes, 1f))
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
    public static bool CompareVectorArrays(Vector2[] a, Vector2[] b)
    {
        if (a == null || b == null || a.Length != b.Length)
            return false;
        return !a.Where((t, i) => !t.Equals(b[i])).Any();
    }
}