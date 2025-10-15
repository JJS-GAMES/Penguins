using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : Manager
{
    #region Editor Variables
    [SerializeField]
    private GameObject _eggFeedback;

    [SerializeField]
    private TextMeshProUGUI _eggFeedbackText;

    [SerializeField]
    private RectTransform _powerUpLayout;

    [Header("Turn VFX")]
    [SerializeField]
    private Color _penguinEnabledColor;

    [SerializeField]
    private Color _penguinDisabledColor;

    [Header("Player 1")]
    [SerializeField]
    private Image _player1Penguin;

    [SerializeField]
    private GameObject _player1Steps;

    [SerializeField]
    private TextMeshProUGUI _player1StepsText;

    [SerializeField]
    private TextMeshProUGUI _player1CoinText;

    [Header("Player 2")]
    [SerializeField]
    private Image _player2Penguin;

    [SerializeField]
    private GameObject _player2Coin;

    [SerializeField]
    private TextMeshProUGUI _player2CoinText;

    [Header("Round Progress")]
    [SerializeField]
    private Slider _progressBar;

    [SerializeField]
    private TextMeshProUGUI _progressText;

    #endregion

    private Image _currentPlayerPenguin;
    private GameMode _currentGameMode;

    #region Manager Implementation

    public override void Initialize()
    {
        if(_isInitialized)
            return;

        _currentGameMode = GameManager.GameMode;
        _progressBar.maxValue = GameManager.Instance.GameParameters.MAX_ROUNDS;
        _progressBar.value = 0;
        _progressText.text = $"{_progressBar.value} / {_progressBar.maxValue}";
        _eggFeedback.SetActive(false);

        _currentPlayerPenguin = _player1Penguin;
        _player1Penguin.color = _penguinEnabledColor;
        _player2Penguin.color = _penguinDisabledColor;

        InitPlayersUI();

        _isInitialized = true;
    }

    #endregion

    #region Unity Callbacks

    private void OnEnable()
    {
        PowerUpManager.OnPowerUpDisabled += AdjustPowerUpLayout;
    }

    private void OnDisable()
    {
        PowerUpManager.OnPowerUpDisabled -= AdjustPowerUpLayout;
    }

    #endregion

    #region Private Methods

    private void InitPlayersUI()
    {
        _player1CoinText.text = "0";

        _player2CoinText.text = "0";
        _player2Coin.SetActive(_currentGameMode == GameMode.MultiPlayer);
        _player2Penguin.gameObject.SetActive(_currentGameMode == GameMode.MultiPlayer);

        _player1StepsText.text = GameManager.Instance.GameParameters.DEFAULT_STEPS_COUNT.ToString();
        _player1Steps.SetActive(_currentGameMode == GameMode.SinglePlayer);
    }

    private void AdjustPowerUpLayout(RectTransform deactivatedButton)
    {
        float buttonWidth = deactivatedButton.rect.width * _powerUpLayout.localScale.x;
        float newWidth = _powerUpLayout.rect.width - buttonWidth;
        _powerUpLayout.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
    }

    #endregion

    #region Public Methods

    public void AddEggProgress()
    {
        _progressBar.value++;
        _progressText.text = $"{_progressBar.value} / {_progressBar.maxValue}";
    }

    public void UpdatePlayerCoinText(float coin, int playerIndex)
    {

        if (playerIndex == 0)
            _player1CoinText.text = coin.ToString();
        else if (playerIndex == 1)
            _player2CoinText.text = coin.ToString();
    }

    public void UpdatePlayerStepsText(float steps)
    {
        _player1StepsText.text = steps.ToString();
    }

    public void StartEggCountdown(Sprite eggPlayerSprite, int countDownValue, int playerIndex)
    {
        _eggFeedback.SetActive(true);
        _eggFeedbackText.text = countDownValue.ToString();

        if (playerIndex == 0)
            _player1Penguin.sprite = eggPlayerSprite;
        else if (playerIndex == 1)
            _player2Penguin.sprite = eggPlayerSprite;
    }

    public void UpdateEggCountdown(int countDown)
    {
        _eggFeedbackText.text = countDown.ToString();

        if (countDown > 0)
            return;

        _eggFeedback.SetActive(false);
    }

    public void UpdateEggPossesion(Sprite playerSprite, int playerIndex)
    {
        if (playerIndex == 0)
            _player1Penguin.sprite = playerSprite;
        else if (playerIndex == 1)
            _player2Penguin.sprite = playerSprite;
    }

    public void UpdatePlayerTurn(int currentPlayerIndex)
    {
        if (_currentGameMode == GameMode.SinglePlayer)
            return;
     
        _player1Penguin.color = currentPlayerIndex == 0 ? _penguinEnabledColor : _penguinDisabledColor;
        _player2Penguin.color = currentPlayerIndex == 1 ? _penguinEnabledColor : _penguinDisabledColor;
    }
    #endregion
}
