using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    [SerializeField] //Полноэкранный режим
    private Toggle toggleFullScreen;
    [SerializeField] //Качество
    private Dropdown dropdownQuality;
    [SerializeField] //Разрешение
    private Dropdown dropdownResolution;
    [SerializeField] //Микшер настройки музыки и звуков
    private AudioMixer audioMixer;
    [SerializeField] //Общая громкость
    private Slider masterSlider;
    [SerializeField] //Громкость музыки
    private Slider musicSlider;
    [SerializeField] //Громкость звуков
    private Slider soundSlider;

    public void ToggleFullScreen() //Полноэкранный переключатель
    {
        Screen.fullScreen = toggleFullScreen.isOn;
        PlayerPrefs.SetString("FullScreen", toggleFullScreen.isOn.ToString());
    }

    public void ChangeQuality() //Изменение качества
    {
        QualitySettings.SetQualityLevel(5 - dropdownQuality.value);
        PlayerPrefs.SetInt("Quality", dropdownQuality.value);
    }

    public void ChangeResolution() //Изменение разрешения
    {
        Screen.SetResolution(Screen.resolutions[dropdownResolution.value].width, Screen.resolutions[dropdownResolution.value].height, toggleFullScreen.isOn);
        PlayerPrefs.SetInt("Resolution", dropdownResolution.value);
    }

    public void ChangeMasterVolume() //Изменение общей громкости
    {
        audioMixer.SetFloat("Master", masterSlider.value);
        PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
    }

    public void ChangeMusicVolume() //Изменение громкости музыки
    {
        audioMixer.SetFloat("Music", musicSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
    }

    public void ChangeSoundVolume() //Изменение громкости звуков
    {
        audioMixer.SetFloat("Sounds", soundSlider.value);
        PlayerPrefs.SetFloat("SoundVolume", soundSlider.value);
    }
}