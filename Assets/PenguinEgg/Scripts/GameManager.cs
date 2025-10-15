using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

public class GameManager : MonoBehaviour
{
    private string _menuSceneName = "Menu";
    private string _gameSceneName = "Game";

    [SerializeField]
    private InitialGameParameters _gameParameters;
    public InitialGameParameters GameParameters { get => _gameParameters; }

    private static GameMode _gameMode;
    public static GameMode GameMode { get => _gameMode; }

    private static Language _language;
    public static Language Language { get => _language; }

    private static GameManager _instance;

    private bool _isRussian = true;
    public bool IsRussian { get => _isRussian; }
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameManager>();
                if (_instance == null)
                {
                    Debug.LogError("GameManager instance is null");
                    throw new InvalidOperationException("GameManager instance is not initialized");
                }
            }

            return _instance;
        }
    }

    public void Awake()
    {
        if (_instance)
        {
            Instance._gameParameters = _gameParameters;
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            _gameMode = _gameParameters.DEFAULT_GAME_MODE;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ToggleLanguage()
    {
        _isRussian = !_isRussian;

        if (_isRussian)
        {
            YG2.lang = "ru";
            _language = Language.Russian;
        }
        else
        {
            YG2.lang = "en";
            _language = Language.England;
        }
    }

    public void ChangeSceneToGamePlay(bool isMultiPlayer)
    {
        _gameMode = isMultiPlayer ? GameMode.MultiPlayer : GameMode.SinglePlayer;
        ChangeScene(_gameSceneName);
    }

    public void ChangeSceneToGamePlay()
    {
        ChangeScene(_gameSceneName);
    }

    public void ChangeSceneToMenu()
    {
        ChangeScene(_menuSceneName);
    }

    private void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
