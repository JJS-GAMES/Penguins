using System;
using UnityEngine;
using UnityEngine.UI;

public class ParametersManager : Manager
{
    #region Editor Variables

    [SerializeField]
    private GameObject _parametersCanvas;

    [SerializeField]
    private Button _homeButton;

    #endregion

    #region Public Variables

    public static event Action OnParametersCompleted;

    #endregion

    private bool _isParemetersOpen = false;

    #region Manager Implementation

    public override void Initialize()
    {
        if (_isInitialized)
            return;

        if (!_parametersCanvas)
            Debug.LogError("Parameters Canvas is not assigned in the inspector");
        else
            _parametersCanvas.SetActive(false);

        if (!_homeButton)
            Debug.LogError("Home Button is not assigned in the inspector");
        else
            _homeButton.onClick.AddListener(() => ToggleParameters());

        _isInitialized = true;
    }

    #endregion

    public void ToggleParameters()
    {
        _isParemetersOpen = !_isParemetersOpen;
        _parametersCanvas.SetActive(_isParemetersOpen);
        OnParametersCompleted?.Invoke();
    }
}
