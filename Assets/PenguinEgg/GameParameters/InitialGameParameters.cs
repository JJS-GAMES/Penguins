using UnityEngine;
using YG;

[CreateAssetMenu(fileName = "AspectRatioConfig", menuName = "Game/Game Parameters", order = 1)]
public class InitialGameParameters : ScriptableObject
{
    [SerializeField]
    private int _maxRounds = 20;
    public int MAX_ROUNDS { get => _maxRounds; }

    [SerializeField]
    private int _eggCountDown = 3;
    public int EGG_COUNTDOWN { get => _eggCountDown; }

    [SerializeField]
    private int _coinPerCombination = 2;
    public int COIN_PER_COMBINATION { get => _coinPerCombination; }

    [SerializeField]
    private int _eggSpawnTurn = 3;
    public int EGG_SPAWN_TURN { get => _eggSpawnTurn; }

    [SerializeField]
    private int _maxCombinationLimit = 9;
    public int MAX_COMBINATION_LIMIT { get => _maxCombinationLimit; }

    [SerializeField]
    private int _mediumStepsCount = 15;
    public int MEDIUM_STEPS_COUNT { get => _mediumStepsCount; }

    [SerializeField]
    private int _hardStepsCount = 10;
    public int HARD_STEPS_COUNT { get => _hardStepsCount; }

    [SerializeField]
    private int _easyStepsCount = 20;
    public int EASY_STEPS_COUNT { get => _easyStepsCount; }

    [SerializeField]
    private GameMode _defaultGameMode = GameMode.SinglePlayer;
    public GameMode DEFAULT_GAME_MODE { get => _defaultGameMode; }

    [SerializeField]
    private Language _defaultLanguage = Language.Russian;
    public Language DEFAULT_LANGUAGE { get => _defaultLanguage; }

    [SerializeField]
    private int _initialPlayerCoins = 0;
    public int INITIAL_PLAYER_COINS { get => _initialPlayerCoins; }
}
