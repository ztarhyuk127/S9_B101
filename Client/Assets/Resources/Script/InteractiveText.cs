using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
public class InteractiveText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Color color;
    public FontStyles fontStyle;

    private TextMeshProUGUI text;
    private Color originalColor;
    private FontStyles originalFontStyle;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        originalColor = text.color;
        originalFontStyle = text.fontStyle;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = color;
        text.fontStyle = fontStyle;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = originalColor;
        text.fontStyle = originalFontStyle;
    }



}
