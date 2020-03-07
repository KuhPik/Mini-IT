using Kuhpik;
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

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource backgroundSource;
    [SerializeField] private SoundData[] datas;

    /// <summary>
    /// Playing the SFX of specified type.
    /// </summary>
    public void PlaySound(SoundType type)
    {
        sfxSource.PlayOneShot(GetClip(type));
    }

    /// <summary>
    /// Changes background music to specified type and playing it.
    /// </summary>
    public void ChangeMusic(SoundType type)
    {
        backgroundSource.clip = GetClip(type);
        backgroundSource.Play();
    }

    /// <summary>
    /// Changes the MASTER-volume.
    /// </summary>
    public void ChangeVolume(float value)
    {
        sfxSource.outputAudioMixerGroup.audioMixer.SetFloat("Volume", value);
    }

    /// <summary>
    /// Changes volume of sfx or backround-music channel.
    /// </summary>
    public void ChangeVolume(float value, bool isSFX)
    {
        var source = isSFX ? sfxSource : backgroundSource;
        source.volume = value;
    }

    #region private

    private void Start()
    {
        HandleCollection();
    }

    private void HandleCollection()
    {
        _soundsCollection = datas.ToDictionary(x => x.type, x => x.clips);
    }

    private AudioClip GetClip(SoundType type)
    {
        return _soundsCollection[type].GetRandom();
    }

    #endregion private
}