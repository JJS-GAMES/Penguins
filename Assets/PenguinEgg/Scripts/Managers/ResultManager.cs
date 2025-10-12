using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : Manager
{
    #region Editor Variables

    [SerializeField]
    private GameObject _resultCanvas;

    [Header("UI General Results")]
    [SerializeField]
    private Image _finalPenguin;

    [SerializeField]
    private Image _finalLabel;

    [SerializeField]
    private Sprite _winnerLabel;

    [SerializeField]
    private Sprite _looserLabel;

    [SerializeField]
    private Sprite _winnerBackground;

    [SerializeField]
    private Sprite _looserBackground;

    [SerializeField]
    private Sprite _loosePenguin;

    [Header("Player 1")]
    [SerializeField]
    private Sprite _yellowPenguin;

    [SerializeField]
    private Image _player1Background;

    [SerializeField]
    private TextMeshProUGUI _combinations1Text;

    [SerializeField]
    private TextMeshProUGUI _egg1Text;

    [SerializeField]
    private TextMeshProUGUI _coins1Text;

    [Header("Player 2")]
    [SerializeField]
    private GameObject _player2Results;

    [SerializeField]
    private Sprite _redPenguin;

    [SerializeField]
    private Image _player2Background;

    [SerializeField]
    private TextMeshProUGUI _combinations2Text;

    [SerializeField]
    private TextMeshProUGUI _egg2Text;

    [SerializeField]
    private TextMeshProUGUI _coins2Text;

    #endregion

    #region Manager Implementation

    public override void Initialize()
    {
        _player2Results.SetActive(GameManager.GameMode == GameMode.MultiPlayer);
        _resultCanvas.SetActive(false);
    }

    #endregion

    public void FinishGame(PlayerResults player1Results, PlayerResults player2Results, Winner winner)
    {
        // Player 1
        _combinations1Text.text = player1Results.CombinationsSpent.ToString();
        _egg1Text.text = player1Results.EggRetention.ToString();
        _coins1Text.text = player1Results.CoinsSpent.ToString();

        // Player 2
        _combinations2Text.text = player2Results.CombinationsSpent.ToString();
        _egg2Text.text = player2Results.EggRetention.ToString();
        _coins2Text.text = player2Results.CoinsSpent.ToString();

        switch(winner)
        {
            case Winner.Player1:
                _finalPenguin.sprite = _yellowPenguin;
                _player1Background.sprite = _winnerBackground;
                _player2Background.sprite = _looserBackground;
                _finalLabel.sprite = _winnerLabel;
                break;
            case Winner.Player2:
                _finalPenguin.sprite = _redPenguin;
                _player2Background.sprite = _winnerBackground;
                _player1Background.sprite = _looserBackground;
                _finalLabel.sprite = _winnerLabel;
                break;
            case Winner.Loose:
                _finalPenguin.sprite = _loosePenguin;
                _player1Background.sprite = _looserBackground;
                _player2Background.sprite = _looserBackground;
                _finalLabel.sprite = _looserLabel;
                break;
        }

        if (GameManager.GameMode == GameMode.SinglePlayer)
            _player2Results.SetActive(false);

        _resultCanvas.SetActive(true);
    }
}
