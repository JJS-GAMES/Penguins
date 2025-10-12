using System;
using UnityEngine;

public struct PlayerResults
{
    public int CombinationsSpent;
    public int EggRetention;
    public int CoinsSpent;
}

public class PlayerActor : MonoBehaviour
{
    public event Action<float, int> OnCoinUpdated;

    [SerializeField]
    private Sprite _playerPenguin;
    public Sprite PlayerPenguin { get => _playerPenguin; }

    [SerializeField]
    private Sprite _playerEgg;
    public Sprite PlayerEgg { get => _playerEgg; }

    [SerializeField]
    private Sprite _playerTileIcon;
    public Sprite PlayerIcon { get => _playerTileIcon; }

    // Is warming egg
    private bool _isWarmingEgg = false;
    public bool IsWarmimgEgg { get => _isWarmingEgg; set => _isWarmingEgg = value; }

    // Player Index
    private int _playerIndex = 0;
    public int PlayerIndex { get => _playerIndex; }

    // Player Results
    private PlayerResults _playerResults;
    public PlayerResults PlayerResults { get => _playerResults; }

    // Current Coins
    private int _currentCoins = 0;
    public int CurrentCoins { get => _currentCoins; }

    private int _combinationsMade = 0;

    public void InitPlayer(int playerIndex)
    {
        _playerIndex = playerIndex;
        _currentCoins = GameManager.Instance.GameParameters.INITIAL_PLAYER_COINS;

        _playerResults.EggRetention = 0;
        _playerResults.CoinsSpent = 0;
        _playerResults.CombinationsSpent = 0;
    }

    public void ReceiveCoin(int pricePerCombination)
    {
        _currentCoins += _combinationsMade * pricePerCombination;

        _combinationsMade = 0;
        OnCoinUpdated?.Invoke(_currentCoins, _playerIndex);
    }

    public void PurchaseItem(int price)
    {
        _currentCoins -= price;
        _playerResults.CoinsSpent += price;
        OnCoinUpdated?.Invoke(_currentCoins, _playerIndex);
    }

    public void AddCombination()
    {
        _combinationsMade++;
        _playerResults.CombinationsSpent++;
    }

    public void AddEggRetention()
    {
        _playerResults.EggRetention++;
    }
}
