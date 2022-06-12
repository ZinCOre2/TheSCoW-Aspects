using System;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
    public AudioManager AudioManager;
    
    public Slider MusicVolumeSlider;
    public Slider SoundVolumeSlider;

    private bool _useLocalManager = false;

    private void OnEnable()
    {
        _useLocalManager = GameController.Instance == null;
        Debug.Log($"Pause panel uses local manager: {_useLocalManager}");
        
        if (_useLocalManager)
        {
            MusicVolumeSlider.value = AudioManager.AudioVolumeData.MusicVolume;
            SoundVolumeSlider.value = AudioManager.AudioVolumeData.SoundVolume;
        }
        else
        {
            MusicVolumeSlider.value = GameController.Instance.AudioManager.AudioVolumeData.MusicVolume;
            SoundVolumeSlider.value = GameController.Instance.AudioManager.AudioVolumeData.SoundVolume;
        }

        SubscribeEvents();
    }
    private void OnDisable()
    {
        UnsubscribeEvents();
    }

    public void SubscribeEvents()
    {
        MusicVolumeSlider.onValueChanged.AddListener(ChangeMusicSliderValue);
        SoundVolumeSlider.onValueChanged.AddListener(ChangeSoundSliderValue);
    }
    public void UnsubscribeEvents()
    {
        MusicVolumeSlider.onValueChanged.RemoveListener(ChangeMusicSliderValue);
        SoundVolumeSlider.onValueChanged.RemoveListener(ChangeSoundSliderValue);
    }

    public void ChangeMusicSliderValue(float value)
    {
        if (_useLocalManager)
        {
            AudioManager.AudioVolumeData.MusicVolume = value;
            AudioManager.UpdateMusicSources(value);
        }
        else
        {
            GameController.Instance.AudioManager.AudioVolumeData.MusicVolume = value;
            GameController.Instance.AudioManager.UpdateMusicSources(value);
        }
    }
    public void ChangeSoundSliderValue(float value)
    {
        if (_useLocalManager)
        {
            AudioManager.AudioVolumeData.SoundVolume = value;
            AudioManager.UpdateSoundSources(value);
        }
        else
        {
            GameController.Instance.AudioManager.AudioVolumeData.SoundVolume = value;
            GameController.Instance.AudioManager.UpdateSoundSources(value);
        }
    }
}
