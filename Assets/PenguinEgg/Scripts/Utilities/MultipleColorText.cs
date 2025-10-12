using TMPro;
using UnityEngine;

public class MultipleColorText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;

    [SerializeField]
    private Color _firstColor;

    [SerializeField]
    private Color _secondColor;

    [SerializeField]
    private int _nLettersToChangeFromTheEnd;

    private void Start()
    {
        SetTextWithMultipleColors();    
    }

    public void SetTextWithMultipleColors()
    {
        string firstPart = _text.text.Substring(0, _text.text.Length - _nLettersToChangeFromTheEnd);
        string secondPart = _text.text.Substring(_text.text.Length - _nLettersToChangeFromTheEnd, _nLettersToChangeFromTheEnd);

        string normalHex = ColorUtility.ToHtmlStringRGBA(_firstColor);
        string lastHex = ColorUtility.ToHtmlStringRGBA(_secondColor);

        _text.text = $"<color=#{normalHex}>{firstPart}</color><color=#{lastHex}>{secondPart}</color>";
    }
}
