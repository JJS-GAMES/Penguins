using TMPro;
using UnityEngine;

namespace YG.Example
{
    public class LanguageExample : MonoBehaviour
    {
        [SerializeField, TextArea(2, 6)] 
        private string _ru;
        [SerializeField, TextArea(2, 6)] 
        private string _en;

        private TextMeshProUGUI _textComponent;

        private void Awake()
        {
            _textComponent = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            YG2.onSwitchLang += SwitchLanguage;
            SwitchLanguage(YG2.lang);
        }
        private void OnDisable()
        {
            YG2.onSwitchLang -= SwitchLanguage;
        }
        private void SwitchLanguage(string lang)
        {
            switch (lang)
            {
                case "ru":
                    _textComponent.text = _ru;
                    break;
                default:
                    _textComponent.text = _en;
                    break;
            }
        }
    }
}