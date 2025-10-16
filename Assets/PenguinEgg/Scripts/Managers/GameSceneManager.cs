using System;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class GameSceneManager : Manager
{
    public static event Action OnPlayerTurnChanged;

    #region Editor Variables

    [Header("Managers")]
    [SerializeField]
    private BoardManager _boardManager;

    [SerializeField]
    private RulesManager _rulesManager;

    [SerializeField]
    private HUDManager _hudManager;

    [SerializeField]
    private ResultManager _resultManager;

    [SerializeField]
    private PowerUpManager _powerUpManager;

    [Header("Audio")]
    [SerializeField]
    private AudioClip _sfxWinner;

    [SerializeField]
    private AudioClip _sfxLooser;

    [Header("Players")]
    [SerializeField]
    private PlayerActor _player1;

    [SerializeField]
    private PlayerActor _player2;

    [Header("Misc")]
    [SerializeField]
    private Button _rulesButton;

    [SerializeField]
    private Button _resultsHomeButton;

    [SerializeField]
    private Button _rulesHomeButton;

    #endregion

    // Current Player
    private PlayerActor _currentPlayer;
    public PlayerActor CurrentPlayer => _currentPlayer;

    // Current Steps Single Player

    private int _currentSteps = 0;
    public int CurrentSteps { get => _currentSteps; }

    // Current Egg Round
    private int _currentEggRound = 0;
    public int CurrentRound => _currentEggRound;

    #region Private Variables

    private int _currentCombinations = 0;
    private int _combinationLimit = 5;
    private int _turnCounter = 0;
    private EggState _currentEggState = EggState.None;
    private int _currentEggCountdown;
    private GameMode _currentGameMode;
    private GameManager _gameManager;
    private InitialGameParameters _gameParameters;

    #endregion

    #region IManager Implementation

    public override void Initialize()
    {
        if (_isInitialized)
            return;

        _gameManager = GameManager.Instance;
        _gameParameters = _gameManager.GameParameters;
        _currentEggCountdown = _gameParameters.EGG_COUNTDOWN;
        _currentGameMode = GameManager.GameMode;
        _currentPlayer = _player1;
        InitializeManagers();
        InitPlayers();

        string value = YG2.GetFlag("StepsCountFlag");

        if (value == "hard")
        {
            _currentSteps = GameManager.Instance.GameParameters.HARD_STEPS_COUNT;
        }
        else if (value == "easy")
        {
            _currentSteps = GameManager.Instance.GameParameters.EASY_STEPS_COUNT;
        }
        else
        {
            _currentSteps = GameManager.Instance.GameParameters.MEDIUM_STEPS_COUNT;
        }

        _isInitialized = true;
    }

    #endregion

    #region Unity Callbacks

    private void OnEnable()
    {
        // Player Events
        _player1.OnCoinUpdated += _hudManager.UpdatePlayerCoinText;
        _player2.OnCoinUpdated += _hudManager.UpdatePlayerCoinText;
    }

    private void OnDisable()
    {
        // Player Events
        _player1.OnCoinUpdated -= _hudManager.UpdatePlayerCoinText;
        _player2.OnCoinUpdated -= _hudManager.UpdatePlayerCoinText;
    }

    void Start()
    {
        Initialize();
    }

    #endregion

    #region Init

    private void InitializeManagers()
    {
        if (_rulesManager == null)
        {
            Debug.LogError("RulesManager is not assigned in the inspector");
            return;
        }
        _rulesButton.onClick.AddListener(() => DisplayRules());
        _rulesHomeButton.onClick.AddListener(() => BackToMenu());
        _rulesManager.Initialize();

        if (_boardManager == null)
        {
            Debug.LogError("BoardManager is not assigned in the inspector");
            return;
        }
        _boardManager.SetManagerContext(this);
        _boardManager.Initialize();

        if (_hudManager == null)
        {
            Debug.LogError("HUDManager is not assigned in the inspector");
            return;
        }
        _hudManager.Initialize();

        if (_resultManager == null)
        {
            Debug.LogError("ResultManager is not assigned in the inspector");
            return;
        }
        _resultsHomeButton.onClick.AddListener(() => BackToMenu());
        _resultManager.Initialize();

        if (_powerUpManager == null)
        {
            Debug.LogError("PowerUpManager is not assigned in the inspector");
            return;
        }
        _powerUpManager.SetUpGameSceneManager(this);
        _powerUpManager.Initialize();
    }

    private void InitPlayers()
    {
        if (_player1 == null)
        {
            Debug.LogError("Player 1 is not assigned in the inspector");
        }
        _player1.InitPlayer(0);

        if (_player2 == null)
        {
            Debug.LogError("Player 2 is not assigned in the inspector");
        }
        _player2.InitPlayer(1);
    }

    #endregion

    #region Game Logic

    private PlayerActor GetNextPlayer()
    {
        if (_currentGameMode == GameMode.SinglePlayer)
            return _player1;

        return _currentPlayer == _player1 ? _player2 : _player1;
    }

    private void UpdateEggPossesion()
    {
        _currentPlayer.IsWarmimgEgg = true;
        _currentEggState = EggState.Player;
        _hudManager.StartEggCountdown(_currentPlayer.PlayerEgg, _gameParameters.EGG_COUNTDOWN,_currentPlayer.PlayerIndex);
    }

    private void UpdateEggCounter(bool isCombination)
    {
        if (!_currentPlayer.IsWarmimgEgg)
            return;

        _currentEggRound++;
        _currentPlayer.AddEggRetention();
        _hudManager.AddEggProgress();
        if (!isCombination)
        {
            _currentEggCountdown--;
            _hudManager.UpdateEggCountdown(_currentEggCountdown);

            if (_currentEggCountdown == 0)
                ResetEgg();
        }


        if (_currentEggRound != _gameParameters.MAX_ROUNDS)
            return;

        FinishGame();
    }

    private void HandleEggStates(bool isCombination, bool isEggCombination)
    {
        switch (_currentEggState)
        {
            case EggState.None:
                _turnCounter++;
                if (_turnCounter < _gameParameters.EGG_SPAWN_TURN)
                    return;

                _currentEggState = EggState.Spawned;
                _boardManager.SpawnEgg();
                break;
            case EggState.Spawned:
                if (!isEggCombination)
                    return;

                UpdateEggPossesion();
                break;
            case EggState.Player:
                UpdateEggCounter(isCombination);
                break;
        }
    }

    public void ResetEgg(bool isPowerUp = false)
    {
        _currentPlayer.IsWarmimgEgg = false;
        _currentEggCountdown = _gameParameters.EGG_COUNTDOWN;
        _hudManager.UpdateEggCountdown(0);
        _hudManager.UpdateEggPossesion(_currentPlayer.PlayerPenguin, _currentPlayer.PlayerIndex);
        _currentEggState = EggState.Spawned;
        _boardManager.SpawnEgg();
    }

    public void CombinationLimitReached()
    {
        _boardManager.ResetBoard();

        _currentCombinations = 0;

        _player1.ReceiveCoin(_gameParameters.COIN_PER_COMBINATION);

        if (_currentGameMode == GameMode.SinglePlayer)
            return;

        _player2.ReceiveCoin(_gameParameters.COIN_PER_COMBINATION);
    }

    public void ReceiveSteps(int pricePerCombination)
    {
        if (_currentSteps <= 0 || _currentGameMode != GameMode.SinglePlayer) return;

        _currentSteps += pricePerCombination;

        _hudManager.UpdatePlayerStepsText(_currentSteps);
    }

    public void SpendingSteps(int pricePerCombination)
    {
        if (_currentGameMode != GameMode.SinglePlayer)
            return;

        _currentSteps -= pricePerCombination;
        _hudManager.UpdatePlayerStepsText(_currentSteps);

        if (_currentSteps <= 0)
        {
            FinishGame();
        }
    }


    #endregion

    #region Power Ups

    public bool AddCombinationLimitAndReturnState()
    {
        if (_combinationLimit >= _gameParameters.MAX_COMBINATION_LIMIT)
            return true;

        _combinationLimit++;

        return _combinationLimit == _gameParameters.MAX_COMBINATION_LIMIT;
    }

    public void UpdatePowerUpManager() 
    {
        _powerUpManager.UpdatePowerUpVisibility(_currentPlayer);    
    }

    #endregion

    #region End Game

    private void FinishGame()
    {
        Winner winner = Winner.Loose;
        if (_player1.IsWarmimgEgg)
            winner = Winner.Player1;
        else if (_player2.IsWarmimgEgg)
            winner = Winner.Player2;

        bool isWin = winner != Winner.Loose;
        AudioManager.Instance.PlaySFX(isWin ? SFXType.Win : SFXType.Loose, isWin ? _sfxWinner : _sfxLooser);

        _resultManager.FinishGame(_player1.PlayerResults, _player2.PlayerResults, winner);
    }

    public void BackToMenu()
    {
        _gameManager.ChangeSceneToMenu();
    }

    public void FinishTurn(bool isCombination, bool isEggCombination)
    {
        HandleEggStates(isCombination, isEggCombination);

        if (isCombination)
        {
            _currentCombinations++;
            _currentPlayer.AddCombination();
            if (_currentCombinations == _combinationLimit)
                CombinationLimitReached();
        }

        _currentPlayer = GetNextPlayer();
        _hudManager.UpdatePlayerTurn(_currentPlayer.PlayerIndex);
        OnPlayerTurnChanged?.Invoke();
    }

    #endregion

    #region Misc

    private void DisplayRules()
    {
        if (!_rulesManager)
        {
            Debug.LogError("Rules Manager is not assigned in the inspector");
            return;
        }

        _rulesManager.ToggleRules(true);
    }

    #endregion
}
