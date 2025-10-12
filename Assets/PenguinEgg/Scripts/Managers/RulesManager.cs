using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RulesManager : Manager
{
    [Serializable]
    public class RulePage
    {
        public Image Marker;
        public GameObject Page;
    }

    #region Editor Variables

    [Header("General")]
    [SerializeField]
    private GameObject _rulesCanvas;

    [SerializeField]
    private Button _homeButton;

    [Header("Page Itmes")]
    [SerializeField]
    private List<RulePage> _pages;

    [SerializeField]
    private Sprite _markerOn;

    [SerializeField]
    private Sprite _markerOff;

    [Header("Buttons")]
    [SerializeField]
    private Button _leftArrow;

    [SerializeField]
    private Button _rightArrow;

    #endregion

    public static event Action OnBackButton;
    private int _currentIndex = 0;

    #region Manager Implementation

    public override void Initialize()
    {
        if (_isInitialized)
            return;

        if (!_rulesCanvas)
            Debug.LogError("Rules Canvas is not assigned in the inspector");
        else
            _rulesCanvas.SetActive(false);

        if (!_homeButton)
            Debug.LogError("Home Button is not assigned in the inspector");
        else
            _homeButton.onClick.AddListener(() => ToggleRules(false));

        InitializePages();
        _isInitialized = true;
    }

    #endregion

    public void ToggleRules(bool isRulesOpen)
    {
        _rulesCanvas.SetActive(isRulesOpen);
        OnBackButton?.Invoke();
    }

    private void InitializePages()
    {
        for (int i = 0; i < _pages.Count; i++)
        {
            _pages[i].Page.SetActive(i == _currentIndex);
            _pages[i].Marker.sprite = i == _currentIndex ? _markerOn : _markerOff;
        }

        _leftArrow.onClick.AddListener(() => ChangePage(true));
        _rightArrow.onClick.AddListener(() => ChangePage(false));

        _leftArrow.interactable = _currentIndex != 0;
        _rightArrow.interactable = _currentIndex != _pages.Count - 1;
    }

    #region Rule Screens Logic

    private void ChangePage(bool isLeftArrow)
    {
        _pages[_currentIndex].Page.SetActive(false);
        _pages[_currentIndex].Marker.sprite = _markerOff;

        if (isLeftArrow)
            _currentIndex--;
        else if (!isLeftArrow)
            _currentIndex++;

        _leftArrow.interactable = _currentIndex != 0;
        _rightArrow.interactable = _currentIndex != _pages.Count - 1;

        _pages[_currentIndex].Page.SetActive(true);
        _pages[_currentIndex].Marker.sprite = _markerOn;
    }

    #endregion
}
