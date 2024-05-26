using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionSlider : MonoBehaviour
{
    private Slider slider;
    [SerializeField] private TextMeshProUGUI text;

    internal void Start()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(v =>
        {
            text.text = "Δt=" + v;
            Bezier.instance.resolution = v;
            Bezier.instance.GenerateBezierCurve();
        });
    }
}