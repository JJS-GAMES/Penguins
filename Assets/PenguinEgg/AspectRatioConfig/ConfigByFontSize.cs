using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

[Serializable]
public class TextSet
{
    public TextMeshProUGUI TargetObject;
    public List<ConfigByFontSize> FontConfigs;
}

[CreateAssetMenu(fileName = "AspectRatioConfig", menuName = "Scale Config/Config By Font Size", order = 1)]
public class ConfigByFontSize : ScriptableObject
{
    public string ConfigName;
    public Vector2 AspectRatio;

    [Header("Anchors")]
    public float FontSize;
}
