using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [Serializable]
    protected struct SoundData
    {
        public SoundType type;
        public AudioClip[] clips;
    }

    private Dictionary<SoundType, AudioClip[]> _soundsCollection;

    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioSource _backgroundSource;
    [SerializeField] private SoundData[] _datas;

    /// <summary>
    /// Playing the SFX of specified type.
    /// </summary>
    public void PlaySound(SoundType type)
    {
        _sfxSource.PlayOneShot(GetClip(type));
    }

    /// <summary>
    /// Changes background music to specified type and playing it.
    /// </summary>
    public void ChangeMusic(SoundType type)
    {
        _backgroundSource.clip = GetClip(type);
        _backgroundSource.Play();
    }

    /// <summary>
    /// Changes the MASTER-volume.
    /// </summary>
    public void ChangeVolume(float value)
    {
        _sfxSource.outputAudioMixerGroup.audioMixer.SetFloat("Volume", value);
    }

    /// <summary>
    /// Changes volume of sfx or backround-music channel.
    /// </summary>
    public void ChangeVolume(float value, bool isSFX)
    {
        var source = isSFX ? _sfxSource : _backgroundSource;
        source.volume = value;
    }

    #region private

    private void Start()
    {
        HandleCollection();
    }

    private void HandleCollection()
    {
        _soundsCollection = _datas.ToDictionary(x => x.type, x => x.clips);
    }

    private AudioClip GetClip(SoundType type)
    {
        return _soundsCollection[type].GetRandom();
    }

    #endregion private
}