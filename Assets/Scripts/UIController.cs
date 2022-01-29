using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Button buildButton;
    [SerializeField] private TextMeshProUGUI totalPowerText;

    [SerializeField] private City city;

    private void Start()
    {
        buildButton.onClick.AddListener(() => city.EnterBuildMode());
        city.totalPowerChanged.AddListener((totalPower) => totalPowerText.text = $"Total Power: {totalPower}");
    }
}
