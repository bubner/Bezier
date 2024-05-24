using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TRunner : MonoBehaviour
{
    private Slider slider;
    [SerializeField] private TextMeshProUGUI text;

    internal void Start()
    {
        slider = GetComponent<Slider>();
        slider.value = 1;
        slider.onValueChanged.AddListener(v =>
        {
            text.text = "t=" + v.ToString(CultureInfo.InvariantCulture);
        });
    }
}