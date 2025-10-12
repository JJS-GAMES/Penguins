using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class AnchorSet
{
    public RectTransform targetObject;
    public List<ConfigByAnchors> anchorConfigs;
}

[CreateAssetMenu(fileName = "AspectRatioConfig", menuName = "Scale Config/Config By Anchors", order = 1)]
public class ConfigByAnchors : ScriptableObject
{
    public string configName;
    public Vector2 aspectRatio;

    [Header("Anchors")]
    public Vector2 _minAnchor;
    public Vector2 _maxAnchor;
}
