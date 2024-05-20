using System;
using System.Linq;
using UnityEngine;

public class Bezier : MonoBehaviour
{
    [SerializeField] private GameObject drawpoint;
    public float resolution = 0.25f;
    
    private Vector2[] points;
    private Vector2[] lastPoints;

    internal void Update()
    {
        // "GameController" tag used for all control points
        points = GameObject.FindGameObjectsWithTag("GameController")
            .Select(o => new Vector2(o.transform.position.x, o.transform.position.y))
            .ToArray();

        if (!points.Equals(lastPoints))
            GenerateBezierCurve();

        lastPoints = points;
    }

    private void GenerateBezierCurve()
    {
        // Progress from t 0->1 at resolution interval and plot Bézier curve points
        for (float t = 0; t <= 1; t += resolution)
        {
            GameObject dp = Instantiate(drawpoint);
            dp.transform.position = BezierVec(points, t);
        }
    }

    /// <summary>
    /// Generate a 2D Bezier function vector based on x,y coordinates.
    /// </summary>
    /// <param name="vectors">The control points</param>
    /// <param name="t">Interpolation ratio in [0, 1]</param>
    /// <returns>A vector at t along the control points</returns>
    private static Vector2 BezierVec(Vector2[] vectors, double t)
    {
        return new Vector2(
            BezierFunc(vectors.Select(v => v.x).ToArray(), t),
            BezierFunc(vectors.Select(v => v.y).ToArray(), t)
        );
    }

    /// <summary>
    /// Generate a Bezier function based on a dimensionless set of points.
    /// </summary>
    /// <param name="points">The points to evaluate</param>
    /// <param name="t">Interpolation ratio in [0, 1]</param>
    /// <returns></returns>
    private static float BezierFunc(float[] points, double t)
    {
        // Clamp t in [0,1]
        t = t switch
        {
            > 1 => 1,
            < 0 => 0,
            _ => t
        };

        float output = 0;
        // Don't create an extra invisible point at (0,0) by subtracting 1
        int n = points.Length - 1;
        // B(t)=\sum_{i=0}^{n}\binom{n}{i}(1-t)^{n-i}t^iP_i
        for (int i = 0; i <= n; i++)
        {
            // \binom{n}{k}=\frac{n!}{k!(n-k)!}
            float binomialCoefficient = Fac(n) / (Fac(i) * Fac(n - i));
            // Sum interpolation of B(t)
            output += binomialCoefficient * (float) Math.Pow(1 - t, n - i) * (float) Math.Pow(t, i) * points[i];
        }
        return output;
    }

    /// <summary>
    /// Take the factorial of a number
    /// </summary>
    /// <param name="x">the number to take the factorial of</param>
    /// <returns>The factorial of x</returns>
    private static float Fac(int x)
    {
        if (x == 0)
            return 1;
        // x! = x(x-1)!
        return x * Fac(x - 1);
    }
}
