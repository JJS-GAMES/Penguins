using System;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class MenuSceneManager : Manager
{
    [Serializable]
    public class SoundItem
    {
        [SerializeField]
        public Button SoundButton;

        [SerializeField]
        public Sprite MuteSprite;

        [SerializeField]
        public Sprite UnmuteSprite;


    }

    [Serializable]
    public class LanguageItem
    {
        [SerializeField]
        public Button LanguageButton;

        [SerializeField]
        public Sprite RussianSprite;

        [SerializeField]
        public Sprite EnglandSprite;


    }

    #region Editor Variables

    [Header("Managers")]
    [SerializeField]
    private ParametersManager _parametersManager;

    [SerializeField]
    private RulesManager _rulesManager;

    [Header("Menu Items")]
    [SerializeField]
    private GameObject _menuCanvas;

    [SerializeField]
    private Button _playButton;

    [SerializeField]
    private Button _multiPlayerButton;

    [SerializeField]
    private Button _singlePlayerButton;

    [SerializeField]
    private Button _rulesButton;

    [SerializeField]
    private SoundItem _musicItem;

    [SerializeField]
    private SoundItem _sfxItem;

    [SerializeField]
    private LanguageItem _languageItem;



    #endregion

    private GameManager _gameManager;
    private AudioManager _audioManager;
    private bool _isMenuOpen = true;

    #region IManager Implementation

    public override void Initialize()
    {
        if (_isInitialized)
            return;

        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;

        InitializeButtons();
        InitializeManagers();

        _isInitialized = true;
    }

    #endregion

    #region Unity Callbacks

    private void OnEnable()
    {
        ParametersManager.OnParametersCompleted += OnToggleMenu;
        RulesManager.OnBackButton += OnToggleMenu;
    }

    private void OnDisable()
    {
        ParametersManager.OnParametersCompleted -= OnToggleMenu;
        RulesManager.OnBackButton -= OnToggleMenu;
    }

    private void Start()
    {
        Initialize();
    }

    #endregion

    private void OnToggleMenu()
    {
        if (!_menuCanvas)
        {
            Debug.LogError("Menu Canvas is not assigned in the inspector");
            return;
        }

        _isMenuOpen = !_isMenuOpen;
        _menuCanvas.SetActive(_isMenuOpen);
    }

    private void InitializeManagers()
    {
        if (!_parametersManager)
            Debug.LogError("Parameters Manager is not assigned in the inspector");
        else
            _parametersManager.Initialize();

        if (!_rulesManager)
            Debug.LogError("Rules Manager is not assigned in the inspector");
        else
            _rulesManager.Initialize();
    }

    private void InitializeButtons()
    {
        _playButton.onClick.AddListener(() => DisplayParameters());
        _rulesButton.onClick.AddListener(() => DisplayRules());

        _multiPlayerButton.onClick.AddListener(() => StartGame(true));
        _singlePlayerButton.onClick.AddListener(() => StartGame(false));

        _musicItem.SoundButton.onClick.AddListener(() => ToggleMusicVolume());
        _musicItem.SoundButton.image.sprite = AudioManager.IsMusicMuted ? _musicItem.MuteSprite : _musicItem.UnmuteSprite;

        _sfxItem.SoundButton.onClick.AddListener(() => ToggleSFXVolume());
        _sfxItem.SoundButton.image.sprite = AudioManager.IsSFXMuted ? _sfxItem.MuteSprite : _sfxItem.UnmuteSprite;

        _languageItem.LanguageButton.onClick.AddListener(() => ToggleLanguage());
    }

    private void ToggleMusicVolume()
    {
        _audioManager.ToggleMusicVolume();
        _musicItem.SoundButton.image.sprite = AudioManager.IsMusicMuted ? _musicItem.MuteSprite : _musicItem.UnmuteSprite;
    }

    private void ToggleSFXVolume()
    {
        _audioManager.ToggleSFXVolume();
        _sfxItem.SoundButton.image.sprite = AudioManager.IsSFXMuted ? _sfxItem.MuteSprite : _sfxItem.UnmuteSprite;
    }

    private void ToggleLanguage()
    {
        GameManager.Instance.ToggleLanguage();

        if (GameManager.Language == Language.Russian)
        {
            _languageItem.LanguageButton.image.sprite = _languageItem.RussianSprite;
            YG2.SwitchLanguage("ru");
        }
        else
        {
            _languageItem.LanguageButton.image.sprite = _languageItem.EnglandSprite;
            YG2.SwitchLanguage("en");
        }
    }
    
    private void DisplayParameters()
    {
        if (!_parametersManager)
        {
            Debug.LogError("Parameters Manager is not assigned in the inspector");
            return;
        }

        _parametersManager.ToggleParameters();
    }

    private void DisplayRules()
    {
        if (!_rulesManager)
        {
            Debug.LogError("Rules Manager is not assigned in the inspector");
            return;
        }

        _rulesManager.ToggleRules(true);
    }

    private void StartGame(bool isMultiPlayer)
    {
        _gameManager.ChangeSceneToGamePlay(isMultiPlayer);
    }
}
