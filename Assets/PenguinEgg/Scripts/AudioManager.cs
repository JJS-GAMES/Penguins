using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;

public enum SFXType
{
    PowerUp = 0,
    Win = 1,
    Loose = 2,

    SFXCount = 3
}

public class AudioManager : MonoBehaviour
{
    #region Editor Variables

    [Header("Audio Mixer")]
    [SerializeField]
    private AudioMixer _musicMixer;

    [SerializeField]
    private AudioMixer _sfxMixer;

    [Header("Audio Sources")]
    [SerializeField]
    private AudioSource _musicSource;

    [SerializeField]
    private AudioSource _sfxSourcePrefab;

    #endregion
    
    private Dictionary<SFXType, AudioSource> _sfxPool = new();

    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<AudioManager>();
                if (_instance == null)
                    throw new InvalidOperationException("AudioManager is not initialized");
            }
            return _instance;
        }
    }

    private static bool _isMusicMuted = false;
    public static bool IsMusicMuted { get => _isMusicMuted; }

    private static bool _isSFXMuted = false;
    public static bool IsSFXMuted { get => _isSFXMuted; }

    void Awake()
    {
        if (_instance)
        {
            if (_instance._musicSource)
                Destroy(_musicSource);
            else
                _instance._musicSource = _musicSource;
            _instance._sfxSourcePrefab = _sfxSourcePrefab;

            Destroy(gameObject);
            return;
        }
        else
        {
            _instance = this;
            _musicSource.Play();
            DontDestroyOnLoad(gameObject);
        }

        InitializeAudioSources();
    }

    private void InitializeAudioSources()
    {
        foreach (SFXType type in Enum.GetValues(typeof(SFXType)))
        {
            AudioSource audioSource = Instantiate(_sfxSourcePrefab, transform).GetComponent<AudioSource>();
            _sfxPool[type] = audioSource;
        }

        _isMusicMuted = !_isMusicMuted;
        ToggleMusicVolume();

        _isSFXMuted = !_isSFXMuted;
        ToggleSFXVolume();
    }

    public void PlaySFX(SFXType type, AudioClip clip)
    {
        if (!_sfxPool.ContainsKey(type))
        {
            Debug.LogWarning($"SFX {type} is not registered");
            return;
        }

        AudioSource audioSource = _sfxPool[type];

        audioSource.PlayOneShot(clip);
    }

    public void ToggleMusicVolume()
    {
        if (!_musicMixer)
        {
            Debug.LogError("Music mixer is not assigned in the inspector");
            return;
        }

        _isMusicMuted = !_isMusicMuted;

        string snapshotName = _isMusicMuted ? "Muted" : "Normal";
        AudioMixerSnapshot snapshot = _musicMixer.FindSnapshot(snapshotName);
        snapshot.TransitionTo(0.1f);
    }

    public void ToggleSFXVolume()
    {
        if (!_sfxMixer)
        {
            Debug.LogError("SFX mixer is not assigned in the inspector");
            return;
        }

        _isSFXMuted = !_isSFXMuted;

        string snapshotName = _isSFXMuted ? "Muted" : "Normal";
        AudioMixerSnapshot snapshot = _sfxMixer.FindSnapshot(snapshotName);
        snapshot.TransitionTo(0.1f);
    }
}
