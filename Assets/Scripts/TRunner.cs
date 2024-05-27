using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TRunner : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject tChanger;
    [SerializeField] private GameObject rtToggler;
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private Image animateButtonImage;

    private GameObject point;
    private Slider slider;
    private bool animating;

    internal void Start()
    {
        point = Instantiate(pointPrefab);
        point.GetComponent<SpriteRenderer>().color = Color.cyan;
        // Always on top
        point.layer = 6;
        slider = tChanger.GetComponent<Slider>();
        // Default to t=0.5
        slider.value = 0.5f;
        slider.onValueChanged.AddListener(v =>
        {
            text.text = "t=" + v.ToString(CultureInfo.InvariantCulture);
        });
    }

    public void OnAnimateButtonClicked()
    {
        animating = !animating;
    }

    internal void Update()
    {
        // Show only at max resolution and when we have enough points
        bool shouldRun = Bezier.isAtMaxResolution && Bezier.instance.controlPoints.Length > 1;
        tChanger.SetActive(shouldRun);
        rtToggler.SetActive(shouldRun);
        // Additionally only show the point in the range 0 < t < 1 to prevent overlap
        point.SetActive(shouldRun && slider.value != 0 && !Mathf.Approximately(slider.value, 1f));
        animating = animating && shouldRun;
        animateButtonImage.color = animating ? Color.green : new Color(127, 127, 127);

        // Animate t progression if required
        slider.value += animating ? Time.deltaTime / Bezier.instance.controlPoints.Length : 0;
        if (animating && slider.value >= 1)
            slider.value = 0;

        point.transform.position = Bezier.BezierVec(Bezier.instance.controlPoints, slider.value);
    }
}