using UnityEngine;
using UnityEngine.UI;

public class ResolutionSlider : MonoBehaviour
{
    private Slider slider;

    internal void Start()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(v =>
        {
            Bezier.instance.resolution = v;
            Bezier.instance.GenerateBezierCurve();
        });
    }
}