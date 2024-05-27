using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TRunner : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject tChanger;
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private Image animateButtonImage;

    private GameObject point;
    private Slider slider;
    private bool animating;

    internal void Start()
    {
        point = Instantiate(pointPrefab);
        point.GetComponent<SpriteRenderer>().color = Color.magenta;
        slider = tChanger.GetComponent<Slider>();
        slider.value = 1;
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
        bool shouldRun = Mathf.Approximately(Bezier.instance.resolution, 0.002f)
                         && Bezier.instance.controlPoints.Length > 1;
        tChanger.SetActive(shouldRun);
        // Additionally only show the point in the range 0 < t < 1 to prevent overlap
        point.SetActive(shouldRun && slider.value != 0 && !Mathf.Approximately(slider.value, 1f));
        animating = animating && shouldRun;
        animateButtonImage.color = animating ? Color.green : new Color(127, 127, 127);

        // Animate t progression if required
        slider.value += animating ? Time.deltaTime / 3f : 0;
        if (animating && slider.value >= 1)
            slider.value = 0;

        point.transform.position = Bezier.BezierVec(Bezier.instance.controlPoints, slider.value);
    }
}