using System;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public static event Action<Tile> OnTilePressed;
    #region Editor Variables

    [SerializeField]
    private Button _tileButton;

    [SerializeField]
    private Sprite _eggSprite;

    [SerializeField]
    private Sprite _highlightedSprite;

    [SerializeField]
    private Color _disableColor;

    #endregion

    // Tile Type
    private TileType _tileType;
    public TileType TileType { get => _tileType; set => _tileType = value; }

    // Tile State
    private TileState _tileState = TileState.Unrevealed;
    public TileState TileState { get => _tileState; set => _tileState = value; }

    // Has the egg
    private bool _hasEgg;
    public bool HasEgg { get => _hasEgg; set => _hasEgg = value; }

    #region Private Variables

    private Sprite _originalSprite;
    private Sprite _tileObjectSprite;
    private Color _originalColor;

    #endregion

    #region Unity Editor

    private void OnEnable()
    {
        BoardManager.OnDisableTileExcepThis += OnDisableTileExcepThis;
        BoardManager.OnEnableTileIfPossible += OnEnableTileIfPossible;
        BoardManager.OnDisableWhenEggOrPlayer += OnDisableWhenEggOrPlayer;
    }

    private void OnDisable()
    {
        BoardManager.OnDisableTileExcepThis -= OnDisableTileExcepThis;
        BoardManager.OnEnableTileIfPossible -= OnEnableTileIfPossible;
        BoardManager.OnDisableWhenEggOrPlayer -= OnDisableWhenEggOrPlayer;
    }

    #endregion

    #region Private Methods

    private void TilePressed()
    {
        OnTilePressed?.Invoke(this);
    }

    #endregion

    #region Event Listener

    private void OnDisableTileExcepThis(Tile tile)
    {
        _tileButton.interactable = tile == this;

        if (tile == null || _tileButton.interactable)
            return;

        _tileButton.image.color = _disableColor;
    }

    private void OnEnableTileIfPossible()
    {
        _tileButton.image.color = _originalColor;
        if (_tileState == TileState.Highlighted)
            ResetTile();

        if (_tileState == TileState.Player)
            return;

        _tileButton.interactable = true;
    }

    private void OnDisableWhenEggOrPlayer()
    {
        if (_tileState != TileState.Player && !_hasEgg)
            return;

        DisableTile();
    }

    #endregion

    #region Public Methods

    public void Initialize(TileType initialType, Sprite initialObjectSprite)
    {
        _originalSprite = _tileButton.image.sprite;
        _originalColor = _tileButton.image.color;

        //
        // debug
        //_tileButton.image.sprite = initialObjectSprite;
        // debug
        //
        _tileObjectSprite = initialObjectSprite;
        _tileType = initialType;
        _tileButton.onClick.AddListener(TilePressed);
    }

    public void RevealTile()
    {
        _tileState = TileState.Revealed;
        _tileButton.image.sprite = _tileObjectSprite;
        _tileButton.interactable = false;
    }

    public void RevealTileByPlayer(Sprite playerSprite)
    {
        _tileState = TileState.Player;
        _tileButton.image.sprite = playerSprite;
        _tileButton.interactable = false;
    }

    public void HighlightTile(bool isInteractable = true)
    {
        _tileButton.image.color = _originalColor;
        _tileState = TileState.Highlighted;
        if(!_hasEgg)
            _tileButton.image.sprite = _highlightedSprite;
        _tileButton.interactable = isInteractable;
    }

    public void DisableTile()
    {
        _tileButton.interactable = false;
        _tileButton.image.color = _disableColor;
    }

    public void ResetTile(TileType newType = TileType.None, Sprite newObjectSprite = null)
    {
        _tileButton.image.sprite = _hasEgg ? _eggSprite : _originalSprite;
        _tileButton.interactable = true;
        _tileState = TileState.Unrevealed;

        if (newType == TileType.None || newObjectSprite == null)
            return;

        _tileType = newType;
        _tileObjectSprite = newObjectSprite;

        //
        // debug
        //_tileButton.image.sprite = _tileObjectSprite;
        // debug
        //
    }

    public void SpawnEgg()
    {
        _hasEgg = true;
        _tileButton.image.sprite = _eggSprite;
    }

    #endregion
}
