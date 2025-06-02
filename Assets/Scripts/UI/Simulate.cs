using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Simulate : MonoBehaviour
{

    public Button simulateButton;
    public TextMeshProUGUI text;
    private bool isSimulating = true;

    void Awake()
    {
        Time.timeScale = 1f;
    }

    void Start()
    {
        simulateButton.onClick.AddListener(SimulateTest);
    }

    private void SimulateTest()
    {
        if (!isSimulating)
        {
            Time.timeScale = 1f;
            text.text = "Pause";
            isSimulating = true;
        }
        else
        {
            Time.timeScale = 0f;
            text.text = "Simulate";
            isSimulating = false;
        }
    }



}
