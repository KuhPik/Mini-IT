using System;
using System.Collections;
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

    public void PlaySound(SoundType type)
    {
        _sfxSource.PlayOneShot(GetClip(type));
    }

    public void ChangeMusic(SoundType type)
    {
        _backgroundSource.clip = GetClip(type);
        _backgroundSource.Play();
    }

    public void ChangeVolume(float value)
    {
        _sfxSource.outputAudioMixerGroup.audioMixer.SetFloat("Volume", value);
    }

    public void ChangeVolume(float value, bool isMusic)
    {
        var source = isMusic ? _backgroundSource : _sfxSource;
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