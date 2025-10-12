using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpManager : Manager
{
    public static event Action<RectTransform> OnPowerUpDisabled;
    public static event Action OnViewCardEnabled;
    public static event Action OnSwapCardEnabled;
    
    #region Editor Variables

    [SerializeField]
    private Button _swapCardButton;

    [SerializeField]
    private Button _viewCardButton;

    [SerializeField]
    private Button _addCombinationsButton;

    [SerializeField]
    private Button _dropEggButton;

    [SerializeField]
    private AudioClip _sfxMoneySpent;
    #endregion

    public static Dictionary<PowerUpType, int> PowerUpPrice = new()  {
        { PowerUpType.SwapCard, 1},
        { PowerUpType.ViewCard, 4},
        { PowerUpType.AddCombinations, 4},
        { PowerUpType.DropEgg, 10},
    };

    private GameSceneManager _gameSceneManager;
    private bool _isFirstTilePressed = false;
    private bool _isCombinationLimitReached = false;
    private bool _isViewing = false;
    private bool _isSwapping = false;

    #region Manager Implementation

    public override void Initialize()
    {
        if (_isInitialized)
            return;

        _swapCardButton.onClick.AddListener(SwapCards);
        _viewCardButton.onClick.AddListener(PowerUpViewCard);
        _addCombinationsButton.onClick.AddListener(AddCombinationLimit);
        _dropEggButton.onClick.AddListener(DropEgg);

        _swapCardButton.interactable = false;
        _viewCardButton.interactable = false;
        _addCombinationsButton.interactable = false;
        _dropEggButton.interactable = false;

        if (GameManager.GameMode != GameMode.MultiPlayer)
        {
            _dropEggButton.gameObject.SetActive(false);
        }

        _isInitialized = true;
    }

    #endregion

    #region Unity Callbacks

    private void OnEnable()
    {
        GameSceneManager.OnPlayerTurnChanged += OnPlayerTurnChanged;
        BoardManager.OnFirstTilePressed += OnFirstTilePressed;
        BoardManager.OnSecondTilePressed += OnSecondTilePressed;
    }

    private void OnDisable()
    {
        GameSceneManager.OnPlayerTurnChanged -= OnPlayerTurnChanged;
        BoardManager.OnFirstTilePressed -= OnFirstTilePressed;
        BoardManager.OnSecondTilePressed -= OnSecondTilePressed;
    }

    #endregion

    #region Private Methods

    private void OnPlayerTurnChanged()
    {
        PlayerActor currentPlayer = _gameSceneManager.CurrentPlayer;
        UpdatePowerUpVisibility(currentPlayer);
    }

    private void OnFirstTilePressed(bool hasEgg)
    {
        PlayerActor currentPlayer = _gameSceneManager.CurrentPlayer;
        int currentCoins = currentPlayer.CurrentCoins;

        _isFirstTilePressed = true;
        _viewCardButton.interactable = !hasEgg && !_isSwapping && currentCoins >= PowerUpPrice[PowerUpType.ViewCard];
        _swapCardButton.interactable = false;
    }

    private void OnSecondTilePressed()
    {
        _viewCardButton.interactable = false;
        _swapCardButton.interactable = false;
        _addCombinationsButton.interactable = false;
        _dropEggButton.interactable = false;

        _isFirstTilePressed = false;
        _isViewing = false;
        _isSwapping = false;
    }

    private void PurchaseItem(PowerUpType powerUp)
    {
        PlayerActor currentPlayer = _gameSceneManager.CurrentPlayer;
        currentPlayer.PurchaseItem(PowerUpPrice[powerUp]);
        AudioManager.Instance.PlaySFX(SFXType.PowerUp, _sfxMoneySpent);

        UpdatePowerUpVisibility(currentPlayer);
    }

    public void UpdatePowerUpVisibility(PlayerActor currentPlayer)
    {
        int currentCoins = currentPlayer.CurrentCoins;

        _swapCardButton.interactable = !_isSwapping && !_isViewing && !_isFirstTilePressed && currentCoins >= PowerUpPrice[PowerUpType.SwapCard];
        _viewCardButton.interactable = !_isSwapping && !_isViewing && _isFirstTilePressed && currentCoins >= PowerUpPrice[PowerUpType.ViewCard];
        _addCombinationsButton.interactable = !_isCombinationLimitReached && currentCoins >= PowerUpPrice[PowerUpType.AddCombinations];
        _dropEggButton.interactable = currentPlayer.IsWarmimgEgg && currentCoins >= PowerUpPrice[PowerUpType.DropEgg];
    }

    #endregion

    #region Power Up Callbacks

    private void SwapCards()
    {
        _isSwapping = true;
        PurchaseItem(PowerUpType.SwapCard);
        OnSwapCardEnabled?.Invoke();
    }

    private void PowerUpViewCard()
    {
        _isViewing = true;
        PurchaseItem(PowerUpType.ViewCard);
        OnViewCardEnabled?.Invoke();
    }

    private void AddCombinationLimit()
    {
        if (_isCombinationLimitReached)
            return;

        PurchaseItem(PowerUpType.AddCombinations);
        _isCombinationLimitReached = _gameSceneManager.AddCombinationLimitAndReturnState();

        if (_isCombinationLimitReached)
        {
            OnPowerUpDisabled?.Invoke(_addCombinationsButton.GetComponent<RectTransform>());
            _addCombinationsButton.gameObject.SetActive(false);
        }
    }

    private void DropEgg()
    {
        _gameSceneManager.ResetEgg(true);
        PurchaseItem(PowerUpType.DropEgg);
    }

    #endregion

    #region Public Methods

    public void SetUpGameSceneManager(GameSceneManager manager)
    {
        _gameSceneManager = manager;
    }

    #endregion
}
