using UnityEngine;
using TMPro;

/// <summary>
/// Texto en pantalla tipo "Presiona E para recoger".
/// Ponlo en un TextMeshPro - Text (UI) dentro del Canvas.
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class InteractionPrompt : MonoBehaviour
{
    private static InteractionPrompt instance;
    private TextMeshProUGUI label;

    private void Awake()
    {
        instance = this;
        label = GetComponent<TextMeshProUGUI>();
        label.text = "";
    }

    public static void Show(string message)
    {
        if (instance) instance.label.text = message;
    }

    public static void Hide()
    {
        if (instance) instance.label.text = "";
    }
}
