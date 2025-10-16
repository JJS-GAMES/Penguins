using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BoardManager : Manager
{
    public const int BOARD_SIZE = 30;

    public static event Action<bool> OnFirstTilePressed;
    public static event Action OnSecondTilePressed;
    public static event Action<Tile> OnDisableTileExcepThis;
    public static event Action OnDisableWhenEggOrPlayer;
    public static event Action OnEnableTileIfPossible;

    #region Editor Variables

    [SerializeField]
    private BoardGenerator _boardGenerator;

    #endregion

    private Tile _firstPressedTile = null;
    private GameSceneManager _gameSceneManager;
    private List<Tile> _tiles = new();
    private Tile _tileEgg;

    private Queue<Tile> _tilesToReset = new();
    private Queue<TileType> _tileTypesToReset = new();
    private bool _isSwapCardEnabled = false;

    #region Manager Implementation

    public override void Initialize()
    {
        _tiles = _boardGenerator.GenerateBoard();
    }

    #endregion

    #region Unity Callbacks

    private void OnEnable()
    {
        Tile.OnTilePressed += HandlePressedTile;
        PowerUpManager.OnViewCardEnabled += OnViewCardEnabled;
        PowerUpManager.OnSwapCardEnabled += OnSwapCardEnabled;
    }

    private void OnDisable()
    {
        Tile.OnTilePressed -= HandlePressedTile;
        PowerUpManager.OnViewCardEnabled -= OnViewCardEnabled;
        PowerUpManager.OnSwapCardEnabled -= OnSwapCardEnabled;
    }

    #endregion

    #region Event Listeners

    private void HandlePressedTile(Tile pressedTile)
    {
        if (_isSwapCardEnabled)
        {
            HandleSwapCard(pressedTile);
            return;
        }

        if (!_firstPressedTile)
        {
            _firstPressedTile = pressedTile;
            pressedTile.RevealTile();
            OnFirstTilePressed?.Invoke(pressedTile.HasEgg);
            return;
        }

        bool isCombination = _firstPressedTile.TileType == pressedTile.TileType;

        pressedTile.RevealTile();
        OnSecondTilePressed?.Invoke();
        OnDisableTileExcepThis(null);

        StartCoroutine(HandleCombination(pressedTile, isCombination));
    }

    private void OnViewCardEnabled()
    {
        Tile otherTile = _tiles.First(t => t != _firstPressedTile && _firstPressedTile.TileType == t.TileType);
        otherTile.HighlightTile();
        OnDisableTileExcepThis?.Invoke(otherTile);
    }

    private void OnSwapCardEnabled()
    {
        OnDisableWhenEggOrPlayer?.Invoke();
        _isSwapCardEnabled = true;
    }

    #endregion

    #region Private Methods

    private IEnumerator HandleCombination(Tile secondPressedTile, bool isCombination)
    {
        if (isCombination)
        {
            yield return new WaitForSecondsRealtime(.3f);

            _firstPressedTile.RevealTileByPlayer(_gameSceneManager.CurrentPlayer.PlayerIcon);
            _tilesToReset.Enqueue(_firstPressedTile);

            secondPressedTile.RevealTileByPlayer(_gameSceneManager.CurrentPlayer.PlayerIcon);
            _tilesToReset.Enqueue(secondPressedTile);

            _tileTypesToReset.Enqueue(_firstPressedTile.TileType);
        }
        else
        {
            yield return new WaitForSecondsRealtime(1.5f);

            _gameSceneManager.SpendingSteps(1);
            _firstPressedTile.ResetTile();
            secondPressedTile.ResetTile();
        }

        bool isEggCombination = isCombination && (_tileEgg == _firstPressedTile || _tileEgg == secondPressedTile);
        if (isEggCombination)
        {
            _tileEgg.HasEgg = false;
            _tileEgg = null;
        }

        _gameSceneManager.FinishTurn(isCombination, isEggCombination);

        // Reset
        _firstPressedTile = null;
        OnEnableTileIfPossible.Invoke();
    }

    #endregion

    #region Public Methods

    public void ResetBoard()
    {
        TileSpriteContainer tileSpriteContainer = TileSpriteContainer.Instance;
        _tileTypesToReset = new Queue<TileType>(_tileTypesToReset.OrderBy(_ => UnityEngine.Random.value));
        _tilesToReset = new Queue<Tile>(_tilesToReset.OrderBy(_ => UnityEngine.Random.value));

        while (_tilesToReset.Count > 0)
        {
            TileType nextType = _tileTypesToReset.Dequeue();

            // Tile 1
            _tilesToReset.Dequeue().ResetTile(nextType, tileSpriteContainer.GetTileSpriteByType(nextType));

            // Tile 2
            _tilesToReset.Dequeue().ResetTile(nextType, tileSpriteContainer.GetTileSpriteByType(nextType));
        }
    }

    public void SetManagerContext(GameSceneManager gameSceneManager)
    {
        _gameSceneManager = gameSceneManager;
    }

    public void SpawnEgg()
    {
        List<Tile> availableTiles = _tiles.Where(t => t.TileState == TileState.Unrevealed).ToList();

        int randomTile = UnityEngine.Random.Range(0, availableTiles.Count);
        _tileEgg = availableTiles[randomTile];
        _tileEgg.SpawnEgg();
        if (_isSwapCardEnabled)
            _tileEgg.DisableTile();
    }

    #endregion

    #region Swap Cards Logic


    private void SwapTiles(Tile tileA, Tile tileB)
    {
        Transform parentA = tileA.transform.parent;
        Transform parentB = tileB.transform.parent;

        int indexA = tileA.transform.GetSiblingIndex();
        int indexB = tileB.transform.GetSiblingIndex();

        if (parentA == parentB)
        {
            tileA.transform.SetSiblingIndex(indexB);
            tileB.transform.SetSiblingIndex(indexA);        }
        else
        {
            tileA.transform.SetParent(null);
            tileB.transform.SetParent(null);

            tileA.transform.SetParent(parentB);
            tileB.transform.SetParent(parentA);

            tileA.transform.SetSiblingIndex(indexB);
            tileB.transform.SetSiblingIndex(indexA);
        }


        int logicIndexA = _tiles.IndexOf(tileA);
        int logicIndexB = _tiles.IndexOf(tileB);
        _tiles[logicIndexA] = tileB;
        _tiles[logicIndexB] = tileA;
    }

    private void HandleSwapCard(Tile pressedTile)
    {
        if (!_firstPressedTile)
        {
            _firstPressedTile = pressedTile;
            OnFirstTilePressed?.Invoke(pressedTile.HasEgg);
            _firstPressedTile.HighlightTile(false);
            return;
        }

        // VFX
        OnSecondTilePressed?.Invoke();
        OnDisableTileExcepThis(_firstPressedTile);

        _firstPressedTile.HighlightTile(false);
        pressedTile.HighlightTile(false);

        // Swap Tile
        SwapTiles(_firstPressedTile, pressedTile);
        _isSwapCardEnabled = false;

        StartCoroutine(DelayResetAfterSwappingCards());
    }

    IEnumerator DelayResetAfterSwappingCards()
    {
        yield return new WaitForSecondsRealtime(1.5f);

        // Reset
        _gameSceneManager.UpdatePowerUpManager();
        _firstPressedTile = null;
    }

    #endregion
}
