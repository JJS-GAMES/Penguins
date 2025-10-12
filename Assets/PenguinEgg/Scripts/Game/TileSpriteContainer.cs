using System.Collections.Generic;
using UnityEngine;

public class TileSpriteContainer : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> _tileSprites;

    private static TileSpriteContainer _instance;
    public static TileSpriteContainer Instance { get => _instance; }

    private void Awake()
    {
        _instance = this;
    }

    public Sprite GetTileSpriteByType(TileType tileType) 
    {
        int index = (int)tileType;
        if (index < 0 || index >= _tileSprites.Count)
            return null;

        return _tileSprites[index];
    }
}
