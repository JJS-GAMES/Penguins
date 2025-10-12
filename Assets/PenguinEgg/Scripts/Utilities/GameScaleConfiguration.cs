using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameScaleConfiguration : MonoBehaviour
{
    [SerializeField]
    protected List<AnchorSet> _anchorSets = new();

    protected Vector2 _lastResolution = new();
    protected float _currentAspectRatio = 0;

    protected virtual void OnEnable()
    {
        _lastResolution = new Vector2(Screen.width, Screen.height);
        _currentAspectRatio = _lastResolution.x / _lastResolution.y;
        ScaleObjects();
    }

    protected virtual void Update()
    {
        if(_lastResolution.x == Screen.width && _lastResolution.y == Screen.height) 
            return;

        _lastResolution = new Vector2(Screen.width, Screen.height);
        _currentAspectRatio = _lastResolution.x / _lastResolution.y;

        ScaleObjects();
    }

    protected void ScaleObjects() 
    {
        foreach (var anchorSet in _anchorSets)
        {
            ConfigByAnchors closestConfig = GetClosestConfig(anchorSet.anchorConfigs, _currentAspectRatio);
            ApplyAnchorConfig(anchorSet.targetObject, closestConfig);
        }
    }

    protected ConfigByAnchors GetClosestConfig(List<ConfigByAnchors> configs, float currentAspectRatio)
    {
        var ordered = configs
            .OrderBy(cfg => Mathf.Abs((cfg.aspectRatio.x / cfg.aspectRatio.y) - currentAspectRatio));
        var first = ordered;
        return configs
            .OrderBy(cfg => Mathf.Abs((cfg.aspectRatio.x / cfg.aspectRatio.y) - currentAspectRatio))
            .First();
    }

    protected void ApplyAnchorConfig(RectTransform rect, ConfigByAnchors config)
    {
        if (rect == null || config == null) return;

        rect.anchorMin = config._minAnchor;
        rect.anchorMax = config._maxAnchor;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
