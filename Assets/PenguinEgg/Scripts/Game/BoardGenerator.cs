using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    #region Editor Variables

    [SerializeField]
    private GameObject _boardPool;

    [SerializeField]
    private GameObject _tileRow;

    [SerializeField]
    private Tile _tilePrefab;

    #endregion

    private List<TileType> _tileTypes = Enum.GetValues(typeof(TileType))
        .Cast<TileType>()
        .Where(t => t != TileType.None)
        .ToList();

    public List<Tile> GenerateBoard()
    {
        TileSpriteContainer tileSpriteContainer = TileSpriteContainer.Instance;
        List<Tile> tiles = new List<Tile>();
        Queue<TileType> shuffledTileTypes1 = FisherYatesShuffle();
        Queue<TileType> shuffledTileTypes2 = new Queue<TileType>(shuffledTileTypes1.OrderBy(_ => UnityEngine.Random.value));

        GameObject row = Instantiate(_tileRow, _boardPool.transform);
        int generatedTiles = 0;

        for (int i = 0; i < BoardManager.BOARD_SIZE / 2; i++)
        {
            // Tile 1
            TileType tile1Type = shuffledTileTypes1.Dequeue();
            Tile tile = Instantiate(_tilePrefab, row.transform);
            tile.Initialize(tile1Type, tileSpriteContainer.GetTileSpriteByType(tile1Type));
            generatedTiles++;
            if (generatedTiles % 5 == 0)
                row = Instantiate(_tileRow, _boardPool.transform);
            tiles.Add(tile);

            // Tile 2
            TileType tile2Type = shuffledTileTypes2.Dequeue();
            tile = Instantiate(_tilePrefab, row.transform);
            tile.Initialize(tile2Type, tileSpriteContainer.GetTileSpriteByType(tile2Type));
            generatedTiles++;
            if (generatedTiles < BoardManager.BOARD_SIZE && generatedTiles % 5 == 0)
                row = Instantiate(_tileRow, _boardPool.transform);
            tiles.Add(tile);
        }

        return tiles;
    }

    public Queue<TileType> FisherYatesShuffle()
    {
        List<TileType> shuffledList = new List<TileType>(_tileTypes);
        int n = shuffledList.Count;
        int count = BoardManager.BOARD_SIZE / 2;

        for (int i = 0; i < count; i++)
        {
            int j = UnityEngine.Random.Range(i, n);
            (shuffledList[i], shuffledList[j]) = (shuffledList[j], shuffledList[i]);
        }

        Queue<TileType> tileQueue = new Queue<TileType>(shuffledList.GetRange(0, count));
        return tileQueue;
    }
}
