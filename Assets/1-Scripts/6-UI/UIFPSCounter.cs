using TMPro;
using UnityEngine;

public class UIFPSCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI counter;

    float avgFrameRate;

    void Update()
    {
        FPSUpdate();
    }

    void FPSUpdate()
    {
        float current = Time.frameCount / Time.time;
        avgFrameRate = (int)current;

        counter.text = avgFrameRate.ToString();
    }
}
