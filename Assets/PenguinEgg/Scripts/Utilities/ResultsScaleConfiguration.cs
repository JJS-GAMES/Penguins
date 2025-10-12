using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ResultsScaleConfiguration : GameScaleConfiguration
{
    [SerializeField]
    private AnchorSet _scoreForSinglePlayer;

    [SerializeField]
    private AnchorSet _scoreForMultiPlayer;

    [SerializeField]
    protected List<TextSet> _textSetsForSinglePlayer = new();
    
    [SerializeField]
    protected List<TextSet> _textSetsForMultiPlayer = new();

    private GameMode _gameMode;

    protected override void OnEnable()
    {
        _gameMode = GameManager.GameMode;
        base.OnEnable();
        ScaleScoreAnchors();
        ScaleScoreTexts();
    }

    protected override void Update()
    {
        if (_lastResolution.x == Screen.width && _lastResolution.y == Screen.height)
            return;

        base.Update();

        ScaleScoreAnchors();
        ScaleScoreTexts();
    }

    private void ScaleScoreAnchors() 
    {
        AnchorSet _scoreSet = _gameMode == GameMode.SinglePlayer ? _scoreForSinglePlayer : _scoreForMultiPlayer;
        ConfigByAnchors closestConfig = GetClosestConfig(_scoreSet.anchorConfigs, _currentAspectRatio);
        ApplyAnchorConfig(_scoreSet.targetObject, closestConfig);
    }

    private void ScaleScoreTexts()
    {
        if (_gameMode == GameMode.SinglePlayer)
        {
            foreach (var textSet in _textSetsForSinglePlayer)
            {
                ConfigByFontSize closestConfig = GetClosestConfig(textSet.FontConfigs, _currentAspectRatio);
                ApplyFontConfig(textSet.TargetObject, closestConfig);
            }

            return;
        }

        foreach (var textSet in _textSetsForMultiPlayer)
        {
            ConfigByFontSize closestConfig = GetClosestConfig(textSet.FontConfigs, _currentAspectRatio);
            ApplyFontConfig(textSet.TargetObject, closestConfig);
        }
    }

    private ConfigByFontSize GetClosestConfig(List<ConfigByFontSize> configs, float currentAspectRatio)
    {
        var ordered = configs
            .OrderBy(cfg => Mathf.Abs((cfg.AspectRatio.x / cfg.AspectRatio.y) - currentAspectRatio));
        var first = ordered;
        return configs
            .OrderBy(cfg => Mathf.Abs((cfg.AspectRatio.x / cfg.AspectRatio.y) - currentAspectRatio))
            .First();
    }

    private void ApplyFontConfig(TextMeshProUGUI text, ConfigByFontSize config)
    {
        if (text == null || config == null) 
            return;

        text.fontSize = config.FontSize;
    }
}
